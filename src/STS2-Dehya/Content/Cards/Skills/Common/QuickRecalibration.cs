using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
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
/// 快速调整(普通技能,0费):获得4点格挡。若本回合尚未打出过费用不低于2的牌,抽1张牌;
/// 否则弃1张牌(玩家选择)并获得1点能量(打牌记录取战斗历史 CardPlaysFinished,不含正在结算的此牌)。
/// 升级:获得保留(OnUpgrade 内 AddKeyword,不用条件式 CanonicalKeywords)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class QuickRecalibration : DehyaCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4m, ValueProp.Move),
        new CardsVar(1),
        new EnergyVar(1),
    };

    public QuickRecalibration()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        bool playedExpensiveCard = CombatManager.Instance.History.CardPlaysFinished
            .Any(e => e.CardPlay.GetPlayer() == base.Owner
                && e.CardPlay.Card.EnergyCost.GetResolved() >= 2m
                && e.HappenedThisTurn(base.CombatState));
        if (!playedExpensiveCard)
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        }
        else
        {
            CardModel? toDiscard = (await CardSelectCmd.FromHandForDiscard(choiceContext, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
            if (toDiscard != null)
            {
                await CardCmd.Discard(choiceContext, toDiscard);
            }
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        CardCmd.ApplyKeyword(this, CardKeyword.Retain);
    }
}
