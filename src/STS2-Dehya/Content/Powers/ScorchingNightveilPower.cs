using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 焰灼夜幕效果:每当你在自己的回合中失去一次生命,获得 Amount 点荆棘(原版 Rupture 的观察口径:
/// AfterDamageReceived + 当前行动方为己方,卡牌代价/毒伤等一切实际扣血都算,按「次」触发;
/// 能力整场常驻,触发窗口仅己方回合——文案校准 2026-10-06,避免「回合内」被误读为本回合限时)。
/// </summary>
[RegisterPower]
public sealed class ScorchingNightveilPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || result.UnblockedDamage <= 0 || base.CombatState.CurrentSide != base.Owner.Side)
        {
            return;
        }
        Flash();
        await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
    }
}
