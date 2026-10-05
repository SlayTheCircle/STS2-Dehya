using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 燃血成灰效果:己方回合内你每回合首次失去生命时,从抽牌堆**手动选择** 1 张牌消耗,
/// 并恢复 Amount 生命(抽牌堆为空时只回血)。观察口径同 Rupture(仅己方回合)。
/// 「每回合首次」用回合计数戳判定,不依赖 BeforeSideTurnStart 重置钩子的模型遍历顺序。
/// </summary>
[RegisterPower]
public sealed class BloodToAshesPower : DehyaPowerBase
{
    private int _lastTriggeredTurn = -1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || result.UnblockedDamage <= 0 || base.CombatState.CurrentSide != base.Owner.Side)
        {
            return;
        }
        if (choiceContext is ThrowingPlayerChoiceContext)
        {
            // 防御(F8 事故 2026-10-05:受限上下文弹选牌 UI 抛 NotImplementedException 杀死回合计程;
            // 根因已在触发源修复——灼热形态改挂 BeforeSideTurnStart 传引擎的 HookPlayerChoiceContext)。
            // 若仍有受限路径抵达此处,整次跳过——绝不退化为随机烧牌(维护者裁定:不可控烧牌=负面效果)。
            return;
        }
        if (base.Owner.Player.PlayerCombatState.TurnNumber == _lastTriggeredTurn)
        {
            return;
        }
        _lastTriggeredTurn = base.Owner.Player.PlayerCombatState.TurnNumber;
        Flash();
        CardPile drawPile = PileType.Draw.GetPile(base.Owner.Player);
        if (!drawPile.IsEmpty)
        {
            CardModel? selected = (await CardSelectCmd.FromCombatPile(
                choiceContext, drawPile, base.Owner.Player, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1))).FirstOrDefault();
            if (selected != null)
            {
                await CardCmd.Exhaust(choiceContext, selected);
            }
        }
        await CreatureCmd.Heal(base.Owner, base.Amount);
    }
}
