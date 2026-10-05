using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.RelicPools;

namespace DehyaMod.Content.Relics;

/// <summary>
/// 止痛剂(罕见遗物):每当你在自己的回合失去生命值,恢复 1 点生命。
/// 观察 HP 变化钩子(负增量=失去,正增量=恢复天然自排除,不会循环触发);
/// 「自己的回合」按 CombatState.CurrentSide == 自己阵营判定(DemonTongue 范式),每次失去生命事件各结算一次。
/// 致命一击不救(2026-10-06 裁定):本钩子派发于引擎死亡判定之前,0 血时治疗会把 IsDead 抬回
/// false、阻断整个死亡流程——等于免费的全款「己方回合免死」,越权于 Rare 能力「死战不屈」的定位;
/// 扣到 0 交由死亡流程结算,其余失血照常恢复。
/// </summary>
[RegisterRelic(typeof(DehyaRelicPool))]
public sealed class Painkillers : DehyaRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new HealVar(1m) };

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!CombatManager.Instance.IsInProgress || creature != base.Owner.Creature || delta >= 0m)
        {
            return;
        }
        if (base.Owner.Creature.CombatState is not { } combatState || combatState.CurrentSide != base.Owner.Creature.Side)
        {
            return;
        }
        if (base.Owner.Creature.CurrentHp <= 0m)
        {
            return;
        }
        Flash();
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
    }
}
