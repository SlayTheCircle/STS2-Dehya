using System;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 斩铁断金的下一张攻击降费(隐藏,Amount=减费幅度):持有者手牌/出牌区的攻击牌费用 -Amount,
/// 实际打出一张攻击牌后移除(参照原版 FreeAttackPower:持有时全手牌攻击都显示降价,
/// 首个被打出者消耗掉)。
/// </summary>
[RegisterPower]
public sealed class NextAttackDiscountPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != base.Owner)
        {
            return false;
        }
        if (card.Type != CardType.Attack)
        {
            return false;
        }
        bool inPlayablePile = card.Pile?.Type switch
        {
            PileType.Hand => true,
            PileType.Play => true,
            _ => false,
        };
        if (!inPlayablePile)
        {
            return false;
        }
        modifiedCost = Math.Max(0m, originalCost - base.Amount);
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }
        bool inPlayablePile = cardPlay.Card.Pile?.Type switch
        {
            PileType.Hand => true,
            PileType.Play => true,
            _ => false,
        };
        if (inPlayablePile)
        {
            await PowerCmd.Remove(this);
        }
    }
}
