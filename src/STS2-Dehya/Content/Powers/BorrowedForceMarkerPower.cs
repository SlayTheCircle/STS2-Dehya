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
/// 借力打力的窗口标记(可见,Amount=荆棘 2):窗口到下回合开始为止(含敌方回合)。
/// 期间你以任何方式实际扣减过生命(卡牌代价/毒/攻击伤害等),下回合开始时获得 Amount 点荆棘;
/// 未失血则标记作废。结算后移除本标记。
/// </summary>
[RegisterPower]
public sealed class BorrowedForceMarkerPower : DehyaPowerBase
{
    private bool _lostHp;

    private bool _resolved;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            _lostHp = true;
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (_resolved || !participants.Contains(base.Owner))
        {
            return;
        }
        _resolved = true;
        if (_lostHp)
        {
            Flash();
            await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        }
        await PowerCmd.Remove(this);
    }
}
