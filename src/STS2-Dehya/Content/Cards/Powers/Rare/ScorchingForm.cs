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
/// 灼热形态(稀有,3 费能力):获得 7 层覆甲;每回合开始时,失去 1 点生命,获得 1 层荆棘,对所有敌人造成 9 点伤害。
/// 升级:覆甲 7→9,AOE 9→12。回合逻辑在 <see cref="ScorchingFormPower"/>,AOE 经 Configure 注入(The Bomb 范式)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class ScorchingForm : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<ScorchingFormPower>(1m),
        new PowerVar<PlatingPower>(7m),
        new DynamicVar("AoeDamage", 9m),
    };

    public ScorchingForm()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NPowerUpVfx.CreateNormal(base.Owner.Creature);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, base.DynamicVars["PlatingPower"].BaseValue, base.Owner.Creature, this);
        ScorchingFormPower? power = await PowerCmd.Apply<ScorchingFormPower>(choiceContext, base.Owner.Creature, base.DynamicVars["ScorchingFormPower"].BaseValue, base.Owner.Creature, this);
        power?.Configure(base.DynamicVars["AoeDamage"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PlatingPower"].UpgradeValueBy(2m);
        base.DynamicVars["AoeDamage"].UpgradeValueBy(3m);
    }
}
