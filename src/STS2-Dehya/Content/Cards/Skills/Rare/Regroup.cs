using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 重整旗鼓(稀有技能,消耗):移除你身上的所有减益(裁定口径 PowerType.Debuff 的 Power 整块移除),
/// 抽 2 张牌。先快照再逐个移除,避免边遍历边改 Powers 列表。升级:费用 3→2。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class Regroup : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new CardsVar(2) };

    public Regroup()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<PowerModel> debuffs = base.Owner.Creature.Powers.Where(static p => p.Type == PowerType.Debuff).ToList();
        foreach (PowerModel debuff in debuffs)
        {
            await PowerCmd.Remove(debuff);
        }
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
