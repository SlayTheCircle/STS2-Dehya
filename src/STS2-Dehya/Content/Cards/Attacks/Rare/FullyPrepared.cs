using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 准备万全(稀有攻击,对全体敌人):先读取当前覆甲层数(先算后耗)——有覆甲时,移除全部覆甲
/// 并对 ALL 敌人造成等量伤害(三件套乘子=当前覆甲层数,base 0/每层 1);没有覆甲时改为获得 2 层覆甲。
/// 升级:无覆甲时的覆甲获取 2→4。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class FullyPrepared : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(0m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            static (card, _) => card.Owner?.Creature?.GetPowerAmount<PlatingPower>() ?? 0),
        new PowerVar<PlatingPower>(2m),
    };

    public FullyPrepared()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.Owner.Creature.GetPowerAmount<PlatingPower>() > 0)
        {
            // 先按当前覆甲求值(AOE 无单一目标,Calculate 传 null),再整体移除覆甲,最后结算伤害。
            decimal amount = base.DynamicVars.CalculatedDamage.Calculate(null);
            await PowerCmd.Remove<PlatingPower>(base.Owner.Creature);
            await DamageCmd.Attack(amount).FromCard(this, cardPlay)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, base.DynamicVars["PlatingPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PlatingPower"].UpgradeValueBy(2m);
    }
}
