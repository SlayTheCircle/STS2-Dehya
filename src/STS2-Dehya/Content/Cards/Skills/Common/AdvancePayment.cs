using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 预支定金(普通技能,0费,虚无):消耗20金币,抽2张牌。金币不足20时不可打出
/// (裁定口径:IsPlayable 按 Gold 门槛拦截,原版 Clash 式条件不可用)。
/// 已知并接受的引擎口径(2026-10-06 裁定):自动打出路径(混乱/Havoc 等)不检查 IsPlayable,
/// 金币不足时被自动打出会扣款钳 0 照常结算——原版 Clash/GrandFinale 同款行为,不修。
/// 升级:抽牌2→3张。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class AdvancePayment : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Ethereal };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(20),
        new CardsVar(2),
    };

    protected override bool IsPlayable => base.Owner.Gold >= (int)base.DynamicVars.Gold.BaseValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    public AdvancePayment()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.LoseGold(base.DynamicVars.Gold.BaseValue, base.Owner, GoldLossType.Spent);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
