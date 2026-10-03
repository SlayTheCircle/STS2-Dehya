using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 雇佣关系(稀有能力,1费):获得20金币,并获得「雇佣关系」——回合开始每持有100金币耗3金币换1点能量,
/// 每击败一名敌人战斗胜利后+5金币(经隐藏账本 VictoryGoldPower 结算)。
/// 升级:立即金币20→25,每杀赏金5→7。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class MercenaryContract : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(20),
        new PowerVar<MercenaryContractPower>(5m),
    };

    public MercenaryContract()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue, base.Owner);
        await PowerCmd.Apply<MercenaryContractPower>(choiceContext, base.Owner.Creature, base.DynamicVars["MercenaryContractPower"].BaseValue, base.Owner.Creature, this);
        await VictoryGoldPower.EnsureApplied(choiceContext, base.Owner, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Gold.UpgradeValueBy(5m);
        base.DynamicVars["MercenaryContractPower"].UpgradeValueBy(2m);
    }
}
