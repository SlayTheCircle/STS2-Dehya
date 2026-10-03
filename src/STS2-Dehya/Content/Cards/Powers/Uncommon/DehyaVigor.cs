using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 示例昂扬(罕见能力):本场战斗中,你造成的伤害 +2。升级:数值 3。
/// 能力卡接线样例:PowerVar 声明数值,loc 用 {DehyaVigorPower:diff()} 引用(类名,不经前缀)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class DehyaVigor : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<DehyaVigorPower>(2m),
    };

    public DehyaVigor()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DehyaVigorPower>(choiceContext, base.Owner.Creature, base.DynamicVars["DehyaVigorPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DehyaVigorPower"].UpgradeValueBy(1m);
    }
}
