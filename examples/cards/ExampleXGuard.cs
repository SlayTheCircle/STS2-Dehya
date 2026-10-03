using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Cards;

namespace DehyaMod.Examples.Cards;

[RegisterCard(typeof(DehyaCardPool))]
public sealed class ExampleXGuard : DehyaCardBase
{
    protected override bool HasEnergyCostX => true;
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(3m, ValueProp.Move) };

    public ExampleXGuard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int count = ResolveEnergyXValue();
        for (int i = 0; i < count; i++)
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, play);
    }

    protected override void OnUpgrade() => base.DynamicVars.Block.UpgradeValueBy(1m);
}
