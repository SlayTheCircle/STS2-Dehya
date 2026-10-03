using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 陷阵之志(罕见攻击):造成 14 点伤害。若此伤害击败敌人,对一名随机存活敌人免费再打出这张牌
/// (可连续触发;CardCmd.AutoPlay 免能量,目标传 null 由引擎从 HittableEnemies 随机选取,
/// 复打发生在本次 OnPlay 内,卡牌后续出牌结算按所在牌堆自然收敛,可连锁裁定即此)。升级:伤害 14→26。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class LineBreaker : DehyaCardBase
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(14m, ValueProp.Move) };

    public LineBreaker()
        : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        bool killed = (await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext)).Results.SelectMany(static r => r).Any(static r => r.WasTargetKilled);
        if (killed && base.CombatState?.HittableEnemies.Any() == true)
        {
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(12m);
    }
}
