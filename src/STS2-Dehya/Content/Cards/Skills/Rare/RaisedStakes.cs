using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Patches;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 加码加价(稀有技能,1费,保留+消耗):消耗抽牌堆中3张随机牌,获得3点力量;
/// 本场战斗的奖励金币清零(隐藏标记 NoVictoryGoldPower + NoVictoryGoldPatch 按房间拦截,仅本场)。
/// 升级:力量3→5(消耗张数不变)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class RaisedStakes : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Retain, CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(3m),
        new CardsVar(3),
    };

    public RaisedStakes()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Mirror 裁定 B17(2026-10-04):消耗抽牌堆「顶部」{Cards} 张(索引0为顶,MoveToTopInternal Insert(0) 实证),非随机。
        List<CardModel> chosen = PileType.Draw.GetPile(base.Owner).Cards
            .Take((int)base.DynamicVars.Cards.BaseValue)
            .ToList();
        foreach (CardModel card in chosen)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<NoVictoryGoldPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
        NoVictoryGoldTracker.Mark(base.Owner, base.RunState?.CurrentRoom);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Strength.UpgradeValueBy(2m);
    }
}
