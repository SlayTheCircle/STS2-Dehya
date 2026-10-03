using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 渐入佳境(稀有攻击):造成 4 点伤害并获得 1 点力量。每次打出后,这张牌本场战斗中费用 +1、伤害翻倍。
/// 升级:伤害 4→6(翻倍基于打出时刻的面板值,升级增量自然参与后续翻倍)。
/// 战斗内成长用标量字段承载:MemberwiseClone 自动复制标量(state.md 标量克隆规则),
/// 引擎另自行克隆 DynamicVars/费用修饰,故不覆写 DeepCloneFields。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class AscendingMastery : DehyaCardBase
{
    private int _plays;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new PowerVar<StrengthPower>(1m),
    };

    public AscendingMastery()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal damage = base.DynamicVars.Damage.BaseValue;
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);

        // 战斗内自我成长:费 +1(EndOfCombat 本地费用修饰,随克隆)、伤害翻倍(直接抬 DamageVar 基值,随克隆)。
        _plays++;
        base.EnergyCost.AddThisCombat(1);
        base.DynamicVars.Damage.BaseValue = damage * 2m;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
