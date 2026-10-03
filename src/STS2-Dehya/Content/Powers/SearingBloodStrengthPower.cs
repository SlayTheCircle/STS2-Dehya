using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 灼热之血的力量部分:原生「本回合临时力量」抽象的自有子类(原生范式:Flex/FlexPotionPower)。
/// 落地时同步施加等量真实力量,回合结束自动回收;标题/描述走原生临时力量词条,无需本地化。
/// </summary>
[RegisterPower]
public sealed class SearingBloodStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Cards.SearingBlood>();
}
