using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 剑斗技巧(罕见技能):下回合获得 3 点临时力量;本回合内你每打出一张攻击牌,
/// 手牌中的这张牌费用 -1(下限 0,回合结束复原)。升级:临时力量 3→4。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class DuelingTechnique : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<DelayedTemporaryStrengthPower>(3m),
    };

    public DuelingTechnique()
        : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<DelayedTemporaryStrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars["DelayedTemporaryStrengthPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<AttackPlayCostTrackerPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DelayedTemporaryStrengthPower"].UpgradeValueBy(1m);
    }
}
