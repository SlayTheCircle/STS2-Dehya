using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 奖励清零标记(可见,裁定 J1/J2):「加码加价」打出时挂到本方玩家身上,仅作本场战斗的在场标记
/// (战内可经 HasPower 查询);实际拦截由 NoVictoryGoldPatch 按登记表执行,Power 本身无行为。
/// </summary>
[RegisterPower]
public sealed class NoVictoryGoldPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

}
