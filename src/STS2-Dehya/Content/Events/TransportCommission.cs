using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Events;

/// <summary>
/// 运输委托(1 层事件,Overgrowth)。设计案:地精委托运输 vs 抢走货物。
/// - 帮助运输:获得 105~120 金币(Mirror 裁定 D8:随机取整,NextInt(105,121))。
/// - 抢走货物:失去 8 点最大生命值,获得 1 件随机遗物(PullNextRelicFromFront 无稀有度重载=随机稀有度)。
/// </summary>
[RegisterActEvent(typeof(Overgrowth))]
public sealed class TransportCommission : DehyaEventBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar("MinGold", 105),
        new GoldVar("MaxGold", 120),
        new DynamicVar("MaxHpLoss", 8m),
    };

    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://STS2-Dehya/images/events/TransportCommission.png");

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() => new[]
    {
        new EventOption(this, HelpTransport, InitialOptionKey("HELP_TRANSPORT")),
        new EventOption(this, StealGoods, InitialOptionKey("STEAL_GOODS")),
    };

    private async Task HelpTransport()
    {
        int gold = base.Rng.NextInt((int)base.DynamicVars["MinGold"].BaseValue, (int)base.DynamicVars["MaxGold"].BaseValue + 1);
        await PlayerCmd.GainGold(gold, base.Owner);
        SetEventFinished(PageDescription("HELPED"));
    }

    private async Task StealGoods()
    {
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["MaxHpLoss"].BaseValue, isFromCard: false);
        RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
        await RelicCmd.Obtain(relic, base.Owner);
        SetEventFinished(PageDescription("STOLE"));
    }
}
