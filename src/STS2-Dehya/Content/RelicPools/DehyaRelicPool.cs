using System.Linq;
using DehyaMod.Content.Timeline;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.RelicPools;

/// <summary>遗物池(TypeList 模式):成员由 [RegisterRelic(typeof(DehyaRelicPool))] 特性聚合。</summary>
public sealed class DehyaRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "ironclad";

    public override Color LabOutlineColor => new Color("E8B23A");

    // 世界线门控(IroncladRelicPool/NaviaRelicPool 同款):第三章·攀登(3 遗物)与
    // 第四章·建筑师(炽金之锅)揭示前,章内遗物不进掉落池。
    public override System.Collections.Generic.IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        var list = base.AllRelics.ToList();
        if (!unlockState.IsEpochRevealed<Dehya3Epoch>())
        {
            list.RemoveAll(r => Dehya3Epoch.RelicUnlockTypes.Any(t => ModelDb.GetId(t) == r.Id));
        }
        if (!unlockState.IsEpochRevealed<Dehya4Epoch>())
        {
            list.RemoveAll(r => Dehya4Epoch.RelicUnlockTypes.Any(t => ModelDb.GetId(t) == r.Id));
        }
        return list;
    }
}
