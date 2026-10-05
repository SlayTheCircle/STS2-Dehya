using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 锤砺锋芒(稀有攻击):造成 2 点伤害;本局游戏中你每打出一张费用不低于 2 的牌,
/// 这张卡的伤害永久 +2(增量走 SavedProperty,由 Content/Patches/TemperedEdgeTrackingPatch
/// 对主卡组与战斗克隆双写,本场即时生效)。升级:每张 +2→+3。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class TemperedEdge : DehyaCardBase
{
    private int _bonusDamage;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(2m, ValueProp.Move),
        new DynamicVar("Bonus", 2m),
    };

    /// <summary>
    /// 本局累计的永久伤害增量(存档持久)。保存独立增量,读档后随升级重放再写回,
    /// 避免把增量并入基础值后重复叠加(参照 ExampleRecordedGuard 模式)。
    /// </summary>
    [SavedProperty]
    public int BonusDamage
    {
        get => _bonusDamage;
        set
        {
            AssertMutable();
            base.DynamicVars.Damage.BaseValue += value - _bonusDamage;
            _bonusDamage = value;
        }
    }

    public TemperedEdge()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    /// <summary>
    /// 由 TemperedEdgeTrackingPatch 在每次打出费用不低于 2 的牌后调用:
    /// 按本实例当前(含升级)的单次加成累计永久增量。
    /// </summary>
    public void RegisterCostlyCardPlayed()
    {
        BonusDamage += (int)base.DynamicVars["Bonus"].BaseValue;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
