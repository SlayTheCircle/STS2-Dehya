using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Characters;

namespace DehyaMod.Content.Characters;

/// <summary>
/// 角色资产档案(美术轮 2026-10-04):战斗场景/图集 tscn 与卡牌拖尾仍借原版铁甲
/// (动画模块未建,裁定 J4/J5);头像/描边/地图标记/选人半身/锁定版与联机手势×4 已换自有派生资产
/// (prep-art 的 art/characters.sh 产出)。选人背景与图标 tscn 槽位待场景轮接入。
/// </summary>
internal static class DehyaCharacterAssets
{
    public static void Register()
    {
        CharacterAssetProfile fallback = CharacterAssetProfiles.Ironclad();
        CharacterUiAssetSet ui = fallback.Ui!;
        CharacterAssetProfile profile = new CharacterAssetProfile(
            fallback.Scenes,
            new CharacterUiAssetSet(
                "res://STS2-Dehya/images/characters/dehya_character_icon.png",
                "res://STS2-Dehya/images/characters/dehya_character_icon_outline.png",
                ui.IconPath,
                ui.CharacterSelectBgPath,
                "res://STS2-Dehya/images/characters/dehya_select.png",
                "res://STS2-Dehya/images/characters/dehya_select_locked.png",
                // 开局转场材质:留空会按条目名推导 mod 下不存在的路径直接炸开局(模板实测教训),先指通用淡入淡出。
                "res://materials/transitions/fade_transition_mat.tres",
                "res://STS2-Dehya/images/characters/dehya_map_marker.png"),
            fallback.Vfx,
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
