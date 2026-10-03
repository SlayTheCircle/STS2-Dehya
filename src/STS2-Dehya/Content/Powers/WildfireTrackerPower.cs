using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
/// </summary>
[RegisterPower]
public sealed class WildfireTrackerPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Card.EnergyCost.GetResolved() != 0)
        {
            return Task.CompletedTask;
        }
        foreach (CardModel card in PileType.Hand.GetPile(base.Owner.Player).Cards.ToList())
        {
            if (card is BlazingWildfire)
            {
                card.EnergyCost.AddThisTurn(-1);
            }
        }
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
