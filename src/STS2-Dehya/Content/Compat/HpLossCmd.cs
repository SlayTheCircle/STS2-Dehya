using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DehyaMod.Content.Compat;

/// <summary>
/// 0.107.1 稳定版垫片(其四):卡牌「失去生命」代价走 CreatureCmd.Damage(Unblockable|Unpowered|Move,
/// 原版 Bloodletting 口径),0.111 的重载多一个 CardPlay 参数。统一经本助手转发,内容代码不写 #if
/// (格挡猛击等卡牌的调用点)。
/// </summary>
public static class HpLossCmd
{
    public static Task LoseHpFromCard(PlayerChoiceContext choiceContext, Creature creature, decimal amount, CardModel card, CardPlay? cardPlay)
    {
#if !MOD_GAME_0107_1
        return CreatureCmd.Damage(choiceContext, creature, amount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, card, cardPlay);
#else
        _ = cardPlay;
        return CreatureCmd.Damage(choiceContext, creature, amount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, card);
#endif
    }
}
