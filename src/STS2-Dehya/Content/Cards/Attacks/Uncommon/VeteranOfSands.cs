using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 黄沙百战(罕见攻击):造成 10 点伤害,弃牌堆中每有一张费用不低于 2 的牌(按实际费用计,X 费不计),
/// 伤害额外 +5。预览感知三件套(CalculationBase+ExtraDamage+CalculatedDamage,乘子扫弃牌堆,
/// vanilla PerfectedStrike 范式);升级:基础伤害 10→11、每份加成 +5→+7(Damage 与 CalculationBase 同步)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class VeteranOfSands : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new CalculationBaseVar(10m),
        new ExtraDamageVar(5m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            static (card, _) => card.Owner?.PlayerCombatState?.DiscardPile.Cards.Count(
                static c => !c.EnergyCost.CostsX && c.EnergyCost.GetWithModifiers(CostModifiers.All) >= 2) ?? 0),
    };

    public VeteranOfSands()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal amount = base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(amount).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars.CalculationBase.UpgradeValueBy(1m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}
