using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.Cards;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 燎原野火的手牌费用追踪(隐藏):持有者打出 0 费牌后,扫描其手牌中的燎原野火,
/// 每张费用临时 -1(可叠加)。降费用走 CardEnergyCost.AddThisTurn(-1),
/// 回合结束复原由引擎的 EndOfTurnCleanup 自动完成;本能力随后自清。
/// 0 费口径=实付能量快照 cardPlay.Resources.EnergySpent(2026-10-06 审查修正,缘由见 RapidOnslaughtPower)。
/// </summary>
[RegisterPower]
public sealed class WildfireTrackerPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Resources.EnergySpent != 0)
        {
            return Task.CompletedTask;
        }
        // Mirror 裁定 B1/B3(2026-10-04):此牌在任意牌堆都计入;修正持续到回合结束或打出后(AddThisTurnOrUntilPlayed)。
        PlayerCombatState? piles = base.Owner.Player?.PlayerCombatState;
        if (piles is null)
        {
            return Task.CompletedTask;
        }
        foreach (CardModel card in piles.Hand.Cards.Concat(piles.DrawPile.Cards).Concat(piles.DiscardPile.Cards).ToList())
        {
            if (card is BlazingWildfire)
            {
                card.EnergyCost.AddThisTurnOrUntilPlayed(-1);
            }
        }
        return Task.CompletedTask;
    }

}
