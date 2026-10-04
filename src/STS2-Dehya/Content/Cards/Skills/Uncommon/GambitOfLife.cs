using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 搏命之灵(罕见,1费技能):失去10生命,获得2点力量;若生命值低于50%,再获得3点力量
/// (在失去生命之后判定)。升级:失去的生命 10→8。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class GambitOfLife : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HpLoss", 10m),
        new PowerVar<StrengthPower>(2m),
        new DynamicVar("BonusStrength", 3m),
    };

    public GambitOfLife()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        await HpLossCmd.LoseHpFromCard(choiceContext, base.Owner.Creature, base.DynamicVars["HpLoss"].BaseValue, this, cardPlay);
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        if (base.Owner.Creature.GetHpPercentRemaining() <= 0.5)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars["BonusStrength"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpLoss"].UpgradeValueBy(-2m);
    }
}
