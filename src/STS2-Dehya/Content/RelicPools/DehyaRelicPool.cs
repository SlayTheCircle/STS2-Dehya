using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.RelicPools;

/// <summary>遗物池(TypeList 模式):成员由 [RegisterRelic(typeof(DehyaRelicPool))] 特性聚合。</summary>
public sealed class DehyaRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "ironclad";

    public override Color LabOutlineColor => new Color("E8B23A");
}
