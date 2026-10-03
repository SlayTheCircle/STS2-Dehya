using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DehyaMod.Content.RelicPools;

namespace DehyaMod.Content.Relics;

/// <summary>
/// 炽热燃烧的臂铠(进阶初始遗物):每场战斗开始时,获得7层[覆甲],获得2层[再生]。
/// 由欧洛巴斯之触替换尚有余温的臂铠获得,不作普通掉落(清点报告 noncard-04 的实现裁定:
/// 采用 RitsuLib 先古升级映射,获取时就地替换)。
/// </summary>
[RegisterRelic(typeof(DehyaRelicPool))]
public sealed class BlazingBracers : DehyaRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Starter; // 进阶初始遗物不入掉落池

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 7m, base.Owner.Creature, null);
        await PowerCmd.Apply<RegenPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 2m, base.Owner.Creature, null);
    }
}
