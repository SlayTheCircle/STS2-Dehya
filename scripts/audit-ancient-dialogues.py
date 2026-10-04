#!/usr/bin/env python3
"""先古对话键完整性:本角色对话全 r 轮播、行号连续、next 逐行、双语一致。

漏 r 的专属轮在该轮次精确匹配后永久绝迹(违背「无论多少次游玩都有机会遇到」的产品要求);
漏 next 会让界面角落回显键名原文;同轮 r 混合会被原版 PopulateLines 直接抛异常。
"""
import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import modmeta

ROOT = Path(__file__).resolve().parent.parent
PREFIX = modmeta.loc_prefix(ROOT)

errors = []
tables = {}
for lang in ('zhs', 'eng'):
    path = ROOT / f'localization/{lang}/ancients.json'
    tables[lang] = json.loads(path.read_text(encoding='utf-8')) if path.exists() else None
    if tables[lang] is None:
        errors.append(f'[{lang}] 缺 localization/{lang}/ancients.json')

# 角色条目键从 characters 表推导(本仓单角色,须恰有一个)。
entries = set()
characters = json.loads((ROOT / 'localization/zhs/characters.json').read_text(encoding='utf-8'))
for key in characters:
    m = re.match(rf'^{re.escape(PREFIX)}CHARACTER_([A-Z0-9_]+)\.title$', key)
    if m:
        entries.add(m.group(1))
if len(entries) != 1:
    errors.append(f'角色条目键不唯一: {sorted(entries)}')
    char = None
else:
    char = f'{PREFIX}CHARACTER_{entries.pop()}'

LINE = re.compile(r'^([A-Z_]+)\.talk\.([A-Z_0-9]+)\.(\d+)-(\d+)(r?)\.(ancient|char)$')
NEXT = re.compile(r'^([A-Z_]+)\.talk\.([A-Z_0-9]+)\.(\d+)-(\d+)(r?)\.next$')
line_count = 0

for lang, table in tables.items():
    if table is None or char is None:
        continue
    lines, nexts = {}, {}
    for key in table:
        m = LINE.match(key)
        if m:
            lines[(m[1], m[2], int(m[3]), int(m[4]))] = bool(m[5])
            continue
        m = NEXT.match(key)
        if m:
            nexts[(m[1], m[2], int(m[3]), int(m[4]))] = bool(m[5])
    mine = {k: v for k, v in lines.items() if k[1] == char}
    line_count = max(line_count, len(mine))
    # 1) 本角色对话行必须全部带 r(轮播池;不带 r 的专属轮拜访推进后永久绝迹)。
    for key, has_r in sorted(mine.items()):
        if not has_r:
            errors.append(f'[{lang}] 对话行未标 r: {key[0]} 轮{key[2]} 行{key[3]}')
    # 2) 行号从零连续;同轮 r 标记一致(混合即抛异常)。
    by_round = {}
    for (anc, _, dg, ln), has_r in mine.items():
        by_round.setdefault((anc, dg), {})[ln] = has_r
    for (anc, dg), lns in sorted(by_round.items()):
        if sorted(lns) != list(range(len(lns))):
            errors.append(f'[{lang}] 行号不连续: {anc} 轮{dg} -> {sorted(lns)}')
        if len(set(lns.values())) > 1:
            errors.append(f'[{lang}] 同轮 r 标记不一致: {anc} 轮{dg}')
        # 3) 除末行外逐行 next;next 的 r 须与所在行一致。
        for j in sorted(lns)[:-1]:
            nk = (anc, char, dg, j)
            if nk not in nexts:
                errors.append(f'[{lang}] 缺 next 键: {anc} {dg}-{j}(界面会回显键名)')
            elif nexts[nk] != lns[j]:
                errors.append(f'[{lang}] next 与行 r 不一致: {anc} {dg}-{j}')

# 4) 双语键集一致。
if tables.get('zhs') is not None and tables.get('eng') is not None:
    for extra in sorted(set(tables['zhs']) - set(tables['eng'])):
        errors.append(f'双语缺 eng: {extra}')
    for extra in sorted(set(tables['eng']) - set(tables['zhs'])):
        errors.append(f'双语缺 zhs: {extra}')

for error in errors:
    print('错误:', error)
if errors:
    raise SystemExit(1)
print(f'先古对话审计通过({line_count} 行 × zhs/eng,全 r 轮播)')
