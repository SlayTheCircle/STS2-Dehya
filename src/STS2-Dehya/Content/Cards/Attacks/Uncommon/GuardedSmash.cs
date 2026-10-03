using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 格挡猛击(罕见攻击,3费):获得15点格挡并造成15点伤害;若本回合此前打出的牌少于5张,
/// 失去3点生命并获得1点能量(打牌数取战斗历史 CardPlaysFinished,不含正在结算的此牌)。
/// 升级:格挡15→18,伤害15→16,张数门槛5→6。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class GuardedSmash : DehyaCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(15m, ValueProp.Move),
        new DamageVar(15m, ValueProp.Move),
        new DynamicVar("CardsThreshold", 5m),
        new DynamicVar("HpLoss", 3m),
        new EnergyVar(1),
    };

    public GuardedSmash()
        : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        int playedThisTurn = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.CardPlay.GetPlayer() == base.Owner && e.HappenedThisTurn(base.CombatState));
        if (playedThisTurn < (int)base.DynamicVars["CardsThreshold"].BaseValue)
        {
            await HpLossCmd.LoseHpFromCard(choiceContext, base.Owner.Creature, base.DynamicVars["HpLoss"].BaseValue, this, cardPlay);
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["CardsThreshold"].UpgradeValueBy(1m);
    }
}
