using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using DehyaMod.Content.CardPools;

namespace DehyaMod.Content.Cards;

/// <summary>
/// 焚势掠地(罕见技能):持续抽 1 张牌,直到抽到一张费用不低于 2 的牌(按实际费用计;X 费牌费用不定,不计入)。
/// 抽牌堆抽空(含重洗后仍空)或手牌已满时自然停止。升级:费用 3→2。
/// </summary>
[RegisterCard(typeof(DehyaCardPool))]
public sealed class BlazingAdvance : DehyaCardBase
{
    public BlazingAdvance()
        : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        while (!CombatManager.Instance.IsOverOrEnding)
        {
            CardModel? card = (await CardPileCmd.Draw(choiceContext, 1m, base.Owner)).FirstOrDefault();
            if (card is null)
            {
                break;
            }
            if (!card.EnergyCost.CostsX && card.EnergyCost.GetWithModifiers(CostModifiers.All) >= 2)
            {
                break;
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
