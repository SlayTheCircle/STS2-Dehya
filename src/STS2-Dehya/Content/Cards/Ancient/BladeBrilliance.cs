using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 铓辉灿漫(先古,2 费能力,达弗给予):每回合开始时,恢复 1 点生命。
/// 升级:费用 2→1。逻辑在 <see cref="BladeBrilliancePower"/>,治疗量 = 能力份数。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
// 达弗·尘封魔典给予:原版 SetupForPlayer 从角色卡池 Ancient 卡中随机选(排除牙齿转化卡后本池唯一)
// 且入卡组时自动升级(原版 DustyTome.AfterObtained CardCmd.Upgrade)——本卡升级=费用 2→1,已适配。
public sealed class BladeBrilliance : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BladeBrilliancePower>(1m),
    };

    public BladeBrilliance()
        : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NPowerUpVfx.CreateNormal(base.Owner.Creature);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<BladeBrilliancePower>(choiceContext, base.Owner.Creature, base.DynamicVars["BladeBrilliancePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级:2 费 → 1 费(vanilla 降费写法)。
        base.EnergyCost.UpgradeBy(-1);
    }
}
