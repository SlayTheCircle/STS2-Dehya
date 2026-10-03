using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 灼眼之鬃(增益,标记位):持有者的荆棘对敌人造成伤害时,由 Content/Patches/ThornsEchoPatch
/// 对该敌人重复一次等值伤害。本能力自身不持有触发逻辑,仅作为补丁的存在判据。
/// </summary>
[RegisterPower]
public sealed class DazzlingManePower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
