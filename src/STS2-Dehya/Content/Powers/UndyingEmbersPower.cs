using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using DehyaMod.Content.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 余焰不息(增益):持有者打出费用不低于 2 的牌后,从本 Mod 卡池(DehyaCardPool)中
/// 随机挑一张 0 费牌置入其弃牌堆。
/// </summary>
[RegisterPower]
public sealed class UndyingEmbersPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Card.EnergyCost.GetResolved() < 2)
        {
            return;
        }
        var player = base.Owner.Player;
        if (player is null)
        {
            return;
        }
        List<CardModel> candidates = ModelDb.CardPool<DehyaCardPool>()
            .AllCards
            .Where(card => card.EnergyCost.Canonical == 0)
            .ToList();
        CardModel? pick = player.RunState.Rng.CombatCardGeneration.NextItem(candidates);
        if (pick is null)
        {
            return;
        }
        Flash();
        CardModel instance = base.CombatState.CreateCard(pick, player);
        await CardPileCmd.AddGeneratedCardToCombat(instance, PileType.Discard, player);
    }
}
