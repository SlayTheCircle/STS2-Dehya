using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 灼热之血的敏捷部分:原生「本回合临时敏捷」抽象的自有子类(原生范式:Anticipate)。
/// 落地时同步施加等量真实敏捷,回合结束自动回收;标题/描述走原生临时敏捷词条,无需本地化。
/// </summary>
[RegisterPower]
public sealed class SearingBloodDexterityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Cards.SearingBlood>();
}
