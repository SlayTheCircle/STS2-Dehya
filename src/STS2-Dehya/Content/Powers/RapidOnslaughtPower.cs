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
/// 费用口径=实付能量快照 cardPlay.Resources.EnergySpent(原版 OneForAllPower 范式;X 费 0 能量打出
/// 仍计 0 费,与旧 GetResolved 口径一致):活查询会被降费能力在 BeforeCardPlayed 的自移除波及
/// (斩铁断金/原版 Unrelenting 打出的 0 费牌读到未降费原价),2026-10-06 审查修正。
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
        if (cardPlay.Resources.EnergySpent != 0)
        {
            return;
        }
        Flash();
        await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
    }
}
