using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 重剑无锋(增益,Instanced,Amount=费用倍率):持有者打出费用不低于 2 的牌后,
/// 获得该牌实际费用 ×Amount 的格挡。多张重剑无锋各建独立实例、各自结算
/// (引擎 Instanced 语义,参照原版 TheBomb)。升级差异仅体现在卡面费用(2→1)。
/// </summary>
[RegisterPower]
public sealed class EdgelessGreatswordPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

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
        await CreatureCmd.GainBlock(base.Owner, cost * base.Amount, ValueProp.Move, cardPlay);
    }
}
