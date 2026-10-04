# 事件肖像:mappings.sh 的 EVENTS 映射 → images/events/<类名>.png(1672×941 cover)。
# 事件图窗与选人背景同规格(assets.md);母版已是 1672×941 时为等比直拷。
mkdir -p "$DST/events"
for path in "${!EVENTS[@]}"; do
    cls="${EVENTS[$path]}"
    convert "$SRC/$path" -resize 1672x941^ -gravity center -extent 1672x941 "$DST/events/$cls.png"
done
echo "事件肖像: ${#EVENTS[@]} 张"
