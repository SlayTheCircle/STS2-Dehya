using System;
using System.Collections.Generic;
using System.Linq;
using DehyaMod.Content.Potions;
using DehyaMod.Content.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace DehyaMod.Content.Timeline;

/// <summary>
/// 第四章·建筑师(混合纪元,自定义 ModEpochTemplate):解锁 甘甜清泉/佣兵酒壶(药水)+ 炽金之锅(遗物)。
/// Mirror 裁定 D1+D2:第四章第三件解锁物=炽金之锅,同时解决其获取途径(noncard-03 缺口闭合)。
/// 混合类型不适用三件单一来源模板(CreateXUnlockText 恒读前三件),故自管 QueueUnlocks
/// (药水组+遗物组分两次入队)与 UnlockText(loc 键 STS2_DEHYA_EPOCH_4.unlockText,带稀有度颜色);
/// 池过滤见 DehyaPotionPool.GetUnlockedPotions 与 DehyaRelicPool.GetUnlockedRelics。
/// 揭示条件=用迪希雅通关进阶 1([UnlockEpochAfterAscensionOneWin],vanilla 第七章同型)。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(DehyaStory))]
[AutoTimelineSlot(EpochEra.Invitation6)]
public sealed class Dehya4Epoch : ModEpochTemplate
{
    /// <summary>解锁物类型(单一来源):本章门控与池过滤共用。</summary>
    public static readonly Type[] PotionUnlockTypes =
    {
        typeof(SweetSpring), typeof(MercenaryFlask),
    };

    public static readonly Type[] RelicUnlockTypes =
    {
        typeof(GildedCookpot),
    };

    public override string Id => "STS2_DEHYA_EPOCH_4";

    public override string StoryId => "Dehya";

    /// <summary>缩略图槽覆盖(原版 epoch_atlas 图集无 mod 条目会 NOPE);大图走原版全局路径推导。</summary>
    public override EpochAssetProfile AssetProfile => new(
        PackedPortraitPath: "res://STS2-Dehya/images/timeline/sts2_dehya_epoch_4_thumb.png");

    public IReadOnlyList<PotionModel> Potions => PotionUnlockTypes
        .Select(t => ModelDb.GetById<PotionModel>(ModelDb.GetId(t))).ToList();

    public IReadOnlyList<RelicModel> Relics => RelicUnlockTypes
        .Select(t => ModelDb.GetById<RelicModel>(ModelDb.GetId(t))).ToList();

    public override void QueueUnlocks()
    {
        NTimelineScreen.Instance.QueuePotionUnlock(Potions.ToList());
        NTimelineScreen.Instance.QueueRelicUnlock(Relics.ToList());
    }
}
