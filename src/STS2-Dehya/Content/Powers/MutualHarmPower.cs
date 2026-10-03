using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 两败俱伤的角斗气场(可见增益):每回合开始时,使所有敌人获得易伤,你同时获得等量易伤。
/// 施放瞬间的「全体 1 易伤」由卡牌 MutualHarm 自己结算;易伤层数 = 能力份数(Amount)。
/// </summary>
[RegisterPower]
public sealed class MutualHarmPower : DehyaPowerBase
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
        PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<VulnerablePower>(choiceContext, combatState.HittableEnemies, base.Amount, base.Owner, null);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
    }
}
