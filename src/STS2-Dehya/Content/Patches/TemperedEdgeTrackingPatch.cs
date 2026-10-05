using System;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using DehyaMod.Content.Cards;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 锤砺锋芒的全局费用监听:postfix Hook.AfterCardPlayed(战斗内出牌统一收口,主卡组不收
/// AfterCardPlayed 钩子,只能全局收口)。打出费用不低于 2 的牌(按实际支付费用,X 费按捕获值)后,
/// 给主卡组与战斗牌堆中所有锤砺锋芒实例双写永久伤害增量(TheScythe/GeneticAlgorithm 范式:
/// 引擎战斗 SetUp 时 CloneCard 深拷贝,只写主卡组会让本场手牌中的克隆体纹丝不动,违背卡面
/// 「本局永久+」的即时生效承诺,2026-10-06 审查修正)。主卡组侧 SavedProperty 随存档持久,
/// 战斗克隆体下场即弃,不会重复累计。由 ModEntry 的 PatchAll 装配。
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed), new[] { typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardPlay) })]
public static class TemperedEdgeTrackingPatch
{
    public static void Postfix(CardPlay cardPlay)
    {
        try
        {
            Player? player = cardPlay?.Card?.Owner;
            if (player is null)
            {
                return;
            }
            if (cardPlay.Card.EnergyCost.GetResolved() < 2)
            {
                return;
            }
            foreach (TemperedEdge edge in player.Deck.Cards.OfType<TemperedEdge>().ToList())
            {
                edge.RegisterCostlyCardPlayed();
            }
            PlayerCombatState? piles = player.PlayerCombatState;
            if (piles != null)
            {
                foreach (TemperedEdge edge in piles.AllCards.OfType<TemperedEdge>().ToList())
                {
                    edge.RegisterCostlyCardPlayed();
                }
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[STS2-Dehya] 锤砺锋芒费用追踪失败: {e}");
        }
    }
}
