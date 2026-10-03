using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 铓辉灿漫的剑意余晖(可见增益):每回合开始时,恢复生命。
/// 治疗量 = 能力份数(Amount),重复施放按份数叠加。
/// </summary>
[RegisterPower]
public sealed class BladeBrilliancePower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner) || base.Owner.IsDead)
        {
            return;
        }
        Flash();
        await CreatureCmd.Heal(base.Owner, base.Amount);
    }
}
