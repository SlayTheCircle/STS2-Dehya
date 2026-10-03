using System;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using DehyaMod.Content.Compat;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Patches;

/// <summary>
/// 灼眼之鬃的荆棘回响:postfix 原版 ThornsPower.BeforeDamageReceived(荆棘反击入口)。
/// 复刻原方法触发判据(受击者=荆棘持有者、有攻击来源、带攻击标记或为 Omnislice),
/// 持有者带 DazzlingManePower 时,等原反击结算完毕后对该敌人再造成一次等值同性质伤害。
/// 注意:postfix 必须 void/async void(带 Task 返回会被 Harmony 判为透传 postfix 直接炸初始化,
/// 见 STS2-Charlotte RepairToolsLoseGoldPatch 实测注释);async void 延续在游戏主循环上下文恢复。
/// </summary>
[HarmonyPatch(typeof(ThornsPower), nameof(ThornsPower.BeforeDamageReceived))]
public static class ThornsEchoPatch
{
    public static async void Postfix(ThornsPower __instance, PlayerChoiceContext choiceContext, Creature target, ValueProp props, Creature? dealer, CardModel? cardSource, Task __result)
    {
        try
        {
            if (dealer is null || target != __instance.Owner)
            {
                return;
            }
            if (!props.IsPoweredAttack() && cardSource is not Omnislice)
            {
                return;
            }
            if (!__instance.Owner.HasPower<DazzlingManePower>())
            {
                return;
            }
            decimal echo = __instance.Amount;
            await __result;
            if (echo <= 0m || !CombatManager.Instance.IsInProgress || dealer.IsDead)
            {
                return;
            }
            await DehyaDamageCmd.Damage(choiceContext, dealer, echo, ValueProp.Unpowered | ValueProp.SkipHurtAnim, __instance.Owner, null, null);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[STS2-Dehya] 灼眼之鬃荆棘回响失败: {e}");
        }
    }
}
