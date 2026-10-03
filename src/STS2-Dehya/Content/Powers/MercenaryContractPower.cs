using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 雇佣关系(可见增益):回合开始(能量重置后)时,每持有100金币消耗3金币并获得1点能量
/// (先按当前金币算出份额,再扣金币——先算后耗)。层数同时是「每击败一名敌人,战斗胜利后+X金币」
/// 的每杀赏金,由 VictoryGoldPower 在敌人死亡时读取入账。
/// </summary>
[RegisterPower]
public sealed class MercenaryContractPower : DehyaPowerBase
{
    private const int GoldPerShare = 100;
    private const int GoldCostPerShare = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IncludeEnergyHoverTip => true;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        int shares = player.Gold / GoldPerShare; // 先算:按当前金币取份额
        if (shares <= 0)
        {
            return;
        }
        Flash();
        await PlayerCmd.LoseGold(GoldCostPerShare * shares, player, GoldLossType.Spent); // 后耗
        await PlayerCmd.GainEnergy(shares, player);
    }
}
