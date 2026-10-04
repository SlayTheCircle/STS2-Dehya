using System;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using DehyaMod.Content.Characters;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 给 3 个原版事件追加迪希雅专属选项(设计案:特殊事件扩充;docs/dev/events.md 补丁通道)。
/// 注入点:EventModel.GenerateInitialOptionsWrapper 基方法后缀——三个目标事件均未覆写它(同 Navia 实证口径,
/// 游戏升级时需重新核对)。仅当事件归属者是迪希雅时追加。
/// 补丁由 ModEntry.Init 的显式 Harmony.PatchAll 装配(本仓 ModInitializer 语义)。
/// SetEventFinished 是 protected,外部 handler 经缓存的反射委托调用(每次启动仅反射一次)。
/// </summary>
[HarmonyPatch(typeof(EventModel), "GenerateInitialOptionsWrapper")]
internal static class VanillaEventDehyaOptions
{
    private static readonly Action<EventModel, LocString>? FinishDelegate = CreateFinishDelegate();

    private static Action<EventModel, LocString>? CreateFinishDelegate()
    {
        MethodInfo? method = typeof(EventModel).GetMethod("SetEventFinished",
            BindingFlags.Instance | BindingFlags.NonPublic, new[] { typeof(LocString) });
        if (method == null)
        {
            Log.Error("[STS2-Dehya] 找不到 EventModel.SetEventFinished——游戏版本不兼容,原版事件扩充将不可用");
            return null;
        }
        return (Action<EventModel, LocString>)Delegate.CreateDelegate(typeof(Action<EventModel, LocString>), method);
    }

    [HarmonyPostfix]
    private static void AppendDehyaOptions(EventModel __instance, ref IReadOnlyList<EventOption> __result)
    {
        try
        {
            if (FinishDelegate == null || __instance.Owner == null || __instance.Owner.Character is not Dehya)
            {
                return;
            }
            EventOption? extra = __instance switch
            {
                TrashHeap => new EventOption(__instance, () => TossSomethingAsync(__instance),
                    "TRASH_HEAP.pages.INITIAL.options.DEHYA_TOSS"),
                SpiritGrafter => new EventOption(__instance, () => ShatterAsync(__instance),
                    "SPIRIT_GRAFTER.pages.INITIAL.options.DEHYA_SHATTER"),
                ColossalFlower => BuildFlowerProbeOption(__instance),
                _ => null,
            };
            if (extra == null)
            {
                return;
            }
            if (__result is List<EventOption> list)
            {
                list.Add(extra);
            }
            else
            {
                __result = __result.Append(extra).ToList();
            }
        }
        catch (Exception e)
        {
            Log.Error($"[STS2-Dehya] 原版事件选项追加失败({__instance.GetType().Name}): {e}");
        }
    }

    /// <summary>垃圾堆·随便扔点什么:移除卡组中一张随机卡(设计案原文「随机」,非自选)。</summary>
    private static async Task TossSomethingAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        List<CardModel> deck = player.Deck.Cards.ToList();
        if (deck.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(evt.Rng.NextItem(deck), showPreview: true);
        }
        FinishDelegate!.Invoke(evt, new LocString("events", "TRASH_HEAP.pages.DEHYA_TOSS.description"));
    }

    /// <summary>灵魂嫁接者·击碎:降级 2 张随机卡牌,移除 1 张随机卡牌
    /// (Mirror 裁定 D8:删 1 降 2 有得有失;随机口径与非卡清点 noncard-10 裁定一致)。</summary>
    private static async Task ShatterAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        List<CardModel> downgradable = player.Deck.Cards.Where(static c => c.IsUpgraded).ToList();
        for (int i = 0; i < 2 && downgradable.Count > 0; i++)
        {
            CardModel card = evt.Rng.NextItem(downgradable);
            downgradable.Remove(card);
            CardCmd.Downgrade(card);
            CardCmd.Preview(card, 1.2f, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
        }
        List<CardModel> deck = player.Deck.Cards.ToList();
        if (deck.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(evt.Rng.NextItem(deck), showPreview: true);
        }
        FinishDelegate!.Invoke(evt, new LocString("events", "SPIRIT_GRAFTER.pages.DEHYA_SHATTER.description"));
    }

    /// <summary>巨大花卉·向它扔些东西试探:失去一瓶随机药水,升级 N 张随机牌
    /// (Mirror 裁定 D8→L4 修订:普通药水 1 张、罕见与稀有药水 2 张;结果页按 1/2 张二分共用文案)。
    /// 无药水时锁定(null handler),文案「锁定 至少需要一瓶药水才能选择」。</summary>
    private static EventOption? BuildFlowerProbeOption(EventModel evt)
    {
        Player player = evt.Owner!;
        if (!player.Potions.Any())
        {
            return new EventOption(evt, null, "COLOSSAL_FLOWER.pages.INITIAL.options.DEHYA_THROW_PROBE_LOCKED");
        }
        return new EventOption(evt, () => ThrowProbeAsync(evt),
            "COLOSSAL_FLOWER.pages.INITIAL.options.DEHYA_THROW_PROBE");
    }

    private static async Task ThrowProbeAsync(EventModel evt)
    {
        Player player = evt.Owner!;
        var potion = evt.Rng.NextItem(player.Potions.ToList());
        int upgrades = potion.Rarity == PotionRarity.Common ? 1 : 2;  // L4:罕见也算 2 张
        string potionName = potion.Rarity == PotionRarity.Common ? "common" : "rare";
        potion.Discard();
        List<CardModel> upgradable = player.Deck.Cards.Where(static c => c.IsUpgradable).ToList();
        for (int i = 0; i < upgrades && upgradable.Count > 0; i++)
        {
            CardModel card = evt.Rng.NextItem(upgradable);
            upgradable.Remove(card);
            CardCmd.Upgrade(card, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
        }
        FinishDelegate!.Invoke(evt, new LocString("events",
            potionName == "rare" ? "COLOSSAL_FLOWER.pages.DEHYA_THROW_PROBE_RARE.description" : "COLOSSAL_FLOWER.pages.DEHYA_THROW_PROBE_COMMON.description"));
    }
}
