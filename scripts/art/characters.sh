# 角色本体资产:本轮只派生「有代码消费方」的槽位(Navia characters.sh 范式裁剪)。
# 六态立绘(站立/技能/受击/倒下/商店/火堆)与剑斩/挥拳攻击动作属动画模块输入,
# 场景(.tscn)就绪前不派生入包(裁定 J4);选人背景为 tscn 槽,同样待场景轮(J5)。
mkdir -p "$DST/characters" "$DST/hands"
# 头像 128² + 程序化描边 + 地图标记(母版一图三用:图片/头像兼用地图标记.png)
convert "$SRC/图片/头像兼用地图标记.png" -resize 128x128 "$DST/characters/dehya_character_icon.png"
convert "$DST/characters/dehya_character_icon.png" -bordercolor none -border 4 -alpha extract \
    -morphology Dilate Disk:3 -gravity center -crop 128x128+0+0 +repage "$TMP/iconmask.png"
convert -size 128x128 xc:'#f4cf70' "$TMP/iconmask.png" -alpha off -compose CopyOpacity \
    -composite "$DST/characters/dehya_character_icon_outline.png"
convert "$SRC/图片/头像兼用地图标记.png" -resize 128x128 "$DST/characters/dehya_map_marker.png"
# 选人半身:透明人物母版等比缩至宽 264 + 灰阶锁定版;原始小图保持不变。
convert "$SRC/图片/选人半身.png" -resize 264x "$DST/characters/dehya_select.png"
convert "$DST/characters/dehya_select.png" -modulate 60,0,100 "$DST/characters/dehya_select_locked.png"
# 联机手势 ×4(猜拳/:石头/剪刀/布/指 → rock/scissors/paper/pointing,母版 2:3 等比派生为 341×512)
convert "$SRC/猜拳/石头.png" -resize 512x512 "$DST/hands/dehya_hand_rock.png"
convert "$SRC/猜拳/剪刀.png" -resize 512x512 "$DST/hands/dehya_hand_scissors.png"
convert "$SRC/猜拳/布.png" -resize 512x512 "$DST/hands/dehya_hand_paper.png"
convert "$SRC/猜拳/指.png" -resize 512x512 "$DST/hands/dehya_hand_pointing.png"
echo '角色资产: 头像/描边/标记 + 选人/锁定 + 联机手×4 就绪'
