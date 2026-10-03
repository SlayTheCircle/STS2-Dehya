using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 包扎伤口的窗口标记(可见,Amount=每层伤害 2):窗口持续到下回合结束(跨一个己方回合结束)。
/// 窗口内你首次失去生命时结算:先读后耗——读取全部再生层数→移除全部再生→
/// 对所有敌人造成 Amount×层数 伤害,然后移除本标记;到时未触发则标记自然消散。
/// </summary>
[RegisterPower]
public sealed class BandageWoundsMarkerPower : DehyaPowerBase
{
    private bool _resolved;

    private bool _passedOneTurnEnd;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || _resolved || result.UnblockedDamage <= 0)
        {
            return;
        }
        _resolved = true;
        int regenStacks = base.Owner.GetPowerAmount<RegenPower>();
        if (regenStacks > 0)
        {
            Flash();
            await PowerCmd.Remove<RegenPower>(base.Owner);
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.Amount * regenStacks, ValueProp.Unpowered, base.Owner);
        }
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        if (_passedOneTurnEnd)
        {
            await PowerCmd.Remove(this);
            return;
        }
        _passedOneTurnEnd = true;
    }
}
