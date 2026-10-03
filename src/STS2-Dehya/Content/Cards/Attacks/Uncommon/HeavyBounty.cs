using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 重金悬赏(罕见攻击,0费):造成4点伤害,每持有100金币伤害额外+5(三件套按打出时金币先算);
/// 以此牌击败敌人时,战斗胜利后+30金币(经隐藏账本 VictoryGoldPower 结算)。
/// 升级:基础伤害4→5,每百金币加成+5→+6。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class HeavyBounty : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(4m),
        new ExtraDamageVar(5m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            static (card, _) => card.Owner is null ? 0m : (decimal)(card.Owner.Gold / 100)),
    };

    public HeavyBounty()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal amount = base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target); // 先算:按当前金币取值
        await DamageCmd.Attack(amount).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (cardPlay.Target.IsDead)
        {
            VictoryGoldPower? ledger = await VictoryGoldPower.EnsureApplied(choiceContext, base.Owner, this);
            ledger?.AddPendingGold(30m);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(1m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
