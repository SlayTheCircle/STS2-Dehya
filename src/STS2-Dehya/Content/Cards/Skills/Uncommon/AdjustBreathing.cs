using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 调整呼吸(罕见技能):抽 1 张牌。若抽到的是 0 费牌,弃 1 张牌(由你选择,原版 Acrobatics 选弃范式),
/// 再抽 2 张牌。升级:再抽的张数 2→3。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class AdjustBreathing : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new CardsVar(2) };

    public AdjustBreathing()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? first = (await CardPileCmd.Draw(choiceContext, 1m, base.Owner)).FirstOrDefault();
        if (first is null || first.EnergyCost.CostsX || first.EnergyCost.GetWithModifiers(CostModifiers.All) != 0)
        {
            return;
        }
        CardModel? toDiscard = (await CardSelectCmd.FromHandForDiscard(choiceContext, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
        if (toDiscard != null)
        {
            await CardCmd.Discard(choiceContext, toDiscard);
        }
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
