using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.Compat;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 炎啸狮咬(增益,Instanced,Amount=每次 0 费牌给予的力量):持有者每打出一张 0 费牌,
/// 获得 Amount 点真实力量并计入本实例追踪(层数显示=已追踪的力量)。
/// 回合结束时(结算敌方伤害的 BeforeSideTurnEnd 时点)先移除全部已追踪力量,
/// 再对所有敌人造成等量伤害,随后清零追踪(力量层数不落 0,能力全程存续)。
/// 多张炎啸狮咬各建独立实例、各自结算(引擎 Instanced 语义,参照原版 TheBomb)。
/// </summary>
[RegisterPower]
public sealed class BlazingLionBitePower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    /// <summary>本回合已追踪的力量值(战斗内存态,不序列化)。</summary>
    private int Tracked { get; set; }

    public override int DisplayAmount => Tracked;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner || cardPlay.Card.EnergyCost.GetResolved() != 0)
        {
            return;
        }
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        Tracked += base.Amount;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner) || Tracked <= 0)
        {
            return;
        }
        int tracked = Tracked;
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, -tracked, base.Owner, null);
        Flash();
        await DehyaDamageCmd.Damage(choiceContext, base.CombatState.HittableEnemies, tracked, ValueProp.Unpowered | ValueProp.SkipHurtAnim, base.Owner, null, null);
        Tracked = 0;
    }
}
