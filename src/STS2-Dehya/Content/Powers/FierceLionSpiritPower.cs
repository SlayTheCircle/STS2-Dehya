using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 烈狮血性的勇猛斗志(可见增益):每回合你打出的第一张费用大于等于 3 的牌,使你获得易伤并获得能量。
/// 易伤/能量数值 = 能力份数(Amount)。一次性标记走 RosulaEmblem 范式:回合开始重置、战斗结束清零。
/// 费用取 GetResolved 实际费用(含修正;X 费按实际支付的 X 计);能力牌自身为 3 费,首次打出本能力即满足条件。
/// </summary>
[RegisterPower]
public sealed class FierceLionSpiritPower : DehyaPowerBase
{
    private const int MinCost = 3;

    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool TriggeredThisTurn
    {
        get => _triggeredThisTurn;
        set
        {
            AssertMutable();
            _triggeredThisTurn = value;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (TriggeredThisTurn || cardPlay.GetPlayer().Creature != base.Owner)
        {
            return;
        }
        if (!CostsAtLeast(cardPlay.Card))
        {
            return;
        }
        TriggeredThisTurn = true;
        Flash();
        await PowerCmd.Apply<VulnerablePower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        if (base.Owner.Player is { } player)
        {
            await PlayerCmd.GainEnergy(base.Amount, player);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            TriggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        TriggeredThisTurn = false;
        return Task.CompletedTask;
    }

    private static bool CostsAtLeast(CardModel card)
    {
        return card.EnergyCost.GetResolved() >= MinCost;
    }
}
