using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DehyaMod.Content.Compat;

/// <summary>
/// 0.107.1 稳定版垫片(其三):CreatureCmd.LoseBlock 在 0.111 引入 PlayerChoiceContext 与移除者参数。
/// 静态方法无法用扩展注入,统一经本助手转发;内容代码不写 #if(卸甲强袭/准备万全等的调用点)。
/// </summary>
public static class BlockCmd
{
    public static Task LoseBlock(PlayerChoiceContext choiceContext, Creature creature, decimal amount, Creature? remover)
    {
#if !MOD_GAME_0107_1
        return CreatureCmd.LoseBlock(choiceContext, creature, amount, remover);
#else
        _ = choiceContext;
        _ = remover;
        return CreatureCmd.LoseBlock(creature, amount);
#endif
    }
}
