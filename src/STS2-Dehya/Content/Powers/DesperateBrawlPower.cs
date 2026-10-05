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
/// (含其间敌方回合与本次回合开始效果)你实际扣减过生命,本回合获得 Amount 点临时力量(回合末自动回收)。
/// 结算挂 AfterSideTurnStart 而非 BeforeSideTurnStart(2026-10-06 审查修正):Before 侧的
/// 结算结果取决于能力挂载顺序(灼热形态烧血先结算则计入本窗口、后结算则计入下一窗口,
/// 同语义局面结果相反);After 侧在全部 Before 回合开始效果完成后统一结算,顺序无关。
/// 施加力量不弹 UI,无需选择上下文(Throwing 足够)。
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

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
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
            await PowerCmd.Apply<DesperateBrawlStrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, base.Owner, null);
        }
    }
}
