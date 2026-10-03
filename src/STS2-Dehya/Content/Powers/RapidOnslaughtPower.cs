using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 极速突击(可见 Power):每当该角色的拥有者打出1张费用为0的卡牌时,获得等量活力(原生 VigorPower)。
/// 费用口径=EnergyCost.GetResolved()(实际支付费用;X 费按捕获值,与本仓其余费用判定一致)。
/// 裁定 F1:设计文本为持续触发(美术图标佐证),非一次性直加。
/// </summary>
[RegisterPower]
public sealed class RapidOnslaughtPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.GetPlayer()?.Creature != base.Owner)
        {
            return;
        }
        if (cardPlay.Card.EnergyCost.GetResolved() != 0)
        {
            return;
        }
        Flash();
        await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
    }
}
