using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 纵横沙海(罕见,1 费能力):回合结束时,若你的格挡至少为 10 点,对所有敌人造成 4 点伤害,并获得 2 层再生。
/// 升级:门槛 10→8,再生 2→3。逻辑在 <see cref="SandsweptPower"/>,三数值经 Configure 注入(The Bomb 范式)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class Sandswept : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<SandsweptPower>(1m),
        new DynamicVar("Threshold", 10m),
        new DynamicVar("AoeDamage", 4m),
        new PowerVar<RegenPower>(2m),
    };

    public Sandswept()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NPowerUpVfx.CreateNormal(base.Owner.Creature);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        SandsweptPower? power = await PowerCmd.Apply<SandsweptPower>(choiceContext, base.Owner.Creature, base.DynamicVars["SandsweptPower"].BaseValue, base.Owner.Creature, this);
        power?.Configure(
            base.DynamicVars["Threshold"].IntValue,
            base.DynamicVars["AoeDamage"].BaseValue,
            base.DynamicVars["RegenPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Threshold"].UpgradeValueBy(-2m);
        base.DynamicVars["RegenPower"].UpgradeValueBy(1m);
    }
}
