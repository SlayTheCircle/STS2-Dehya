using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 燃血成灰效果:己方回合内你每回合首次失去生命时,从抽牌堆**手动选择**(可取消) 1 张牌消耗,
/// 并恢复 Amount 生命(抽牌堆为空或玩家取消时只回血)。观察口径同 Rupture(仅己方回合)。
/// 「每回合首次」用回合计数戳判定,不依赖回合钩子的模型遍历顺序。
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
            // 根因已在触发源修复——灼热形态现挂 BeforeHandDraw 抽牌管线钩子,引擎传入合法上下文)。
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
            // F8 二次修复(2026-10-06 裁定 E/F,维护者「超标就超标到底」):
            // ① 免弹窗快捷路径在 num<=MinSelect 且未要求手动确认时会静默全选自动烧牌(CardSelectCmd 实证),
            //    构造器对 (0,1) 自动 RequireManualConfirmation=true——抽牌堆恰 1 张也必弹框;
            // ② 「可控烧牌」含拒绝权:战斗选牌框的跳过机制是 MinSelect==0(NCombatPileCardSelectScreen
            //    对 0 初始即启用确认键,不选直接确认=不烧、仍回血,与空堆只回血语义一致)——
            //    Cancelable 标志只被 NDeck 系卡组浏览框消费,战斗框不读(2026-10-06 实测),不可用;
            // ③ 提示词用本 Mod 专属键(card_selection.json 注入),避免原版 TO_EXHAUST「选择1张」
            //    的必选措辞误导(MinSelect=0 实为最多选 1)。
            CardSelectorPrefs prefs = new CardSelectorPrefs(
                new LocString("card_selection", "STS2_DEHYA_BLOOD_TO_ASHES"), 0, 1);
            CardModel? selected = (await CardSelectCmd.FromCombatPile(
                choiceContext, drawPile, base.Owner.Player, prefs)).FirstOrDefault();
            if (selected != null)
            {
                await CardCmd.Exhaust(choiceContext, selected);
            }
        }
        await CreatureCmd.Heal(base.Owner, base.Amount);
    }
}
