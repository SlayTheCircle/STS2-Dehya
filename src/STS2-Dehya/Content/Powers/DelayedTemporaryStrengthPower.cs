using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 剑斗技巧的延迟临时力量(增益):持有者所在一方回合开始时,给予 Amount 点临时力量
/// (DuelingTechniqueStrengthPower,回合结束自动回落),随后移除自身(参照 DelayedFocus 范式)。
/// </summary>
[RegisterPower]
public sealed class DelayedTemporaryStrengthPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<DuelingTechniqueStrengthPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
    }
}
