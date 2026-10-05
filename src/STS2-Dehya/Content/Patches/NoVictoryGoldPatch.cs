using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using DehyaMod.Content.Cards;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 「加码加价」的本场奖励金币清零:标记是写在主卡组 RaisedStakes 实例上的 [SavedProperty]
/// 旗标(2026-10-06 审查修正:原进程内静态登记表在「奖励界面退出重进」的读档路径上会失效——
/// 战斗胜利瞬间即写盘存档,奖励界面重生成时静态字典已随进程销毁,金币失而复得)。
/// 旗标随卡组序列化:读档重生成奖励同样被拦截;下场战斗 SetUpCombat 时复位(仅本场生效)。
/// 用运行时 Harmony 补丁而非 BeforeCombatRewardOffered 钩子——后者 0.111 才引入,
/// 而 RewardsSet.GenerateRewardsFor 在 0.107.1/0.111 同名同形,两目标共用一份补丁。
/// </summary>
[HarmonyPatch(typeof(RewardsSet), "GenerateRewardsFor")]
internal static class NoVictoryGoldPatch
{
    private static void Postfix(Player player, AbstractRoom room, ref List<Reward> __result)
    {
        // 限定战斗房间:旗标从打出本场战斗到下一场战斗 SetUpCombat 之间一直存活,
        // 不设此门槛会把同窗口内其他房间的奖励金币一并误伤。
        if (room is not CombatRoom || !player.Deck.Cards.OfType<RaisedStakes>().Any(static c => c.VictoryGoldSuppressed))
        {
            return;
        }
        __result.RemoveAll(static r => r is GoldReward);
    }
}

/// <summary>
/// 旗标复位:新一场战斗开始时清掉上一场的清零标记。读档回「已结束战斗」的奖励重生成路径
/// (CombatRoom.StartPreFinishedCombat)不经过 SetUpCombat,不会被误清。
/// </summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
internal static class NoVictoryGoldResetPatch
{
    public static void Postfix(CombatState state)
    {
        try
        {
            foreach (RaisedStakes card in state.Players.SelectMany(static p => p.Deck.Cards).OfType<RaisedStakes>()
                         .Where(static c => c.VictoryGoldSuppressed).ToList())
            {
                card.VictoryGoldSuppressed = false;
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[STS2-Dehya] 加码加价旗标复位失败: {e}");
        }
    }
}
