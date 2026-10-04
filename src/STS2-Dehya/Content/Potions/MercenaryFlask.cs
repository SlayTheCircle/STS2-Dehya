using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.PotionPools;

namespace DehyaMod.Content.Potions;

/// <summary>
/// 佣兵酒壶:获得5点力量,失去2点敏捷。战斗内使用(CombatOnly);稀有度暂定 Uncommon(noncard-08 待追认)。
/// </summary>
[RegisterPotion(typeof(DehyaPotionPool))]
public sealed class MercenaryFlask : DehyaPotionBase
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(5m),
        new DynamicVar("DexLoss", 2m),
    };

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var owner = base.Owner.Creature;
        await PowerCmd.Apply<StrengthPower>(choiceContext, owner, base.DynamicVars.Strength.BaseValue, owner, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, owner, -base.DynamicVars["DexLoss"].BaseValue, owner, null);
    }
}
