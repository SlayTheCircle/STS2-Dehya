using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Acts;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DehyaMod.Content.Events;

/// <summary>
/// 危险旅伴(2 层事件,Hive)。设计案:假佣兵搭讪。
/// - 戳穿他:升级你卡组中的两张随机牌(REFLECTIONS 的 NextItem+CardCmd.Upgrade 范式)。
/// - 假意答应:失去 12 点生命,移除你卡组中的一张牌(Mirror 裁定 D8:自选,FromDeckForRemoval)。
/// </summary>
[RegisterActEvent(typeof(Hive))]
public sealed class DangerousCompanion : DehyaEventBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HpLoss", 12m),
        new DynamicVar("UpgradeCount", 2m),
    };

    public override EventAssetProfile AssetProfile => new(
        InitialPortraitPath: "res://STS2-Dehya/images/events/DangerousCompanion.png");

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() => new[]
    {
        new EventOption(this, Expose, InitialOptionKey("EXPOSE")),
        new EventOption(this, FeignAgreement, InitialOptionKey("FEIGN_AGREEMENT")).ThatDoesDamage((int)base.DynamicVars["HpLoss"].BaseValue),
    };

    private async Task Expose()
    {
        List<CardModel> upgradable = base.Owner.Deck.Cards.Where(static c => c.IsUpgradable).ToList();
        for (int i = 0; i < (int)base.DynamicVars["UpgradeCount"].BaseValue && upgradable.Count > 0; i++)
        {
            CardModel card = base.Rng.NextItem(upgradable);
            upgradable.Remove(card);
            CardCmd.Upgrade(card, CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
        }
        SetEventFinished(PageDescription("EXPOSED"));
    }

    private async Task FeignAgreement()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature,
            base.DynamicVars["HpLoss"].BaseValue, DamageProps.cardHpLoss, (Creature?)null);
        List<CardModel> removed = (await CardSelectCmd.FromDeckForRemoval(
            base.Owner, new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1))).ToList();
        await CardPileCmd.RemoveFromDeck(removed);
        SetEventFinished(PageDescription("FEIGNED"));
    }
}
