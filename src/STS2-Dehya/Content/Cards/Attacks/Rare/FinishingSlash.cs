using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 终结之斩(稀有攻击,消耗):造成 30 点伤害并获得 2 点能量。若此伤害击败敌人,
/// 将这张牌的一个复制(保留当前升级状态,CreateClone 战斗克隆)放入抽牌堆。升级:伤害 30→40。
/// 击杀判定读 AttackCommand.Results 的 DamageResult.WasTargetKilled(vanilla Sunder 同款)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class FinishingSlash : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(30m, ValueProp.Move),
        new EnergyVar(2),
    };

    public FinishingSlash()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        bool killed = (await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext)).Results.SelectMany(static r => r).Any(static r => r.WasTargetKilled);
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        if (killed)
        {
            CardModel copy = CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
