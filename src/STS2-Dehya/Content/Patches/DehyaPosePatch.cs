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
/// 短暂展示对应姿势立绘后切回站立图;Dead 不接(倒下为横图且死亡另有表现,直接换图会破版)。
/// 双攻击形态(裁定 E8):剑斩=≥1 费攻击动作、挥拳=0 费攻击动作——按触发时本回合最后打出的
/// 卡的 GetResolved() 费用取形态(与燃烧心火/木桩等 0 费体系判定同口径);无卡上下文
/// (雷球等非出牌来源)时默认剑斩。剑斩母版为横构图 1536×1024,按 Normal/pose 高度比
/// 补偿缩放保持人物同高、剑势向两侧展开。补丁由 ModEntry.Init 的显式 PatchAll 挂载;纹理预加载。
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
    };

    private static readonly Texture2D? Normal = Load("dehya_normal");

    // 每个小人一个版本号:新触发使旧恢复协程失效,避免连击时旧恢复覆盖新姿势。
    private static readonly Dictionary<NCreature, uint> Versions = new();

    // 每个小人的基准缩放缓存:连击时后续触发须以入场缩放(而非上一个姿势的补偿缩放)为基数。
    private static readonly Dictionary<NCreature, Vector2> BaseScales = new();

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
            if (__instance.Entity.Player?.Character is not Dehya || __instance.HasSpineAnimation)
            {
                return;
            }
            if (__instance.Visuals.GetNodeOrNull<Sprite2D>("%Visuals") is not { } sprite)
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

            if (!BaseScales.TryGetValue(__instance, out Vector2 baseScale))
            {
                baseScale = sprite.Scale;
                BaseScales[__instance] = baseScale;
            }

            Versions.TryGetValue(__instance, out uint version);
            uint mine = ++version;
            Versions[__instance] = mine;
            sprite.Texture = pose;
            // 横构图姿势按高度比补偿,保持人物视高一致(竖构图因子恒为 1)。
            float factor = pose.GetSize().Y > 0 ? (float)Normal.GetSize().Y / pose.GetSize().Y : 1f;
            sprite.Scale = baseScale * factor;
            await Task.Delay(trigger == "Hit" ? HitHoldMs : AttackHoldMs);
            if (Versions.TryGetValue(__instance, out uint current) && current == mine && GodotObject.IsInstanceValid(sprite))
            {
                sprite.Texture = Normal;
                sprite.Scale = baseScale;
            }
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Dehya] 小人姿势切换失败({trigger}): {e}");
        }
    }

    /// <summary>双攻击形态:E8 裁定 剑斩=≥1费 / 挥拳=0费;取本回合最后打出的己方卡的当前结算费用。</summary>
    private static Texture2D? ResolveAttackPose(Player player)
    {
        if (CombatManager.Instance is { IsInProgress: true } combat)
        {
            var started = combat.History.CardPlaysStarted;
            // 0.107.1 的 CardPlay 无 Player 属性,经 Card.Owner 判归属(两版通用)。
            var card = started.LastOrDefault(e => e.CardPlay.Card.Owner == player)?.CardPlay.Card;
            if (card != null && card.EnergyCost.GetResolved() == 0)
            {
                return Poses["AttackPunch"];
            }
        }
        return Poses["AttackSlash"];
    }
}
