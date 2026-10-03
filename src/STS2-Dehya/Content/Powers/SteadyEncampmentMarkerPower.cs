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
/// 步步为营的窗口标记(可见,Amount=再生层数 3):窗口到下回合开始为止。
/// 期间若你因攻击伤害(可格挡的 Move 伤害,即不带 Unblockable 标记)实际扣减过生命,
/// 标记作废;否则下回合开始时获得 Amount 层再生,随后移除本标记。
/// </summary>
[RegisterPower]
public sealed class SteadyEncampmentMarkerPower : DehyaPowerBase
{
    private bool _lostHpFromAttack;

    private bool _resolved;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && result.UnblockedDamage > 0 && !props.HasFlag(ValueProp.Unblockable))
        {
            _lostHpFromAttack = true;
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
        if (!_lostHpFromAttack)
        {
            Flash();
            await PowerCmd.Apply<RegenPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        }
        await PowerCmd.Remove(this);
    }
}
