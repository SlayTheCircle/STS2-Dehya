using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 染血纷争效果:你每打出一张结算费用 ≥2 的牌(X 费按实际支付值),恢复等同其费用的生命。
/// 读取 EnergyCost.GetResolved()(IntimidatingHelmet 同款口径:含全部费用修正后的实付值)。
/// </summary>
[RegisterPower]
public sealed class BloodstainedStrifePower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
        {
            return;
        }
        int cost = cardPlay.Card.EnergyCost.GetResolved();
        if (cost < 2)
        {
            return;
        }
        Flash();
        await CreatureCmd.Heal(base.Owner, cost);
    }
}
