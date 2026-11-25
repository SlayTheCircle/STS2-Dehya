using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib;

namespace Sts2Mod.Foreseer;

// Common source shared by the independently packaged character adapters.
internal static class ForeseerCardAdapter
{
    private const string Namespace = "RandomForeseer.";
    private const BindingFlags Members = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private static IDisposable? _subscription;
    private static MethodInfo? _attack;
    private static Func<CardModel, bool>? _supports;
    private static Action<ForecastPlay>? _play;

    public static void Subscribe(string modId, Func<CardModel, bool> supports, Action<ForecastPlay> play)
    {
        _subscription ??= RitsuLibFramework.SubscribeLifecycle<DeferredInitializationCompletedEvent>(
            _ => Install(modId, supports, play), true);
    }

    private static void Install(string modId, Func<CardModel, bool> supports, Action<ForecastPlay> play)
    {
        if (_attack is not null) { return; }
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "RandomForeseer");
        if (assembly is null) { return; }
#if MOD_GAME_0114_0
        const string target = "0.17.0";
#else
        const string target = "0.13.21";
#endif
        string? version = ModManager.Mods.FirstOrDefault(m => m.manifest?.id == "RandomForeseer")?.manifest?.version;
        if (version != target) { return; }
        var harmony = new Harmony(modId + ".foreseer");
        try
        {
            Type Resolve(string name) => assembly.GetType(Namespace + name, throwOnError: true)!;
            var simulator = Resolve("InCombat.Simulation.CombatPredictionSimulator");
            var predictedCard = Resolve("Common.PredictedCard");
            var onPlay = Resolve("InCombat.Mirrors.Cards.OnPlay.CardOnPlayMirrors");
            var forecast = Resolve("InCombat.CombatCardPrediction");
            var attack = Require(simulator, "ExecuteAttack", typeof(AttackCommand));
            var invoke = Require(onPlay, "Invoke", simulator, predictedCard, typeof(CardPlay));
            var canMirror = Require(onPlay, "CanMirror", typeof(CardModel));
            var predict = Require(forecast, "Predict", typeof(CardModel), typeof(Creature));
            _supports = supports;
            _play = play;
            _attack = attack;
            harmony.Patch(invoke, prefix: new(typeof(ForeseerCardAdapter), nameof(PlayPrefix)));
            harmony.Patch(canMirror, prefix: new(typeof(ForeseerCardAdapter), nameof(CanMirror)));
            harmony.Patch(predict, finalizer: new(typeof(ForeseerCardAdapter), nameof(DiscardUnavailable)));
            Log.Info($"[{modId}] Optional Random Foreseer adapter active: {version}.");
        }
        catch (Exception error)
        {
            harmony.UnpatchAll(harmony.Id);
            _attack = null;
            _supports = null;
            _play = null;
            Log.Warn($"[{modId}] Random Foreseer adapter disabled after protocol binding failure: {error}");
        }
    }

    private static MethodInfo Require(Type type, string name, params Type[] arguments)
        => type.GetMethod(name, Members, null, arguments, null) ?? throw new MissingMethodException(type.FullName, name);

    private static bool CanMirror(CardModel card, ref bool __result)
    {
        if (!_supports!(card)) { return true; }
        __result = true;
        return false;
    }

    private static bool PlayPrefix(object simulator, CardPlay cardPlay)
    {
        if (!_supports!(cardPlay.Card)) { return true; }
        _play!(new ForecastPlay(simulator, cardPlay, _attack!));
        return false;
    }

    private static Exception? DiscardUnavailable(Exception? __exception, ref object? __result)
    {
        for (var error = __exception; error is not null; error = error.InnerException)
        {
            if (error is not ForecastUnavailable) { continue; }
            __result = null;
            return null;
        }
        return __exception;
    }
}
