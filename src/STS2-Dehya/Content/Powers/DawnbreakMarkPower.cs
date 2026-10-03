using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 夜尽天明的标记(减益,挂在被标记敌人身上):任意玩家打出 0 费牌时,
/// 被标记的敌人失去 Amount 点生命(不可格挡的 HP 损失,裁定:「你打出 0 费牌」按任意 0 费出牌计)。
/// 标记持续至战斗结束。
/// </summary>
[RegisterPower]
public sealed class DawnbreakMarkPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner || cardPlay.Card.EnergyCost.GetResolved() != 0)
        {
            return;
        }
        Flash();
        await DehyaDamageCmd.Damage(choiceContext, base.Owner, base.Amount, DamageProps.cardHpLoss, cardPlay.Card.Owner.Creature, null, cardPlay);
    }
}
