using System.Linq;
using DehyaMod.Content.Timeline;
using Godot;
using MegaCrit.Sts2.Core.Unlocks;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DehyaMod.Content.CardPools;

/// <summary>
/// 卡池(TypeList 模式):池成员由各卡类上的 [RegisterCard(typeof(DehyaCardPool))] 特性自动聚合,
/// 本类只负责主题属性。增删卡 = 增删卡类文件,无需改动本文件。
/// 能量图标不覆写走原版默认;自备图标后覆写 Big/TextEnergyIconPath。
/// 卡框着色:vanilla 全部卡框共用一张底图,颜色即 HSV 着色参数(h, s, v)。
/// 主题色=炽焰金(Navia 校准:红 h.025/橙 h.12/金 h.15;随能量球母版交付稿 h≈0.15 取 h.12,
/// 与 NameColor E8B23A/能量描边 8A6210 同族);游戏内目检后可微调。
/// </summary>
public sealed class DehyaCardPool : TypeListCardPoolModel
{
    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateHsvShaderMaterial(0.12f, 1.2f, 1.2f);

    public override string Title => "dehya";

    // 原版图集回退前缀(energy_<name>.tres);自定义图标经下面两个覆写路径接管主消费方。
    public override string EnergyColorName => "ironclad";

    // 金焰能量球(生图工作单 v2 ③,母版 图片/能量球.png 1254² 透明底):
    // big 256² 供战斗能量计, text 24² 供卡牌费用行内图标;均为 energy.sh 派生。
    public override string? BigEnergyIconPath => "res://STS2-Dehya/images/energy/dehya_energy_big.png";

    public override string? TextEnergyIconPath => "res://STS2-Dehya/images/energy/dehya_energy_text.png";

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

