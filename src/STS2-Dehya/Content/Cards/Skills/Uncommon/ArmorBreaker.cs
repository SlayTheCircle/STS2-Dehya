using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Compat;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 卸甲强袭(罕见技能,保留):先按当前格挡求值(每 3 点格挡折 1 点力量,通用计算变量
/// CalculatedStrength 预览感知,base 0/每份 1),再经 Compat 垫片移除全部格挡,获得对应力量;
/// 随后挂载隐藏的 ThisTurnNoBlockPower(本回合内卡牌来源的格挡清零,回合结束自动消失)。
/// 升级:每 3 点格挡折算的力量 1→2。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class ArmorBreaker : DehyaCardBase
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Retain };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedStrength").WithMultiplier(
            static (card, _) => System.Math.Floor((card.Owner?.Creature?.Block ?? 0) / 3m)),
    };

    public ArmorBreaker()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature self = base.Owner.Creature;
        // 先读后耗:力量按移除前的格挡求值。
        decimal strength = ((CalculatedVar)base.DynamicVars["CalculatedStrength"]).Calculate(null);
        await BlockCmd.LoseBlock(choiceContext, self, self.Block, self);
        if (strength > 0m)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, self, strength, self, this);
        }
        await PowerCmd.Apply<ThisTurnNoBlockPower>(choiceContext, self, 1, self, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationExtra.UpgradeValueBy(1m);
    }
}
