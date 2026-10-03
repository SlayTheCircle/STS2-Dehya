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
/// 剑斗技巧的攻击出牌费用追踪(隐藏):持有者本回合每打出一张攻击牌,
/// 扫描其手牌中的剑斗技巧,每张费用临时 -1(下限 0)。
/// 降费用走 CardEnergyCost.AddThisTurn(-1),回合结束复原由引擎 EndOfTurnCleanup 自动完成;
/// 本能力随后自清。
/// </summary>
[RegisterPower]
public sealed class AttackPlayCostTrackerPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Card.Type != CardType.Attack)
        {
            return Task.CompletedTask;
        }
        foreach (CardModel card in PileType.Hand.GetPile(base.Owner.Player).Cards.ToList())
        {
            if (card is DuelingTechnique)
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
