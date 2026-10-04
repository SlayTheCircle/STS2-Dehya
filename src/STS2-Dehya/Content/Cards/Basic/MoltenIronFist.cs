using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 熔铁之拳(初始卡):造成3点伤害,给予1层易伤。升级:伤害5,易伤2层。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
[RegisterArchaicToothTranscendence(typeof(FuriousRush))] // 欧洛巴斯之牙:熔铁之拳→怒势疾迅(Mirror 裁定 D3;Charlotte Kacha→Smile 同型,2026-10-04 核验:RitsuLib 补丁走原版 GetTranscendence* 路径,转化保升级与附魔)
public sealed class MoltenIronFist : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m),
    };

    public MoltenIronFist()
        : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}
