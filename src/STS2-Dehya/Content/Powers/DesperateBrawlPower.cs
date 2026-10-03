using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 殊死搏斗效果(Amount=临时力量值):己方回合开始时,若自上个己方回合开始以来
/// (含其间敌方回合)你实际扣减过生命,本回合获得 Amount 点临时力量(回合末自动回收)。
/// </summary>
[RegisterPower]
public sealed class DesperateBrawlPower : DehyaPowerBase
{
    private bool _lostHpSinceLastTurnStart;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            _lostHpSinceLastTurnStart = true;
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        bool lostSinceLastTurn = _lostHpSinceLastTurnStart;
        _lostHpSinceLastTurnStart = false;
        if (lostSinceLastTurn)
        {
            Flash();
            await PowerCmd.Apply<DesperateBrawlStrengthPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        }
    }
}
