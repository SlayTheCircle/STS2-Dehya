using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 失血台账(可见,裁定 J1/J2):统计本回合(己方回合)玩家失去生命的「次数」,供血溅沙场等卡读取。
/// 每次实际扣减生命的伤害事件计 1 次(多段攻击按段计),己方回合开始时归零。
/// 由 HpLossLedgerCombatPatch 在每场战斗SetUp时挂到迪希雅玩家身上,全程不可见、无需本地化。
/// </summary>
[RegisterPower]
public sealed class HpLossLedgerPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            base.SetAmount(0, silent: true);
        }
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            base.SetAmount(base.Amount + 1, silent: true);
        }
        return Task.CompletedTask;
    }
}
