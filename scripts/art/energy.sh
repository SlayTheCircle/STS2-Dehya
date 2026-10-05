# 能量球派生:火花/辉光为程序化白色柔光(颜色由场景调制),先供卡牌拖尾消费;
# big/text 两枚费用图标待母版 图片/能量球.png(生图工作单 v2 ③)交付后同脚本自动接入。
mkdir -p "$DST/energy"
convert -size 64x64 xc:white -alpha set \
    -channel A -fx 'max(0,1-hypot(i-31.5,j-31.5)/31.5)^2' +channel \
    "$DST/energy/dehya_spark.png"
convert -size 128x128 xc:white -alpha set \
    -channel A -fx 'max(0,1-hypot(i-63.5,j-63.5)/63.5)^2' +channel \
    "$DST/energy/dehya_glow.png"
if [[ -f "$SRC/图片/能量球.png" ]]; then
    convert "$SRC/图片/能量球.png" -resize 256x256 "$DST/energy/dehya_energy_big.png"
    convert "$DST/energy/dehya_energy_big.png" -resize 24x24 "$DST/energy/dehya_energy_text.png"
    echo '能量球: big 256² + text 24² + 火花/辉光就绪'
else
    echo '能量球: 火花/辉光就绪;big/text 待母版 图片/能量球.png'
fi
