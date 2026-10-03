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
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 包扎伤口(稀有,2费技能):获得6层再生,并挂窗口标记——下回合结束前你首次失去生命时,
/// 移除全部再生,对所有敌人造成每层2点伤害。升级:费用 2→1。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BandageWounds : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<RegenPower>(6m),
        new PowerVar<BandageWoundsMarkerPower>(2m),
    };

    public BandageWounds()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RegenPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RegenPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<BandageWoundsMarkerPower>(choiceContext, base.Owner.Creature, base.DynamicVars["BandageWoundsMarkerPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
