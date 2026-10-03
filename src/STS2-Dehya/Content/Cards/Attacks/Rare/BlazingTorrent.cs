using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 赤炎流火(稀有,3费攻击):先失去6点生命,再造成33点伤害。升级:失去4点生命,伤害44。
/// 自伤走 Compat 垫片 HpLossCmd(0.107/0.111 重载差异),结算顺序照 vanilla Breakthrough:先失生命后伤害。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BlazingTorrent : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(33m, ValueProp.Move),
        new DynamicVar("HpLoss", 6m),
    };

    public BlazingTorrent()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        await HpLossCmd.LoseHpFromCard(choiceContext, base.Owner.Creature, base.DynamicVars["HpLoss"].BaseValue, this, cardPlay);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(11m);
        base.DynamicVars["HpLoss"].UpgradeValueBy(-2m);
    }
}
