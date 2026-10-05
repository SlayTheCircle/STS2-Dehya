using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 悬赏委托(罕见技能,2费,消耗):标记一名敌人(可见减益「悬赏标记」);该敌人被击败(任意死因)时,
/// 获得1点能量、抽1张牌,并在战斗胜利后获得20金币(经隐藏账本 VictoryGoldPower 结算)。
/// 入手即开始计数,计满5场战斗后从主卡组自删([SavedProperty] 计数;获得卡的战斗算第1场)。
/// 升级:击败时的抽牌1→2张。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BountyContract : DehyaCardBase
{
    private const int BattlesBeforeSelfRemoval = 5;

    private int _battlesCounted;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BountyMarkPower>(20m),
        new CardsVar(1),
    };

    [SavedProperty]
    public int BattlesCounted
    {
        get => _battlesCounted;
        set
        {
            AssertMutable();
            _battlesCounted = value;
        }
    }

    public BountyContract()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        BountyMarkPower? mark = await PowerCmd.Apply<BountyMarkPower>(choiceContext, cardPlay.Target, base.DynamicVars["BountyMarkPower"].BaseValue, base.Owner.Creature, this);
        mark?.Configure((int)base.DynamicVars.Cards.BaseValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        // 战斗克隆也会收到本钩子,只在主卡组实例上计数
        if (base.Pile?.Type != PileType.Deck)
        {
            return;
        }
        // 入手即计:首个战斗结束钩子时计到2(获得卡的战斗=1),此后每场+1;计满自删。
        // 非战斗入手(商店/事件/药水)同样按「入手当回合=第1场槽位」计——任何来源入手后都恰好
        // 经历 4 场真实战斗自删,统一口径(2026-10-06 审查确认,无 off-by-one)。
        BattlesCounted = BattlesCounted == 0 ? 2 : BattlesCounted + 1;
        if (BattlesCounted >= BattlesBeforeSelfRemoval)
        {
            await CardPileCmd.RemoveFromDeck(this, showPreview: false);
        }
    }
}
