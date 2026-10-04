using System;
using System.Collections.Generic;
using DehyaMod.Content.Cards;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace DehyaMod.Content.Timeline;

/// <summary>
/// 第二章·水土不服(卡牌纪元,Ironclad2Epoch/Navia4Epoch 同款):解锁 炽鬃狮血/雇佣关系/炎啸狮咬。
/// 揭示条件=用迪希雅通关一次([UnlockEpochAfterWinAs]);池过滤见 DehyaCardPool.FilterThroughEpochs。
/// 基类 CardUnlockEpochTemplate 自动提供 UnlockText/QueueUnlocks(解锁展示文案读前三项,故恰好 3 件)。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(DehyaStory))]
[AutoTimelineSlot(EpochEra.Flourish0)]
public sealed class Dehya2Epoch : CardUnlockEpochTemplate
{
    /// <summary>解锁物类型(单一来源):模板 QueueUnlocks/UnlockText 与池门控共用。</summary>
    public static readonly Type[] CardUnlockTypes =
    {
        typeof(BlazingLionBlood), typeof(MercenaryContract), typeof(BlazingLionBite),
    };

    public override string Id => "STS2_DEHYA_EPOCH_2";

    public override string StoryId => "Dehya";

    /// <summary>缩略图槽覆盖(原版 epoch_atlas 图集无 mod 条目会 NOPE);大图走原版全局路径推导。</summary>
    public override EpochAssetProfile AssetProfile => new(
        PackedPortraitPath: "res://STS2-Dehya/images/timeline/sts2_dehya_epoch_2_thumb.png");

    protected override IEnumerable<Type> CardTypes => CardUnlockTypes;
}
