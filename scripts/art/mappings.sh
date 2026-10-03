# 母版映射表：ART_SOURCE_DIR 下完整相对路径（含扩展名）→ 实现类名。
# 不要求设计者改目录或文件名；格式由 ImageMagick 解码。右侧必须等于内容类名。
declare -A CARDS=([卡图/示例打击.png]=DehyaStrike [卡图/示例防御.png]=DehyaDefend [卡图/示例昂扬.png]=DehyaVigor)
declare -A RELICS=([遗物/示例坠饰.png]=DehyaLocket)
declare -A POTIONS=([药水/示例药剂.png]=DehyaTonic)
declare -A POWERS=([buff图标/示例昂扬.png]=DehyaVigorPower)
declare -A ENCHANTMENTS=()
ANCIENT_CARDS=""  # 空格分隔的先古卡类名,对应 606×852 图窗。
SUPPORT_BADGE_SOURCE="buff图标/支援徽记.png"  # 同样使用完整相对路径；空值表示不用共享徽记。
OUTLINE_COLOR="#f4cf70"
