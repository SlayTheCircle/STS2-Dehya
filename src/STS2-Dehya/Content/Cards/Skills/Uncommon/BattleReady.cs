using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Powers;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 整装待发(罕见 X 费技能):获得 4 点格挡 X 次;下个回合获得 X 点力量(自定义可见
/// DelayedStrengthPower,回合开始转正为原生力量后消失)和 X-1 点能量(原生 EnergyNextTurnPower,
/// 至少 0,X=0 时仍可打出但无收益)。升级:格挡 4→5/次,下回合能量 X-1→X。
/// X 费范式 = HasEnergyCostX + 构造费 0 + ResolveEnergyXValue(vanilla Whirlwind/ExampleXGuard)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BattleReady : DehyaCardBase
{
    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(4m, ValueProp.Move) };

    public BattleReady()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue();
        for (int i = 0; i < x; i++)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        }
        if (x > 0)
        {
            await PowerCmd.Apply<DelayedStrengthPower>(choiceContext, base.Owner.Creature, x, base.Owner.Creature, this);
            int energy = IsUpgraded ? x : System.Math.Max(0, x - 1);
            if (energy > 0)
            {
                await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, base.Owner.Creature, energy, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(1m);
    }
}
