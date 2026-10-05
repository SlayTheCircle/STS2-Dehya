using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.RelicPools;

namespace DehyaMod.Content.Relics;

/// <summary>
/// 尚有余温的臂铠(初始遗物):每场战斗开始时,获得5层[覆甲],恢复2点生命值。
/// 欧洛巴斯之触经由 Refinement 特性将其替换为炽热燃烧的臂铠(Navia 的 RosulaEmblem 范式)。
/// </summary>
[RegisterRelic(typeof(DehyaRelicPool))]
[RegisterTouchOfOrobasRefinement(typeof(BlazingBracers))] // 欧洛巴斯之触替换(2026-10-04 核验:原版 RefinementUpgrades=BlackBlood 等进阶对,目标稀有度 Starter 不入掉落;不接会被兜底换成圆环饰)
public sealed class StillWarmBracers : DehyaRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    /// <summary>描述中的机制名词挂悬停词条(非卡牌模型无关键词管线,须显式挂;原版 Akabeko/Gorget 范式)。</summary>
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PlatingPower>(),
    };

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 5m, base.Owner.Creature, null);
        await CreatureCmd.Heal(base.Owner.Creature, 2);
    }
}
