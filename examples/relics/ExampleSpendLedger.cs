using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.RelicPools;
using DehyaMod.Content.Relics;

namespace DehyaMod.Examples.Relics;

[RegisterRelic(typeof(DehyaRelicPool))]
public sealed class ExampleSpendLedger : DehyaRelicBase
{
    private int _goldSpent;
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override bool ShowCounter => true;
    public override int DisplayAmount => GoldSpent;

    [SavedProperty]
    public int GoldSpent
    {
        get => _goldSpent;
        set
        {
            AssertMutable();
            _goldSpent = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterItemPurchased(Player player, MerchantEntry itemPurchased, int goldSpent)
    {
        if (player == base.Owner && goldSpent > 0)
            GoldSpent += goldSpent;
        return Task.CompletedTask;
    }
}
