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
/// 灼热形态的余烬领域(可见增益):每回合开始时,失去 1 点生命、获得 1 层荆棘,并对所有敌人造成伤害。
/// AOE 伤害由卡牌 ScorchingForm 施放时经 <see cref="Configure"/> 注入;入场覆甲由卡牌直接施加,不经过本 Power。
/// </summary>
[RegisterPower]
public sealed class ScorchingFormPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("AoeDamage", 9m),
    };

    /// <summary>卡牌施放时注入实际数值;重复施放时以最后打出的数值为准。</summary>
    internal void Configure(decimal aoeDamage)
    {
        AssertMutable();
        DynamicVars["AoeDamage"].BaseValue = aoeDamage;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner) || base.Owner.IsDead)
        {
            return;
        }
        Flash();
        PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();
        await CreatureCmd.Damage(choiceContext, base.Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner, 1m, base.Owner, null);
        await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, DynamicVars["AoeDamage"].BaseValue, ValueProp.Unpowered, base.Owner);
    }
}
