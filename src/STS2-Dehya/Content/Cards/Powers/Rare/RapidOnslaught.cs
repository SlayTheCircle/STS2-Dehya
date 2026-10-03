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
/// 极速突击(稀有能力,2费):你每打出1张费用为0的卡牌时,获得 4 点活力(经可见 RapidOnslaughtPower
/// 持续触发,原生 VigorPower 直加)。升级:每次活力 4→6。
/// 裁定 F1:按设计原文「每打出1张0费卡获得活力」的持续触发实现(美术图标佐证)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class RapidOnslaught : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<RapidOnslaughtPower>(4m),
    };

    public RapidOnslaught()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "PowerUp", base.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<RapidOnslaughtPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RapidOnslaughtPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["RapidOnslaughtPower"].UpgradeValueBy(2m);
    }
}
