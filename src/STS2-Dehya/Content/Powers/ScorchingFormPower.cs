using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 灼热形态的余烬领域(可见增益):每回合开始时(能量重置后、抽牌前),失去 1 点生命、获得 1 层荆棘,
/// 并对所有敌人造成伤害。AOE 伤害由卡牌 ScorchingForm 施放时经 <see cref="Configure"/> 注入;
/// 入场覆甲由卡牌直接施加,不经过本 Power。
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
    /// 挂在 BeforeHandDraw(抽牌管线钩子,原版 ForegoneConclusion/InfiniteBlades 同款)而非回合开始钩子——
    /// 两代事故记录:F8(2026-10-05)AfterSideTurnStart 无上下文,自造 Throwing 传入 Damage 管线让选牌 UI
    /// 抛 NotImplementedException 杀死回合计程;F8 二次(2026-10-06)BeforeSideTurnStart 虽有引擎上下文,
    /// 但钩子内的玩家选择会把自身 GameAction 挂起出队(ActionQueueSet.PauseActionForPlayerChoice 只暂停
    /// 单个 action),抽牌 action 立即执行——燃血成灰的选牌框开着看五张牌被逐一抽走/洗牌回流,确认时
    /// 选中牌可能已进手牌被误烧。BeforeHandDraw 在能量重置后、抽牌前派发(CombatManager.SetupPlayerTurn),
    /// 选牌挂起的是 SetupPlayerTurn 任务本身,弹框时抽牌堆一张未动,选完才抽,时序天然正确。
    /// </summary>
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != base.Owner.Player || base.Owner.IsDead)
        {
            return;
        }
        Flash();
        await CreatureCmd.Damage(choiceContext, base.Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner, 1m, base.Owner, null);
        await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, DynamicVars["AoeDamage"].BaseValue, ValueProp.Unpowered, base.Owner);
    }
}
