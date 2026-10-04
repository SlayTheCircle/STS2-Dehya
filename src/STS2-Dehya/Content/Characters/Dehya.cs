using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using DehyaMod.Content.CardPools;
using DehyaMod.Content.Cards;
using DehyaMod.Content.PotionPools;
using DehyaMod.Content.RelicPools;
using DehyaMod.Content.Relics;
using DehyaMod.Content.Timeline;

namespace DehyaMod.Content.Characters;

/// <summary>
/// 迪希雅:镀金旅团的「炽鬃之狮」。基础生命值 80(设计案);
/// 资产档案由 DehyaCharacterAssets 显式借用铁甲,见 ModEntry 的接线。
/// 世界线等深层模块为可选代码(见 docs/dev/worldline.md)。
/// </summary>
[RegisterCharacter]
[UnlockEpochAfterRunAs(typeof(Dehya1Epoch))]        // 第一章·委托:完成一局迪希雅
[UnlockEpochAfterWinAs(typeof(Dehya2Epoch))]        // 第二章·水土不服:首次通关
[UnlockEpochAfterBossVictories(typeof(Dehya3Epoch), 3)] // 第三章·攀登:累计击败 3 首领
[UnlockEpochAfterAscensionOneWin(typeof(Dehya4Epoch))] // 第四章·建筑师:进阶 1 通关(vanilla 第七章同型)
public sealed class Dehya : CharacterModel
{
    public override Color NameColor => new Color("E8B23AFF");

    public override CharacterGender Gender => CharacterGender.Feminine;

    // 角色不锁定——UnlocksAfterRunAs 维持 null。
    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override int StartingHp => 80;

    public override int StartingGold => 99;

    public override CardPoolModel CardPool => ModelDb.CardPool<DehyaCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DehyaRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DehyaPotionPool>();

    // 初始卡组(设计案): 打击×4 防御×4 熔铁之拳 以守待攻。
    public override IEnumerable<CardModel> StartingDeck => new CardModel[]
    {
        ModelDb.Card<DehyaStrike>(),
        ModelDb.Card<DehyaStrike>(),
        ModelDb.Card<DehyaStrike>(),
        ModelDb.Card<DehyaStrike>(),
        ModelDb.Card<DehyaDefend>(),
        ModelDb.Card<DehyaDefend>(),
        ModelDb.Card<DehyaDefend>(),
        ModelDb.Card<DehyaDefend>(),
        ModelDb.Card<MoltenIronFist>(),
        ModelDb.Card<BracedStrike>(),
    };

    public override IReadOnlyList<RelicModel> StartingRelics => new RelicModel[] { ModelDb.Relic<StillWarmBracers>() };

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.4f;

    public override Color EnergyLabelOutlineColor => new Color("8A6210");

    public override Color DialogueColor => new Color("9A7B2D");

    public override Color MapDrawingColor => new Color("D4A017");

    public override Color RemoteTargetingLineColor => new Color("E8B23AFF");

    public override Color RemoteTargetingLineOutline => new Color("8A6210");

    // 占位:复用铁甲的切场音效,待配音接入后替换。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

#if !MOD_GAME_0107_1
    // 0.107.1 无此虚属性,其 GenerateAnimator 硬编码的默认映射与本覆写逐项相同,省略即等价。
    protected override List<(AnimState, string)> AnimationStates => new List<(AnimState, string)>
    {
        (new AnimState("attack"), "Attack"),
        (new AnimState("hurt"), "Hit"),
        (new AnimState("cast"), "Cast"),
    };
#endif

    public override List<string> GetArchitectAttackVfx()
    {
        return new List<string> { "vfx/vfx_attack_slash", "vfx/vfx_heavy_blunt", "vfx/vfx_bloody_impact" };
    }
}
