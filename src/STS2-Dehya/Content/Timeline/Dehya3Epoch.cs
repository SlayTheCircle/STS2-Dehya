using System;
using System.Collections.Generic;
using DehyaMod.Content.Relics;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace DehyaMod.Content.Timeline;

/// <summary>
/// 第三章·攀登(遗物纪元,RelicUnlockEpochTemplate):解锁 伤痕累累的木桩/玩具木剑/止痛剂。
/// 揭示条件=用迪希雅累计击败 3 个首领([UnlockEpochAfterBossVictories(3)],「熟练的攀登客」);
/// 池过滤见 DehyaRelicPool.GetUnlockedRelics。基类自动提供 UnlockText/QueueUnlocks(恰好 3 件)。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(DehyaStory))]
[AutoTimelineSlot(EpochEra.Flourish2)]
public sealed class Dehya3Epoch : RelicUnlockEpochTemplate
{
    /// <summary>解锁物类型(单一来源):模板 QueueUnlocks/UnlockText 与池门控共用。</summary>
    public static readonly Type[] RelicUnlockTypes =
    {
        typeof(ScarredTrainingPost), typeof(ToyWoodenSword), typeof(Painkillers),
    };

    public override string Id => "STS2_DEHYA_EPOCH_3";

    public override string StoryId => "Dehya";

    /// <summary>缩略图槽覆盖(原版 epoch_atlas 图集无 mod 条目会 NOPE);大图走原版全局路径推导。</summary>
    public override EpochAssetProfile AssetProfile => new(
        PackedPortraitPath: "res://STS2-Dehya/images/timeline/sts2_dehya_epoch_3_thumb.png");

    protected override IEnumerable<Type> RelicTypes => RelicUnlockTypes;
}
