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
/// 燎原野火(稀有攻击):造成 45 点伤害;本回合内你每打出一张 0 费牌,
/// 手牌中的这张牌费用 -1(可叠加,回合结束复原)。升级:伤害 45→54。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BlazingWildfire : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(45m, ValueProp.Move),
    };

    public BlazingWildfire()
        : base(5, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(9m);
    }
}
