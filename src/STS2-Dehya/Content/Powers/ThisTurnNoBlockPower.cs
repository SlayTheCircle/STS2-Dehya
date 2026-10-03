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
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 本回合禁格挡标记(隐藏减益):卸甲强袭把格挡折算成力量后挂载,
/// 语义对齐 vanilla NoBlockPower(卡牌来源的格挡清零,Unpowered/非卡来源不受影响),
/// 差异是本类在持有者回合结束即整块移除(单回合标记)且不可见。永不可见,无本地化。
/// </summary>
[RegisterPower]
public sealed class ThisTurnNoBlockPower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != base.Owner || props.HasFlag(ValueProp.Unpowered) || cardSource == null)
        {
            return 1m;
        }
        return 0m;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(base.Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
