using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Sts2Mod.Foreseer;

internal sealed class ForecastUnavailable : Exception;

internal sealed class ForecastPlay(object simulator, CardPlay play, MethodInfo executeAttack)
{
    public CardModel Card => play.Card;
    private object State => Read(simulator, "State");
    public void RequireSinglePlayWithOriginalHealth()
    {
        var creature = Card.Owner.Creature;
        object simulated = Call(State, "GetCreature", creature) ?? throw new ForecastUnavailable();
        if (play.IsAutoPlay || play.PlayCount != 1 || (int)Read(simulated, "CurrentHp") != creature.CurrentHp)
        {
            throw new ForecastUnavailable();
        }
    }

    public int Count(string name) => Card.DynamicVars[name].IntValue;

    public void Attack(int hits, Creature? target = null)
    {
        if (hits <= 0) { return; }
        target ??= play.Target ?? throw new ForecastUnavailable();
        var command = DamageCmd.Attack(Card.DynamicVars.Damage.BaseValue).FromCard(Card, play)
            .WithHitCount(hits).Targeting(target);
        executeAttack.Invoke(simulator, [command]);
    }

    public void ExhaustAndAttack(Func<CardModel, bool> filter)
    {
        var player = Call(State, "GetPlayerCombatState", Card.Owner)!;
        object[] Cards(string name) => ((IEnumerable)Read(Read(player, name), "Cards")).Cast<object>().ToArray();
        var cards = Cards("DrawPile").Concat(Cards("DiscardPile"))
            .Where(card => filter((CardModel)Read(card, "MutablePreview"))).ToArray();
        var rng = (Rng)Read(Read(simulator, "Rng"), "CombatTargets");
        foreach (var card in cards)
        {
            Call(simulator, "Exhaust", card, false);
            var targets = (IReadOnlyList<Creature>)Read(State, "HittableEnemies");
            if (targets.Count > 0) { Attack(1, rng.NextItem(targets)); }
        }
    }

    private static object Read(object source, string property)
        => source.GetType().GetProperty(property)?.GetValue(source) ?? throw new ForecastUnavailable();

    private static object? Call(object source, string method, params object[] arguments)
    {
        var selected = source.GetType().GetMethods().SingleOrDefault(m => m.Name == method
            && !m.IsGenericMethod && m.GetParameters().Length == arguments.Length
            && m.GetParameters().Zip(arguments).All(p => p.First.ParameterType.IsInstanceOfType(p.Second)));
        return (selected ?? throw new ForecastUnavailable()).Invoke(source, arguments);
    }
}
