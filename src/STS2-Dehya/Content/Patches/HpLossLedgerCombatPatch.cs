using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 失血台账挂载:HpLossLedgerPower 是「战斗内常驻计数器」,没有原版生命周期入口
/// (不是遗物/能力),只能 Harmony 收口——在 CombatManager.SetUpCombat 后为每位
/// 迪希雅玩家静默挂上(幂等,已有则跳过)。由 ModEntry.Init 的显式 PatchAll 装配。
/// 注意:postfix 必须是 void/async void(带 Task 返回值会被 Harmony 判为透传 postfix
/// 直接抛 HarmonyException,见夏洛蒂仓 2026-10-02 冒烟实测);延续在游戏主循环恢复,
/// 异常就地捕获记日志。
/// </summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
internal static class HpLossLedgerCombatPatch
{
    public static async void Postfix(CombatState state)
    {
        try
        {
            foreach (Player player in state.Players)
            {
                if (player.Character is not Characters.Dehya)
                {
                    continue;
                }
                Creature creature = player.Creature;
                if (creature == null || creature.HasPower<HpLossLedgerPower>())
                {
                    continue;
                }
                // Apply 的 amount 为 0 会被引擎直接吞掉,先用 1 落槽再静默归零。
                HpLossLedgerPower? ledger = await PowerCmd.Apply<HpLossLedgerPower>(
                    new ThrowingPlayerChoiceContext(), creature, 1m, null, null, silent: true);
                ledger?.SetAmount(0, silent: true);
                // Mirror 裁定 B1/B3(2026-10-04):费用追踪器同样常驻(燎原野火/剑斗技巧的降费
                // 在打出前就要生效,由卡牌自挂为时已晚),与台账同点挂载。
                if (!creature.HasPower<WildfireTrackerPower>())
                {
                    await PowerCmd.Apply<WildfireTrackerPower>(new ThrowingPlayerChoiceContext(), creature, 1m, null, null, silent: true);
                }
                if (!creature.HasPower<AttackPlayCostTrackerPower>())
                {
                    await PowerCmd.Apply<AttackPlayCostTrackerPower>(new ThrowingPlayerChoiceContext(), creature, 1m, null, null, silent: true);
                }
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[STS2-Dehya] 失血台账挂载失败: {e}");
        }
    }
}
