using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.RelicPools;

namespace DehyaMod.Content.Relics;

/// <summary>
/// 炽金锅(罕见遗物):拾起时恢复 20 点生命;每场战斗中你第一次失去生命值时,获得 1 层敏捷。
/// 拾起治疗走 HasUponPickupEffect + AfterObtained(NutritiousOyster 范式,奖励界面外也能结算);
/// 「第一次失去生命值」为战斗内一次性标记,AfterCombatEnd 重置(RosulaEmblem 范式),
/// 观察 HP 变化钩子的负增量,拾起治疗自身(正增量)不会误触发。
/// </summary>
[RegisterRelic(typeof(DehyaRelicPool))]
public sealed class GildedCookpot : DehyaRelicBase
{
    private bool _lostHpThisCombat;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool HasUponPickupEffect => true;

    /// <summary>描述中的机制名词挂悬停词条(非卡牌模型无关键词管线,须显式挂;原版 BeltBuckle 范式)。</summary>
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<DexterityPower>(),
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new HealVar(20m),
        new PowerVar<DexterityPower>(1m),
    };

    private bool LostHpThisCombat
    {
        get => _lostHpThisCombat;
        set
        {
            AssertMutable();
            _lostHpThisCombat = value;
        }
    }

    public override async Task AfterObtained()
    {
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!CombatManager.Instance.IsInProgress || creature != base.Owner.Creature || delta >= 0m || LostHpThisCombat)
        {
            return;
        }
        LostHpThisCombat = true;
        Flash();
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, null);
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        LostHpThisCombat = false;
        return Task.CompletedTask;
    }
}
