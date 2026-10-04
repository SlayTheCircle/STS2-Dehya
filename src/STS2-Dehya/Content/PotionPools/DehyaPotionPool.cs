using System.Linq;
using DehyaMod.Content.Timeline;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.PotionPools;

/// <summary>药水池(TypeList 模式):成员由 [RegisterPotion(typeof(DehyaPotionPool))] 特性聚合。</summary>
public sealed class DehyaPotionPool : TypeListPotionPoolModel
{
    public override string EnergyColorName => "ironclad";

    public override Color LabOutlineColor => new Color("E8B23A");

    // 世界线门控(RegentPotionPool/NaviaPotionPool 同款):第四章·建筑师揭示前,章内 2 药水不进奖励池。
    public override System.Collections.Generic.IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)
    {
        var list = base.AllPotions.ToList();
        if (!unlockState.IsEpochRevealed<Dehya4Epoch>())
        {
            list.RemoveAll(p => Dehya4Epoch.PotionUnlockTypes.Any(t => ModelDb.GetId(t) == p.Id));
        }
        return list;
    }
}
