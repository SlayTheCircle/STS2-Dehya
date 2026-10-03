using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 热血沸腾(罕见,0费技能):抽1张牌,失去5生命;若生命值高于50%,再抽2张牌。
/// 升级:失去的生命 5→3。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BoilingBlood : DehyaCardBase
{
    private const int BonusDraw = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1),
        new DynamicVar("HpLoss", 5m),
    };

    public BoilingBlood()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        await HpLossCmd.LoseHpFromCard(choiceContext, base.Owner.Creature, base.DynamicVars["HpLoss"].BaseValue, this, cardPlay);
        if (base.Owner.Creature.GetHpPercentRemaining() > 0.5)
        {
            await CardPileCmd.Draw(choiceContext, BonusDraw, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpLoss"].UpgradeValueBy(-2m);
    }
}
