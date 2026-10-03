using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 悬赏标记(可见减益):挂在被标记的敌人身上。该敌人被击败(任意死因)时,施加者获得1点能量、
/// 抽 Configure 记录张数的牌,并向 VictoryGoldPower 记入战后发放的赏金(=本能力层数)。
/// </summary>
[RegisterPower]
public sealed class BountyMarkPower : DehyaPowerBase
{
    private sealed class Data
    {
        public int DrawCount = 1;
    }

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    /// <summary>记录击败奖励的抽牌张数(重复标记取较大值;基础1张,升级委托2张)。</summary>
    public void Configure(int drawCount)
    {
        Data data = GetInternalData<Data>();
        data.DrawCount = Math.Max(data.DrawCount, drawCount);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != base.Owner)
        {
            return;
        }
        if (base.Applier is not { IsDead: false })
        {
            return;
        }
        Flash();
        await PlayerCmd.GainEnergy(1m, base.Applier.Player);
        await CardPileCmd.Draw(choiceContext, GetInternalData<Data>().DrawCount, base.Applier.Player);
        VictoryGoldPower? ledger = await VictoryGoldPower.EnsureApplied(choiceContext, base.Applier.Player, cardSource: null);
        ledger?.AddPendingGold(base.Amount);
    }
}
