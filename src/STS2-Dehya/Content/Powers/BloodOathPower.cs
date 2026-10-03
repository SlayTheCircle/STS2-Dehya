using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 血之誓约的搏命意志(可见增益):每回合开始时,若你的生命值低于 50%,抽牌并获得能量。
/// 抽牌数与能量数 = 能力份数(Amount)。「低于 50%」按当前生命*2 严格小于最大生命判定。
/// </summary>
[RegisterPower]
public sealed class BloodOathPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner) || base.Owner.IsDead)
        {
            return;
        }
        if ((decimal)base.Owner.CurrentHp * 2m >= (decimal)base.Owner.MaxHp)
        {
            return;
        }
        Flash();
        if (base.Owner.Player is { } player)
        {
            PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();
            await CardPileCmd.Draw(choiceContext, base.Amount, player);
            await PlayerCmd.GainEnergy(base.Amount, player);
        }
    }
}
