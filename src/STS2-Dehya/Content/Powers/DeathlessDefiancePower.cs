using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 死战不屈效果:失去生命的效果(卡牌代价/毒/最大生命损失等一切带 Unblockable 标记的 HP 损失)
/// 无法把你打到 0——钳制到「当前生命-1」(BeatingRemnant 的 ModifyHpLostAfterOsty 范式)。
/// 攻击伤害是可格挡的 Move 伤害、不带 Unblockable,不在此钳制口径内,仍可致死。
/// </summary>
[RegisterPower]
public sealed class DeathlessDefiancePower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != base.Owner || amount <= 0m || !props.HasFlag(ValueProp.Unblockable))
        {
            return amount;
        }
        decimal floor = Math.Max(target.CurrentHp - 1m, 0m);
        return Math.Min(amount, floor);
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }
}
