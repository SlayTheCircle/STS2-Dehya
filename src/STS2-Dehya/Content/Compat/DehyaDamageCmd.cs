using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DehyaMod.Content.Compat;

/// <summary>
/// 0.107.1 稳定版垫片(其五):CreatureCmd.Damage 的 7 参重载(末位 CardPlay)为 0.111 新增,
/// 0.107.1 只有 6 参形态。统一经本助手转发,内容代码不写 #if
/// (炎啸狮咬/夜尽天明/灼眼之鬃荆棘回响的调用点)。
/// 类名避开游戏原生 MegaCrit.Sts2.Core.Commands.DamageCmd(大量卡牌在用,防 CS0104 歧义)。
/// </summary>
public static class DehyaDamageCmd
{
    public static Task<IEnumerable<DamageResult>> Damage(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
#if !MOD_GAME_0107_1
        return CreatureCmd.Damage(choiceContext, target, amount, props, dealer, cardSource, cardPlay);
#else
        _ = cardPlay;
        return CreatureCmd.Damage(choiceContext, target, amount, props, dealer, cardSource);
#endif
    }

    public static Task<IEnumerable<DamageResult>> Damage(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
#if !MOD_GAME_0107_1
        return CreatureCmd.Damage(choiceContext, targets, amount, props, dealer, cardSource, cardPlay);
#else
        _ = cardPlay;
        return CreatureCmd.Damage(choiceContext, targets, amount, props, dealer, cardSource);
#endif
    }
}
