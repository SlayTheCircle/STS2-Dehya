using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 战后金币账本(隐藏):挂在本方玩家生物上,累计「战斗胜利后发放」的金币。
/// 敌人被击败时,按「雇佣关系」(MercenaryContractPower)的每杀赏金自动入账;
/// 「重金悬赏」「悬赏委托」的直接赏金也经 AddPendingGold 写入此处。
/// 战斗结束时走原版 Royalties 同款管线,以额外金币奖励发到奖励界面(原版 GreedyBank 思路)。
/// </summary>
[RegisterPower]
public sealed class VictoryGoldPower : PowerModel
{
    private sealed class Data
    {
        public decimal PendingGold;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    protected override object InitInternalData() => new Data();

    /// <summary>确保账本存在并返回实例(已有则复用,Amount 恒为 1 的哑值,真实账目在内部数据)。</summary>
    public static async Task<VictoryGoldPower?> EnsureApplied(PlayerChoiceContext choiceContext, Player player, CardModel? cardSource)
    {
        VictoryGoldPower? existing = player.Creature.GetPower<VictoryGoldPower>();
        if (existing != null)
        {
            return existing;
        }
        return await PowerCmd.Apply<VictoryGoldPower>(choiceContext, player.Creature, 1m, player.Creature, cardSource);
    }

    /// <summary>记录一笔战斗胜利后发放的金币。</summary>
    public void AddPendingGold(decimal amount) => GetInternalData<Data>().PendingGold += amount;

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!wasRemovalPrevented && creature.IsMonster)
        {
            decimal perKill = base.Owner.GetPower<MercenaryContractPower>()?.Amount ?? 0m;
            if (perKill > 0m)
            {
                AddPendingGold(perKill);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        int pending = (int)GetInternalData<Data>().PendingGold;
        if (pending > 0)
        {
            room.AddExtraReward(base.Owner.Player, new GoldReward(pending, base.Owner.Player));
        }
        return Task.CompletedTask;
    }
}
