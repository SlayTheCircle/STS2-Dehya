# 角色本体资产:头像/选人/手势 + 多形态立绘套(2026-10-05 场景轮,J4/J5 解除)。
# 立绘母版 1024×1536(剑斩/倒下为横构图 1536×1024)原尺寸直入;倒下为姿势储备,暂无消费方
# (死亡另有表现,与 Charlotte 的 down 同处理);选人背景大图 1672×941 直入 tscn 槽。
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
# ---- 多形态立绘(Charlotte 无 Spine 路线):战斗五态 + 商店/火堆 + 倒下储备 + 选人背景 ----
convert "$SRC/立绘/站立.png" "$DST/characters/dehya_normal.png"
convert "$SRC/立绘/剑斩.png" "$DST/characters/dehya_attack_slash.png"
convert "$SRC/立绘/挥拳.png" "$DST/characters/dehya_attack_punch.png"
convert "$SRC/立绘/技能.png" "$DST/characters/dehya_skill.png"
convert "$SRC/立绘/受击.png" "$DST/characters/dehya_hit.png"
convert "$SRC/立绘/倒下.png" "$DST/characters/dehya_down.png"
convert "$SRC/立绘/商店.png" "$DST/characters/dehya_merchant.png"
convert "$SRC/立绘/火堆.png" "$DST/characters/dehya_rest_site.png"
convert "$SRC/图片/选人界面大图.png" "$DST/characters/dehya_char_select_bg.png"
echo '角色资产: 头像/描边/标记 + 选人/锁定 + 联机手×4 + 立绘×8 + 选人背景就绪'
