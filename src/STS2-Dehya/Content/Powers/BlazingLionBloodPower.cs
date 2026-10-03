using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DehyaMod.Content.Powers;

/// <summary>
/// 炽鬃狮血(增益,Instanced,Amount=每差几点生命):持有者每缺失 Amount 点生命即拥有 1 点
/// 由本能力同步的真实力量;失血与恢复都会触发重算(恢复时力量回落)。
/// 多张炽鬃狮血各建独立实例、各自按自身除数结算(引擎 Instanced 语义,参照原版 TheBomb)。
/// 施加时与 HP 变化时同步;只在战斗进行中生效。
/// </summary>
[RegisterPower]
public sealed class BlazingLionBloodPower : DehyaPowerBase
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    /// <summary>已同步到真实 StrengthPower 的力量值(战斗内存态,不序列化)。</summary>
    private int Applied { get; set; }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await Sync(new ThrowingPlayerChoiceContext());
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != base.Owner)
        {
            return;
        }
        await Sync(new ThrowingPlayerChoiceContext());
    }

    private async Task Sync(PlayerChoiceContext choiceContext)
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return;
        }
        int divisor = base.Amount > 0 ? base.Amount : 1;
        int missing = base.Owner.MaxHp - base.Owner.CurrentHp;
        if (missing < 0)
        {
            missing = 0;
        }
        int target = missing / divisor;
        int diff = target - Applied;
        if (diff == 0)
        {
            return;
        }
        Applied = target;
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, diff, base.Owner, null);
    }
}
