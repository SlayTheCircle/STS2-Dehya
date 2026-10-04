using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Timeline.Scaffolding;

namespace DehyaMod.Content.Timeline;

/// <summary>
/// 第一章·委托(杂项纪元,DarvEpoch/Navia1Epoch 同款):炽鬃之狮揭下高塔委托,向尖塔进发。
/// 角色不锁(characters.unlockText「安装后即可游玩」,Navia 同型裁定):本章仅作剧情节点,
/// 解锁文案「迪希雅成为一名可玩角色」为仪式性展示;揭示条件=完成一局迪希雅
/// (Dehya 类上的 [UnlockEpochAfterRunAs],「使用迪希雅完成一次攀登」与设计叙事一致)。
/// </summary>
[RegisterEpoch]
[RegisterStoryEpoch(typeof(DehyaStory))]
[AutoTimelineSlot(EpochEra.Blight0)]
public sealed class Dehya1Epoch : ModEpochTemplate
{
    public override string Id => "STS2_DEHYA_EPOCH_1";

    public override string StoryId => "Dehya";

    /// <summary>缩略图槽覆盖:原版缩略图走 epoch_atlas 图集,mod 无条目会 NOPE;
    /// 大图留空走原版 timeline/epoch_portraits 全局路径推导(stories.sh 已供图)。</summary>
    public override EpochAssetProfile AssetProfile => new(
        PackedPortraitPath: "res://STS2-Dehya/images/timeline/sts2_dehya_epoch_1_thumb.png");

    public override void QueueUnlocks()
    {
        LocString locString = new LocString("epochs", Id + ".unlock");
        NTimelineScreen.Instance.QueueMiscUnlock(locString.GetFormattedText() ?? "");
    }
}
