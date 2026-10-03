using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 加固运输(罕见技能,1费):获得3点格挡;每持有100金币,额外消耗6金币并获得+6点格挡(升级后消耗降为4,裁定F2:原案基础=6/升级=4)
/// (三件套按打出时金币先算,再扣金币——先算后耗)。
/// 升级:每百金币的消耗4→6(按 spec 升级变化列字面实现,格挡份额不变,见批次 notes)。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class ReinforcedConvoy : DehyaCardBase
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(3m),
        new CalculationExtraVar(6m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(
            static (card, _) => card.Owner is null ? 0m : (decimal)(card.Owner.Gold / 100)),
        new DynamicVar("GoldCost", 6m),
    };

    public ReinforcedConvoy()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal block = base.DynamicVars.CalculatedBlock.Calculate(target: null); // 先算:按当前金币取格挡
        int shares = base.Owner.Gold / 100;
        if (shares > 0)
        {
            await PlayerCmd.LoseGold(base.DynamicVars["GoldCost"].BaseValue * shares, base.Owner, GoldLossType.Spent); // 后耗
        }
        await CreatureCmd.GainBlock(base.Owner.Creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["GoldCost"].UpgradeValueBy(-2m);
    }
}
