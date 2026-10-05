using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using DehyaMod.Content.Characters;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 多形态立绘姿势切换(CharlottePosePatch 范式):非 Spine 小人的 NCreature.SetAnimationTrigger
/// 对 Sprite2D 是 no-op——本补丁在迪希雅收到 Attack/Cast/Hit 触发时切换战斗小人 Sprite 纹理,
/// 短暂展示对应姿势立绘后切回站立图。
/// 双攻击形态(裁定 E8):剑斩=≥1 费攻击动作、挥拳=0 费攻击动作——按触发时本回合最后打出的
/// 卡的 GetResolved() 费用取形态(与燃烧心火/木桩等 0 费体系判定同口径);无卡上下文
/// (雷球等非出牌来源)时默认剑斩。剑斩母版为横构图 1536×1024,按 Normal/pose 高度比
/// 补偿缩放保持人物同高、剑势向两侧展开。
/// 死亡姿态(2026-10-05 试玩反馈 4):StartDeathAnim 对非 Spine 小人不发 "Dead" 触发——
/// 直接 postfix 该方法换成倒下立绘(不回切);StartReviveAnim 恢复站立。
/// 补丁由 ModEntry.Init 的显式 PatchAll 挂载;纹理预加载。
/// 待游戏内校准:各姿势的横向偏移(构图主体偏左)与倒下的落地纵向偏移。
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
internal static class DehyaPosePatch
{
    private static readonly Dictionary<string, Texture2D?> Poses = new()
    {
        ["AttackSlash"] = Load("dehya_attack_slash"),
        ["AttackPunch"] = Load("dehya_attack_punch"),
        ["Cast"] = Load("dehya_skill"),
        ["Hit"] = Load("dehya_hit"),
        ["Dead"] = Load("dehya_down"),
    };

    private static readonly Texture2D? Normal = Load("dehya_normal");

    // 每个小人一个版本号:新触发使旧恢复协程失效,避免连击时旧恢复覆盖新姿势。
    private static readonly Dictionary<NCreature, uint> Versions = new();

    // 每个小人的基准缩放缓存(**只存绝对值**):连击时后续触发须以入场缩放为基数;
    // scale.X 的符号是游戏的朝向状态(帝王蟹 SurroundedPower.FlipScale 直接翻转 %Visuals=Body 的
    // scale.X 符号),所有写入必须保留写入时刻的实时符号,否则会把翻转洗掉(2026-10-05 试玩反馈 3)。
    private static readonly Dictionary<NCreature, Vector2> BaseScales = new();

    // 每个小人的基准位置缓存(tscn 里 Sprite 并非在原点):偏移一律从基准叠加,回切还原基准。
    private static readonly Dictionary<NCreature, Vector2> BasePositions = new();

    // 姿势横向偏移(构图主体偏离画面中心的修正;游戏内目检后微调):
    // 剑斩/受击的人物主体偏左,换图时右移使身体位置连续,缓解正面↔侧面切换的"转身感"。
    private static readonly Dictionary<string, float> PoseOffsetX = new()
    {
        ["AttackSlash"] = 30f,
        ["Hit"] = 40f,
    };

    // 倒下(俯卧横图)落地:把横躺的身体从站立悬浮位压到地面附近(游戏内目检后微调)。
    private const float DeadOffsetY = 60f;

    private static Texture2D? Load(string name)
    {
        return GD.Load<Texture2D>($"res://STS2-Dehya/images/characters/{name}.png");
    }

    // 与 AttackAnimDelay(0.15)+出牌节奏相配的展示时长;Hit 稍长便于看清受击反馈。
    private const int AttackHoldMs = 400;
    private const int HitHoldMs = 550;

