using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.RelicPools;

namespace DehyaMod.Content.Relics;

/// <summary>
/// 玩具木剑(罕见遗物):回合结束时若你仍有格挡,对随机敌人造成 5 点伤害。
/// 走 BeforeSideTurnEnd(此时点格挡尚未在下回合开始清除,且敌方受伤效果按引擎约定放本钩子);
/// 随机目标用 Rng.CombatTargets(ForgottenSoul/Juggernaut 同款),无存活敌人时静默跳过。
/// </summary>
[RegisterRelic(typeof(DehyaRelicPool))]
public sealed class ToyWoodenSword : DehyaRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(5m, ValueProp.Unpowered) };

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner.Creature))
        {
            return;
        }
        if (base.Owner.Creature.Block <= 0 || base.Owner.Creature.CombatState is not { } combatState)
        {
            return;
        }
        Creature? target = base.Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
        if (target == null)
        {
            return;
        }
        Flash();
        VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_attack_blunt");
        await CreatureCmd.Damage(choiceContext, target, base.DynamicVars.Damage, base.Owner.Creature);
    }
}
