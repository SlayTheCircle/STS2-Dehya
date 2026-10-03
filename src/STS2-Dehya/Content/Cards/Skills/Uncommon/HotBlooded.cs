using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 血气方刚(罕见,0费技能):失去3生命,获得2点能量;若生命值低于50%,再获得1点能量。
/// 升级:失去的生命 3→1。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class HotBlooded : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HpLoss", 3m),
        new EnergyVar(2),
        new EnergyVar("BonusEnergy", 1),
    };

    public HotBlooded()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        await HpLossCmd.LoseHpFromCard(choiceContext, base.Owner.Creature, base.DynamicVars["HpLoss"].BaseValue, this, cardPlay);
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        if (base.Owner.Creature.GetHpPercentRemaining() < 0.5)
        {
            await PlayerCmd.GainEnergy(base.DynamicVars["BonusEnergy"].IntValue, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpLoss"].UpgradeValueBy(-2m);
    }
}
