using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 怒势疾迅(先古,0费攻击,欧洛巴斯给予):造成6点伤害,给予3层易伤,抽2张牌。
/// 升级:伤害9,易伤5层。先古稀有度入 Ancient 目录(空 Ancient 池会崩达弗/DustyTome)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
// 先古卡须进池(Charlotte 先例):接牙后由 RitsuLib TranscendenceCardsPatch 自动从尘封魔典候选剔除,
// 不与铓辉灿漫(魔典卡)互抢;怒势疾迅的给予途径=古老牙齿转化,别处不应再发放。
public sealed class FuriousRush : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<VulnerablePower>(3m),
        new CardsVar(2),
    };

    public FuriousRush()
        : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars.Vulnerable.UpgradeValueBy(2m);
    }
}
