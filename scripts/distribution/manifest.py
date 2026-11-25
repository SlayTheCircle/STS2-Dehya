"""让平铺安装目录声明实际内容 DLL 的编译目标；不改源码清单。"""
import argparse
import json
from pathlib import Path
import sys


# 游戏版本按最低到最高排列。值为对应的 RitsuLib compat。
TARGET_COMPAT = {
    '0.111.1': '0.111.0',
    '0.114.0': '0.114.0',
}


def for_target(source: dict, target: str) -> dict:
    if target not in TARGET_COMPAT:
        raise ValueError(f'不支持的游戏目标: {target}')
    return {**source, 'min_game_version': target}


def write(source: Path, destination: Path, target: str) -> None:
    if source.resolve() == destination.resolve():
        raise ValueError('输出不得覆盖源码清单')
    manifest = for_target(json.loads(source.read_text(encoding='utf-8')), target)
    destination.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')


def main() -> None:
    if sys.argv[1:] == ['--package-targets']:
        # 先构建新版，最终平铺安装目录及 Loader 使用较低版本。
        for target, compat in reversed(TARGET_COMPAT.items()):
            print(target, compat)
        return
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('source', type=Path)
    parser.add_argument('destination', type=Path)
    parser.add_argument('target', choices=TARGET_COMPAT)
    args = parser.parse_args()
    try:
        write(args.source, args.destination, args.target)
    except ValueError as error:
        parser.error(str(error))


if __name__ == '__main__':
    main()
