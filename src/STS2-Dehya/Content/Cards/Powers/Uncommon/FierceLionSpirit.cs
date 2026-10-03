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
/// 烈狮血性(罕见,3 费能力,虚无):每回合你打出的第一张费用大于等于 3 的牌,使你获得 1 层易伤,并获得 1 点能量。
/// 升级:不再虚无。逻辑在 <see cref="FierceLionSpiritPower"/>(Apparition 的虚无升级范式)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class FierceLionSpirit : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Ethereal };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<FierceLionSpiritPower>(1m),
    };

    public FierceLionSpirit()
        : base(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NPowerUpVfx.CreateNormal(base.Owner.Creature);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<FierceLionSpiritPower>(choiceContext, base.Owner.Creature, base.DynamicVars["FierceLionSpiritPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
