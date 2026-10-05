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

    /// <summary>
    /// 挂在 BeforeSideTurnStart 而非 AfterSideTurnStart:F8 事故(2026-10-05 游戏内)——后者无上下文,
    /// 自造 ThrowingPlayerChoiceContext 传入 Damage 管线会让燃血成灰的选牌 UI 抛 NotImplementedException、
    /// 回合计程死亡;Before 侧引擎原生构造 HookPlayerChoiceContext(原版 ForegoneConclusion 同族),
    /// 下游玩家选择经游戏动作队列合法同步。
    /// </summary>
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner) || base.Owner.IsDead)
        {
            return;
        }
        Flash();
        await CreatureCmd.Damage(choiceContext, base.Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner, 1m, base.Owner, null);
        await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, DynamicVars["AoeDamage"].BaseValue, ValueProp.Unpowered, base.Owner);
    }
}
