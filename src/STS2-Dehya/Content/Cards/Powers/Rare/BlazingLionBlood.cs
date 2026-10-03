using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 炽鬃狮血(稀有能力):动态力量——你每缺失 5 点生命便获得 1 点真实力量,
/// 生命恢复时相应回落(监听 HP 变化同步真实 StrengthPower 增量)。
/// 升级:每 5 点→每 4 点。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BlazingLionBlood : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<BlazingLionBloodPower>(5m),
    };

    public BlazingLionBlood()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<BlazingLionBloodPower>(choiceContext, base.Owner.Creature, base.DynamicVars["BlazingLionBloodPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BlazingLionBloodPower"].UpgradeValueBy(-1m);
    }
}
