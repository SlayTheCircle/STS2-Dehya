using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 炽热余烬(普通,0费攻击):造成4点伤害,丢弃1张手牌(玩家选);若弃掉的不是0费牌,抽2张牌。
/// 升级:伤害6,抽3张牌(抽牌基数按规格表升级列 2→3 取值)。弃选照 vanilla Acrobatics 的 FromHandForDiscard。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class ScorchingEmbers : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new CardsVar(2),
    };

    public ScorchingEmbers()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        CardModel? discarded = (await CardSelectCmd.FromHandForDiscard(choiceContext, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
        if (discarded != null)
        {
            await CardCmd.Discard(choiceContext, discarded);
            // 「0费牌」口径同原版 AllForOne/Scrape:GetWithModifiers 未钳位(-1 费诅咒/状态不被钳成 0),
            // 且排除 X 费;GetResolved 会把不可打出牌的 -1 钳为 0,导致弃诅咒不触发抽牌(2026-10-06 审查修正)。
            bool discardedIsZeroCost = discarded.EnergyCost.GetWithModifiers(CostModifiers.All) == 0 && !discarded.EnergyCost.CostsX;
            if (!discardedIsZeroCost)
            {
                await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
