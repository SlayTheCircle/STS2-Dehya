using System.Linq;
using DehyaMod.Content.Timeline;
using Godot;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.CardPools;

/// <summary>
/// 卡池(TypeList 模式):池成员由各卡类上的 [RegisterCard(typeof(DehyaCardPool))] 特性自动聚合,
/// 本类只负责主题属性。增删卡 = 增删卡类文件,无需改动本文件。
/// 能量图标不覆写走原版默认;自备图标后覆写 Big/TextEnergyIconPath。
/// 卡框着色:vanilla 全部卡框共用一张底图,颜色即 HSV 着色参数(h, s, v)。
/// </summary>
public sealed class DehyaCardPool : TypeListCardPoolModel
{
    private static readonly Material? _poolFrameMaterial = null;

    public override string Title => "dehya";

    public override string EnergyColorName => "ironclad";

    public override Material? PoolFrameMaterial => _poolFrameMaterial;

    public override Color DeckEntryCardColor => new Color("E8B23A");

    public override Color EnergyOutlineColor => new Color("8A6210");

    public override bool IsColorless => false;

    // 世界线门控(RegentCardPool/NaviaCardPool 同款):第二章·水土不服揭示前,章内 3 卡不进奖励/商店池。
    protected override System.Collections.Generic.IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> FilterThroughEpochs(
        UnlockState unlockState,
        System.Collections.Generic.IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> cards)
    {
        var list = cards.ToList();
        if (!unlockState.IsEpochRevealed<Dehya2Epoch>())
        {
            list.RemoveAll(c => Dehya2Epoch.CardUnlockTypes.Any(t => MegaCrit.Sts2.Core.Models.ModelDb.GetId(t) == c.Id));
        }
        return list;
    }
}

