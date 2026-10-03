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
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 咆哮之剑的战吼回响(可见增益):每回合你打出的第一张费用大于等于 2 的牌,抽牌。
/// 抽牌数 = 能力份数(Amount)。一次性标记走 RosulaEmblem 范式:回合开始重置、战斗结束清零。
/// 费用取 GetResolved 实际费用(含修正;X 费按实际支付的 X 计)。
/// </summary>
[RegisterPower]
public sealed class RoaringBladePower : DehyaPowerBase
{
    private const int MinCost = 2;

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
        if (cardPlay.Card.EnergyCost.GetResolved() < MinCost)
        {
            return;
        }
        TriggeredThisTurn = true;
        Flash();
        if (base.Owner.Player is { } player)
        {
            await CardPileCmd.Draw(choiceContext, base.Amount, player);
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
}
