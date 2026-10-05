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
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 加码加价(稀有技能,1费,保留+消耗):消耗抽牌堆中3张随机牌,获得3点力量;
/// 本场战斗的奖励金币清零(隐藏标记 NoVictoryGoldPower 战内在场 + 主卡组 SavedProperty 旗标
/// 持久拦截,读档重生成奖励不失效,仅本场)。
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

    private bool _victoryGoldSuppressed;

    /// <summary>本场战斗奖励金币清零的持久标记:写主卡组实例随存档,战斗奖励界面读档重开
    /// (SaveAndQuit→Continue 重生成奖励)也能保住惩罚;下场战斗 SetUpCombat 复位
    /// (NoVictoryGoldPatch/NoVictoryGoldResetPatch,2026-10-06 审查修正)。</summary>
    [SavedProperty]
    public bool VictoryGoldSuppressed
    {
        get => _victoryGoldSuppressed;
        set
        {
            AssertMutable();
            _victoryGoldSuppressed = value;
        }
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
        // OnPlay 中的 this 是战斗克隆,标记必须写到主卡组实例(DeckVersion,TheScythe 双写范式)。
        (base.DeckVersion as RaisedStakes ?? this).VictoryGoldSuppressed = true;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Strength.UpgradeValueBy(2m);
    }
}