    private static async void Postfix(NCreature __instance, string trigger)
    {
        try
        {
            if (trigger != "Attack" && trigger != "Cast" && trigger != "Hit")
            {
                return;
            }
            if (!IsDehyaSprite(__instance, out var sprite))
            {
                return;
            }

            Texture2D? pose = trigger switch
            {
                "Attack" => ResolveAttackPose(__instance.Entity.Player),
                "Cast" => Poses["Cast"],
                _ => Poses["Hit"],
            };
            if (pose == null || Normal == null)
            {
                Log.Error($"[STS2-Dehya] pose 缺资源: {trigger} pose={(pose != null)} normal={(Normal != null)}");
                return;
            }

            uint mine = ApplyPose(__instance, sprite, pose, PoseOffsetX.GetValueOrDefault(trigger, 0f), 0f);
            await Task.Delay(trigger == "Hit" ? HitHoldMs : AttackHoldMs);
            RestoreIfCurrent(__instance, sprite, mine);
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Dehya] 小人姿势切换失败({trigger}): {e}");
        }
    }

    /// <summary>死亡姿态:非 Spine 小人不会收到 "Dead" 触发,在 StartDeathAnim 后直接换倒下立绘,不回切。</summary>
    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    [HarmonyPostfix]
    private static void PostfixStartDeathAnim(NCreature __instance)
    {
        try
        {
            if (!IsDehyaSprite(__instance, out var sprite) || Poses["Dead"] is not { } down || Normal == null)
            {
                return;
            }
            ApplyPose(__instance, sprite, down, 0f, DeadOffsetY);
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Dehya] 死亡姿态切换失败: {e}");
        }
    }

    /// <summary>临时复活(死战不屈类)走 StartReviveAnim:无条件回站立图与基准缩放/位置
    /// (死亡姿势带 1.5× 补偿与落地偏移,必须显式还原),版本+1 作废在途恢复,朝向符号保留。</summary>
    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartReviveAnim))]
    [HarmonyPostfix]
    private static void PostfixStartReviveAnim(NCreature __instance)
    {
        try
        {
            if (!IsDehyaSprite(__instance, out var sprite) || Normal == null)
            {
                return;
            }
            Versions.TryGetValue(__instance, out uint version);
            Versions[__instance] = ++version; // 作废在途的延迟恢复协程
            sprite.Texture = Normal;
            if (BaseScales.TryGetValue(__instance, out Vector2 baseScale))
            {
                float facing = CurrentFacing(sprite);
                sprite.Scale = new Vector2(baseScale.X * facing, baseScale.Y);
                sprite.Position = BasePositions.GetValueOrDefault(__instance, sprite.Position);
            }
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Dehya] 复原站立失败: {e}");
        }
    }

    private static bool IsDehyaSprite(NCreature node, out Sprite2D sprite)
    {
        sprite = null!;
        if (node.Entity.Player?.Character is not Dehya || node.HasSpineAnimation)
        {
            return false;
        }
        if (node.Visuals.GetNodeOrNull<Sprite2D>("%Visuals") is not { } found)
        {
            return false;
        }
        sprite = found;
        return true;
    }

    /// <summary>换姿势并登记版本号;返回本次版本(调用方延迟后凭版本回切)。横构图按高度比补偿。</summary>
    private static uint ApplyPose(NCreature node, Sprite2D sprite, Texture2D pose, float offsetX, float offsetY)
    {
        if (!BaseScales.TryGetValue(node, out Vector2 baseScale))
        {
            baseScale = new Vector2(Mathf.Abs(sprite.Scale.X), Mathf.Abs(sprite.Scale.Y));
            BaseScales[node] = baseScale;
        }
        if (!BasePositions.TryGetValue(node, out Vector2 basePos))
        {
            basePos = sprite.Position;
            BasePositions[node] = basePos;
        }
        Versions.TryGetValue(node, out uint version);
        uint mine = ++version;
        Versions[node] = mine;
        sprite.Texture = pose;
        float factor = pose.GetSize().Y > 0 && Normal != null ? (float)Normal.GetSize().Y / pose.GetSize().Y : 1f;
        float facing = CurrentFacing(sprite);
        sprite.Scale = new Vector2(baseScale.X * factor * facing, baseScale.Y * factor);
        // 横向偏移随朝向镜像:构图主体的左右在镜像渲染下互换。
        sprite.Position = basePos + new Vector2(offsetX * facing, offsetY);
        return mine;
    }

    private static void RestoreIfCurrent(NCreature node, Sprite2D sprite, uint mine)
    {
        if (Versions.TryGetValue(node, out uint current) && current == mine && GodotObject.IsInstanceValid(sprite))
        {
            sprite.Texture = Normal;
            if (BaseScales.TryGetValue(node, out Vector2 baseScale))
            {
                float facing = CurrentFacing(sprite);
                sprite.Scale = new Vector2(baseScale.X * facing, baseScale.Y);
                sprite.Position = BasePositions.GetValueOrDefault(node, sprite.Position);
            }
        }
    }

    /// <summary>朝向符号:+1 朝右 / -1 朝左(scale.X 符号即 SurroundedPower 维护的朝向状态,不可覆写)。</summary>
    private static float CurrentFacing(Sprite2D sprite)
    {
        return sprite.Scale.X < 0f ? -1f : 1f;
    }

    /// <summary>双攻击形态:E8 裁定 剑斩=≥1费 / 挥拳=0费;取本回合最后打出的己方卡的当前结算费用。
    /// 按本回合过滤(HappenedThisTurn,原版 RingingPower 同款):本回合尚未出牌时即为「无卡上下文」,
    /// 默认剑斩——否则非出牌来源的攻击会错误回退命中早前回合的最后一张卡(2026-10-06 审查修正)。</summary>
    private static Texture2D? ResolveAttackPose(Player player)
    {
        if (CombatManager.Instance is { IsInProgress: true } combat)
        {
            var started = combat.History.CardPlaysStarted;
            // 0.107.1 的 CardPlay 无 Player 属性,经 Card.Owner 判归属(两版通用)。
            var card = started.LastOrDefault(e => e.CardPlay.Card.Owner == player
                && e.HappenedThisTurn(player.Creature.CombatState))?.CardPlay.Card;
            if (card != null && card.EnergyCost.GetResolved() == 0)
            {
                return Poses["AttackPunch"];
            }
        }
        return Poses["AttackSlash"];
    }
}
