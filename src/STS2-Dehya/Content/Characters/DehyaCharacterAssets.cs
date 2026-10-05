using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Characters;

namespace DehyaMod.Content.Characters;

/// <summary>
/// 角色资产档案(多形态立绘轮 2026-10-05,Charlotte 无 Spine 路线):战斗形象/商店/休息点/选人背景/
/// 图标/卡牌拖尾/能量计均为自建场景——角色表现层至此全自有。
/// 战斗姿势切换由 DehyaPosePatch 接管(站/剑斩/挥拳/技能/受击五态,裁定 E8/J4)。
/// </summary>
internal static class DehyaCharacterAssets
{
    public static void Register()
    {
        CharacterAssetProfile fallback = CharacterAssetProfiles.Ironclad();
        CharacterAssetProfile profile = new CharacterAssetProfile(
            new CharacterSceneAssetSet(
                "res://STS2-Dehya/scenes/characters/dehya_character.tscn",
                "res://STS2-Dehya/scenes/combat/dehya_energy_counter.tscn",
                "res://STS2-Dehya/scenes/characters/dehya_merchant.tscn",
                "res://STS2-Dehya/scenes/characters/dehya_rest_site.tscn"),
            new CharacterUiAssetSet(
                "res://STS2-Dehya/images/characters/dehya_character_icon.png",
                "res://STS2-Dehya/images/characters/dehya_character_icon_outline.png",
                "res://STS2-Dehya/scenes/characters/dehya_icon.tscn",
                "res://STS2-Dehya/scenes/characters/dehya_char_select_bg.tscn",
                "res://STS2-Dehya/images/characters/dehya_select.png",
                "res://STS2-Dehya/images/characters/dehya_select_locked.png",
                // 开局转场材质:留空会按条目名推导 mod 下不存在的路径直接炸开局(模板实测教训),先指通用淡入淡出。
                "res://materials/transitions/fade_transition_mat.tres",
                "res://STS2-Dehya/images/characters/dehya_map_marker.png"),
            // 出牌轨迹:炽焰金橙拖尾(焰色渐变 + 暖白芯 + 火花),场景结构同 Navia。
            new CharacterVfxAssetSet("res://STS2-Dehya/scenes/vfx/dehya_card_trail.tscn"),
            null,
            null,
            // 联机手势(裁定 E3:猜拳/ 四图 = 多人手势资产;「指」=选遗物指向)。
            new CharacterMultiplayerAssetSet(
                "res://STS2-Dehya/images/hands/dehya_hand_pointing.png",
                "res://STS2-Dehya/images/hands/dehya_hand_rock.png",
                "res://STS2-Dehya/images/hands/dehya_hand_paper.png",
                "res://STS2-Dehya/images/hands/dehya_hand_scissors.png"));
        string entry = ModContentRegistry.GetCompoundId(ModEntry.ModId, "character", nameof(Dehya)).ToLowerInvariant();
        ModContentRegistry.For(ModEntry.ModId).RegisterCharacterAssetReplacement(entry, profile);
    }
}
