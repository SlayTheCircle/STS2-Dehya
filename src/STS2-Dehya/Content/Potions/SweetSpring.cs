using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.PotionPools;

namespace DehyaMod.Content.Potions;

/// <summary>
/// 甘甜清泉:获得3点最大生命值,然后恢复最大生命值的30%(去小数,鲜血药水口径——Mirror 裁定 D8)。
/// 治疗类药水,场外可用(AnyTime);稀有度暂定 Common(noncard-08 待追认)。
/// </summary>
[RegisterPotion(typeof(DehyaPotionPool))]
public sealed class SweetSpring : DehyaPotionBase
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.AnyTime;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("MaxHpGain", 3m),
        new DynamicVar("HealPercent", 30m),
    };

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var owner = base.Owner.Creature;
        await CreatureCmd.GainMaxHp(owner, base.DynamicVars["MaxHpGain"].BaseValue); // 增上限同时等量回复
        // 以提升后的最大生命值为基数取 30%,向下取整(Mirror:小数不要了)。
        decimal heal = System.Math.Floor((decimal)owner.MaxHp * base.DynamicVars["HealPercent"].BaseValue / 100m);
        await CreatureCmd.Heal(owner, heal);
    }
}
