using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Acts;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Events;

/// <summary>
/// 灼眼烈焰(3 层事件,Glory)。设计案:扑面火焰的两难。
/// - 扑灭火焰:失去一瓶随机药水,获得一件普通遗物(草案「普通遗物」= RelicRarity.Common);
///   无药水时选项锁定(Mirror 裁定 D8:null handler 锁定范式,文案「锁定 至少需要一瓶药水」)。
/// - 接纳火焰:失去 5 点最大生命值,恢复 22 点生命。
/// </summary>
[RegisterActEvent(typeof(Glory))]
public sealed class ScorchingFlare : DehyaEventBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("MaxHpLoss", 5m),
        new HealVar(22m),
    };

    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://STS2-Dehya/images/events/ScorchingFlare.png");

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> options = new();
        if (base.Owner.Potions.Any())
        {
            options.Add(new EventOption(this, Extinguish, InitialOptionKey("EXTINGUISH")));
        }
        else
        {
            options.Add(new EventOption(this, null, InitialOptionKey("EXTINGUISH_LOCKED")));
        }
        options.Add(new EventOption(this, EmbraceFlame, InitialOptionKey("EMBRACE_FLAME")));
        return options;
    }

    private async Task Extinguish()
    {
        var potion = base.Rng.NextItem(base.Owner.Potions.ToList());
        potion.Discard();
        RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Common).ToMutable();
        await RelicCmd.Obtain(relic, base.Owner);
        SetEventFinished(PageDescription("EXTINGUISHED"));
    }

    private async Task EmbraceFlame()
    {
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["MaxHpLoss"].BaseValue, isFromCard: false);
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
        SetEventFinished(PageDescription("EMBRACED"));
    }
}
