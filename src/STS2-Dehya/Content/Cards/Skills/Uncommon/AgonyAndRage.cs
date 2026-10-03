using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 至痛至怒(罕见技能,消耗):消耗手牌/抽牌堆/弃牌堆中全部状态牌与诅咒牌,
/// 每消耗 1 张获得 1 层覆甲和 1 点荆棘。升级:获得保留(AddKeyword)。
/// 「所有牌堆」按可参与循环的三堆计——消耗堆中的牌再消耗无意义,打出中的本卡自身是技能不受影响。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class AgonyAndRage : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<PlatingPower>(1m),
        new PowerVar<ThornsPower>(1m),
    };

    public AgonyAndRage()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        PlayerCombatState? piles = base.Owner.PlayerCombatState;
        if (piles is null)
        {
            return;
        }
        List<CardModel> doomed = piles.Hand.Cards
            .Concat(piles.DrawPile.Cards)
            .Concat(piles.DiscardPile.Cards)
            .Where(static c => c.Type == CardType.Status || c.Type == CardType.Curse)
            .ToList();
        foreach (CardModel card in doomed)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        if (doomed.Count > 0)
        {
            await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, doomed.Count * base.DynamicVars["PlatingPower"].BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner.Creature, doomed.Count * base.DynamicVars["ThornsPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
