using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 殊死搏斗授予的「本回合临时力量」:原生抽象的自有子类,由殊死搏斗能力在回合开始结算时施加。
/// 回合结束自动回收;标题/描述走原生临时力量词条,无需本地化。
/// </summary>
[RegisterPower]
public sealed class DesperateBrawlStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Cards.DesperateBrawl>();
}
