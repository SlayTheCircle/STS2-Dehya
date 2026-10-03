using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.PotionPools;

/// <summary>药水池(TypeList 模式):成员由 [RegisterPotion(typeof(DehyaPotionPool))] 特性聚合。</summary>
public sealed class DehyaPotionPool : TypeListPotionPoolModel
{
    public override string EnergyColorName => "ironclad";

    public override Color LabOutlineColor => new Color("E8B23A");
}
