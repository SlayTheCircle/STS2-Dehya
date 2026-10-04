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

namespace DehyaMod.Content.Cards;

/// <summary>
/// 蓄势打击(罕见攻击):造成 10 点伤害并抽 1 张牌。每次打出后,这张牌本场战斗中费用 +1、抽牌数 +1。
/// 升级:抽牌数 1→2(伤害按规格裁定保持 10 不变)。
/// 战斗内成长用标量字段承载:MemberwiseClone 自动复制标量(state.md 标量克隆规则),
/// 引擎另自行克隆 DynamicVars/费用修饰,故不覆写 DeepCloneFields。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class MomentumStrike : DehyaCardBase
{
    private int _plays;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new CardsVar(1),
    };

    public MomentumStrike()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int draws = base.DynamicVars.Cards.IntValue;
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, draws, base.Owner);

        // 战斗内自我成长:费 +1(EndOfCombat 本地费用修饰,随克隆)、抽牌数 +1(由字段推导同步到卡面)。
        _plays++;
        base.EnergyCost.AddThisCombat(1);
        base.DynamicVars.Cards.BaseValue = draws + _plays;
    }

    protected override void OnUpgrade()
    {
        // Mirror 裁定 A1(2026-10-04):升级伤害 10→11(原案「1」为笔误,漏打十位)。
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
