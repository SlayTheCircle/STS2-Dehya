using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.Compat;
using DehyaMod.Content.RelicPools;

namespace DehyaMod.Content.Relics;

/// <summary>
/// 伤痕累累的木桩(罕见遗物):你打出的 0 费攻击牌伤害 +3。
/// 走遗物 ModifyDamageAdditive 直改(StrikeDummy 范式),不需要 Power;
/// 「0 费」按 GetResolved 实际费用判定(被降费到 0 的也算,X 费按实际支付的 X 计),多段攻击每段各 +3。
/// </summary>
[RegisterRelic(typeof(DehyaRelicPool))]
public sealed class ScarredTrainingPost : DehyaDamageRelicBase
{
    private const string _extraDamageKey = "ExtraDamage";

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DynamicVar(_extraDamageKey, 3m) };

    protected override decimal ModifyDamageAdditiveCore(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack())
        {
            return 0m;
        }
        if (cardSource == null || cardSource.Type != CardType.Attack)
        {
            return 0m;
        }
        if (dealer != base.Owner.Creature && cardSource.Owner != base.Owner)
        {
            return 0m;
        }
        if (cardSource.EnergyCost.GetResolved() != 0)
        {
            return 0m;
        }
        return base.DynamicVars[_extraDamageKey].BaseValue;
    }
}
