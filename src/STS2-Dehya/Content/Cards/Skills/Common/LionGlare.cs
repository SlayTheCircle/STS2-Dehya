using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 烈狮怒瞳(普通技能,0费):每有一名敌人持有攻击意图,获得4点格挡并抽1张牌(裁定:随敌数叠);
/// 若没有敌人持有攻击意图,改为获得3层再生,且此牌打出后消耗(OnPlay 内直接 CardCmd.Exhaust,非常驻关键词)。
/// 升级:每次格挡4→5点,再生3→4层。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class LionGlare : DehyaCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4m, ValueProp.Move),
        new PowerVar<RegenPower>(3m),
    };

    public LionGlare()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int attackers = base.CombatState?.HittableEnemies.Count((Creature e) => e.Monster?.IntendsToAttack ?? false) ?? 0;
        if (attackers > 0)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue * attackers, base.DynamicVars.Block.Props, cardPlay);
            await CardPileCmd.Draw(choiceContext, attackers, base.Owner);
        }
        else
        {
            await PowerCmd.Apply<RegenPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RegenPower"].BaseValue, base.Owner.Creature, this);
            // 消耗须在 OnPlay 内直接执行:引擎 OnPlayWrapper 在调 OnPlay 之前就缓存了结果牌堆,
            // 此处置 ExhaustOnNextPlay 对本次打出已无效(标志读取点已过),卡会落回弃牌堆(2026-10-06 审查修正)。
            await CardCmd.Exhaust(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(1m);
        base.DynamicVars["RegenPower"].UpgradeValueBy(1m);
    }
}
