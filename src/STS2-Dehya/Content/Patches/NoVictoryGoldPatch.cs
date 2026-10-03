using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Rewards;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 「加码加价」的本场奖励金币清零:静态登记表按(玩家, 当次战斗房间)记录标记,
/// 战后奖励生成时移除该玩家在该房间的全部金币条目并消耗标记(仅本场,房间不匹配不生效)。
/// 用运行时 Harmony 补丁而非 BeforeCombatRewardOffered 钩子——后者 0.111 才引入,
/// 而 RewardsSet.GenerateRewardsFor 在 0.107.1/0.111 同名同形,两目标共用一份补丁。
/// </summary>
internal static class NoVictoryGoldTracker
{
    private static readonly Dictionary<Player, AbstractRoom> MarkedRooms = new();

    public static void Mark(Player player, AbstractRoom? room)
    {
        if (room != null)
        {
            MarkedRooms[player] = room;
        }
    }

    public static bool Consume(Player player, AbstractRoom room)
    {
        return MarkedRooms.TryGetValue(player, out AbstractRoom? marked) && marked == room && MarkedRooms.Remove(player);
    }
}

/// <summary>
/// 拦截战斗奖励生成(私有方法按名补丁):被标记玩家在该房间的金币奖励条目清零。
/// 注意只动 GenerateRewardsFor 产出的原版金币;Royalties/悬赏类 ExtraReward 是另行赚取的赏金,不受影响。
/// </summary>
[HarmonyPatch(typeof(RewardsSet), "GenerateRewardsFor")]
internal static class NoVictoryGoldPatch
{
    private static void Postfix(Player player, AbstractRoom room, ref List<Reward> __result)
    {
        if (NoVictoryGoldTracker.Consume(player, room))
        {
            __result.RemoveAll(static r => r is GoldReward);
        }
    }
}
