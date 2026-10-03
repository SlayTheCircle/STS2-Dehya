using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 剑斗技巧授予的临时力量:原生 TemporaryStrengthPower 的具名子类
/// (原生类为抽象类,每个授予者建带自己标记的子类)。
/// 施加即同步真实力量,回合结束自动移除;文案走原版「临时力量」词条,无需本地化。
/// </summary>
[RegisterPower]
public sealed class DuelingTechniqueStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Cards.DuelingTechnique>();
}
