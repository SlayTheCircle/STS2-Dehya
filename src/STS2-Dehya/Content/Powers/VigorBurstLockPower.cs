using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 活力锁定(可见减益,本回合):持有者本回合无法抽牌、无法获得能量(分别参照原生
/// NoDrawPower / NoEnergyGainPower 的判定口径),回合结束自动移除。由「活力迸发」施加。
/// </summary>
[RegisterPower]
public sealed class VigorBurstLockPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (fromHandDraw || player != base.Owner.Player)
        {
            return true;
        }
        Flash();
        return false;
    }

    public override decimal ModifyEnergyGain(Player player, decimal amount)
    {
        if (player != base.Owner.Player)
        {
            return amount;
        }
        return 0m;
    }

    public override Task AfterModifyingEnergyGain()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
