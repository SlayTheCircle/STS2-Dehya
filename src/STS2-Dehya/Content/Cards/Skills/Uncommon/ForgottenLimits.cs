using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 忘却极限(罕见技能):抽牌直到手牌达到上限(手牌 10 张,不足才抽,原版 Dredge 的余量公式)。
/// 本次抽到的每张非 0 费牌(按含全部费用修饰的实际费用计,X 费按非 0 计,原版 Scrape 口径)
/// 使你失去 1 点生命。升级:费用 3→2。生命代价走 HpLossCmd 垫片(原版 Bloodletting 范式)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class ForgottenLimits : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DynamicVar("HpLoss", 1m) };

    public ForgottenLimits()
        : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int room = CardPile.MaxCardsInHand - PileType.Hand.GetPile(base.Owner).Cards.Count;
        if (room <= 0)
        {
            return;
        }
        IEnumerable<CardModel> drawn = await CardPileCmd.Draw(choiceContext, room, base.Owner);
        decimal hpLoss = drawn.Count(static c => c.EnergyCost.GetWithModifiers(CostModifiers.All) != 0 || c.EnergyCost.CostsX)
            * base.DynamicVars["HpLoss"].BaseValue;
        if (hpLoss > 0m)
        {
            await HpLossCmd.LoseHpFromCard(choiceContext, base.Owner.Creature, hpLoss, this, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
