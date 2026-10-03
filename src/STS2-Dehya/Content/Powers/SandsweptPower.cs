using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 纵横沙海的沙暴光环(可见增益):回合结束时,若你的格挡至少为门槛值,对所有敌人造成伤害并获得再生。
/// 门槛/伤害/再生三个数值由卡牌 Sandswept 在施放时经 <see cref="Configure"/> 注入(The Bomb 的 SetDamage 范式),
/// 升级后的数值随再次施放刷新;回合结束走 BeforeSideTurnEnd(伤害敌人在此钩子,先于再生结算)。
/// </summary>
[RegisterPower]
public sealed class SandsweptPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Threshold", 10m),
        new DynamicVar("AoeDamage", 4m),
        new PowerVar<RegenPower>(2m),
    };

    /// <summary>卡牌施放时注入实际数值;重复施放时以最后打出的数值为准。</summary>
    internal void Configure(int threshold, decimal aoeDamage, decimal regen)
    {
        AssertMutable();
        DynamicVars["Threshold"].BaseValue = threshold;
        DynamicVars["AoeDamage"].BaseValue = aoeDamage;
        DynamicVars["RegenPower"].BaseValue = regen;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner) || base.Owner.IsDead)
        {
            return;
        }
        if (base.Owner.Block < DynamicVars["Threshold"].IntValue)
        {
            return;
        }
        Flash();
        await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, DynamicVars["AoeDamage"].BaseValue, ValueProp.Unpowered, base.Owner);
        await PowerCmd.Apply<RegenPower>(choiceContext, base.Owner, DynamicVars["RegenPower"].BaseValue, base.Owner, null);
    }
}
