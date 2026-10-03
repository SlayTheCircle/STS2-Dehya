using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 燃烧心火(增益):你打出的 0 费攻击牌伤害 +Amount(按实际支付费用判定,含被打折到 0 的牌)。
/// 升级差异由卡面升级数值体现(+3→+5)。
/// </summary>
[RegisterPower]
public sealed class BurningHeartPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override decimal ModifyDamageAdditiveCore(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? card)
    {
        if (dealer != base.Owner || card is null || card.Type != CardType.Attack)
        {
            return 0m;
        }
        if (!props.IsPoweredAttack())
        {
            return 0m;
        }
        if (card.EnergyCost.GetResolved() != 0)
        {
            return 0m;
        }
        return base.Amount;
    }
}
