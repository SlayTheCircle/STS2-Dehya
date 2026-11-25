using DehyaMod.Content.Cards;
using DehyaMod.Content.Powers;

namespace Sts2Mod.Foreseer;

internal static class DehyaForecasts
{
    public static void Subscribe(string modId)
        => ForeseerCardAdapter.Subscribe(modId, card => card is HeadOnSlam or BloodiedBattlefield, Play);

    private static void Play(ForecastPlay forecast)
    {
        if (forecast.Card is HeadOnSlam)
        {
            forecast.Attack(forecast.Count("Hits"));
            return;
        }
        forecast.RequireSinglePlayWithOriginalHealth();
        forecast.Attack(forecast.Card.Owner.Creature.GetPowerAmount<HpLossLedgerPower>());
    }
}
