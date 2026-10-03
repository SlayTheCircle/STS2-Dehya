using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 燃血成灰效果:己方回合内你每回合首次失去生命时,从抽牌堆选择 1 张牌消耗,
/// 并恢复 Amount 生命(抽牌堆为空时只回血)。观察口径同 Rupture(仅己方回合)。
/// </summary>
[RegisterPower]
public sealed class BloodToAshesPower : DehyaPowerBase
{
    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            _triggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || _triggeredThisTurn || result.UnblockedDamage <= 0 || base.CombatState.CurrentSide != base.Owner.Side)
        {
            return;
        }
        _triggeredThisTurn = true;
        Flash();
        CardPile drawPile = PileType.Draw.GetPile(base.Owner.Player);
        if (!drawPile.IsEmpty)
        {
            CardModel? selected = (await CardSelectCmd.FromCombatPile(
                choiceContext, drawPile, base.Owner.Player, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1))).FirstOrDefault();
            if (selected != null)
            {
                await CardCmd.Exhaust(choiceContext, selected);
            }
        }
        await CreatureCmd.Heal(base.Owner, base.Amount);
    }
}
