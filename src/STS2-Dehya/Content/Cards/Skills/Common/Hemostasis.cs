using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 紧急止血(普通,0费技能):消耗。移除自身的易伤和虚弱(PowerCmd.Remove),获得3层再生。升级:再生4层。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class Hemostasis : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new HashSet<CardKeyword> { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<RegenPower>(3m),
    };

    public Hemostasis()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Remove<VulnerablePower>(base.Owner.Creature);
        await PowerCmd.Remove<WeakPower>(base.Owner.Creature);
        await PowerCmd.Apply<RegenPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RegenPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["RegenPower"].UpgradeValueBy(1m);
    }
}
