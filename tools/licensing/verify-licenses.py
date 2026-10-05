from pathlib import Path
import argparse
import hashlib
import json
import re
from urllib.parse import unquote

root = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser()
parser.add_argument('--published', action='store_true')
args = parser.parse_args()
errors = []

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def rooted(path, base=''):
    value = str(path).replace('\\', '/')
    candidate = root / base / value if base and not value.startswith('licenses/') else root / value
    candidate = candidate.resolve()
    if not candidate.is_relative_to(root):
        raise ValueError(f'路径超出项目目录：{path}')
    return candidate

baseline = json.loads((root / 'tools/licensing/original-license-hashes.json').read_text(encoding='utf-8-sig'))
for item in baseline:
    path = rooted(item['path'])
    if not path.is_file() or sha(path).lower() != item['sha256'].lower():
        errors.append(f'原文发生变化：{item["path"]}')

record_count = 0
translation_count = 0
recorded_paths = set()
for ledger in sorted((root / 'licenses').rglob('sources.json')):
    data = json.loads(ledger.read_text(encoding='utf-8-sig'))
    for item in data.get('files', []) + data.get('entries', []):
        record_count += 1
        path = rooted(item['path'], data.get('pathBase', ''))
        recorded_paths.add(path)
        if not path.is_file():
            errors.append(f'来源记录文件缺失：{item["path"]}')
            continue
        actual_sha = sha(path)
        if actual_sha.lower() != item.get('sha256', '').lower():
            errors.append(f'来源记录哈希不符：{item["path"]}')
        expected_bytes = item.get('byteLength', item.get('bytes'))
        if expected_bytes is not None and path.stat().st_size != expected_bytes:
            errors.append(f'来源记录长度不符：{item["path"]}')
        if '.zh-CN' in path.name:
            translation_count += 1
            text = path.read_text(encoding='utf-8-sig')
            if not re.search('[\u4e00-\u9fff]', text):
                errors.append(f'译文缺少中文：{item["path"]}')
            original = item.get('translationOf', item.get('sourcePath'))
            original_sha = item.get('originalSha256', item.get('sourceSha256'))
            if original:
                source = rooted(original, data.get('pathBase', ''))
                if not source.is_file():
                    errors.append(f'译文对应原文缺失：{item["path"]} -> {original}')
                elif original_sha and sha(source).lower() != original_sha.lower():
                    errors.append(f'译文对应原文哈希不符：{item["path"]}')

actual_translations = list((root / 'licenses').rglob('*.zh-CN*')) + [root / 'LICENSE.zh-CN']
for path in actual_translations:
    if path.resolve() not in recorded_paths:
        errors.append(f'中文译文未登记来源：{path.relative_to(root)}')
    text = path.read_text(encoding='utf-8-sig')
    if '\ufffd' in text:
        errors.append(f'中文译文存在乱码替代字符：{path.relative_to(root)}')

docs = [root / 'README.md', root / 'CONTRIBUTING.md', root / 'licenses/README.md']
docs += list((root / 'licenses').rglob('README.md'))
docs += list((root / 'licenses').rglob('*.zh-CN.md'))
for doc in dict.fromkeys(docs):
    text = doc.read_text(encoding='utf-8-sig')
    for target in re.findall(r'\]\(([^)]+)\)', text):
        target = target.strip().split(' "', 1)[0].strip('<>')
        if re.match(r'^[a-z]+:', target) or target.startswith('#'):
            continue
        target = unquote(target.split('#', 1)[0])
        if target and not (doc.parent / target).exists():
            errors.append(f'本地链接不存在：{doc.relative_to(root)} -> {target}')

for name in ['NOTICE', 'THIRD-PARTY-NOTICES.txt']:
    text = ' '.join((root / name).read_text(encoding='utf-8-sig').split())
    for attribution in [
        'This software uses FreeType and is based in part on the work of the FreeType Team.',
        'This software is based in part on the work of the Independent JPEG Group.',
    ]:
        if attribution not in text:
            errors.append(f'必需署名缺失：{name}')

license_files = [root / name for name in ['LICENSE', 'LICENSE.zh-CN', 'NOTICE', 'THIRD-PARTY-NOTICES.txt', 'DISTRIBUTION-TERMS.txt']]
license_files += sorted(path for path in (root / 'licenses').rglob('*') if path.is_file())
if args.published:
    for source in license_files:
        relative = source.relative_to(root)
        target = root / 'artifacts/single-file' / relative
        if not target.is_file() or sha(source) != sha(target):
            errors.append(f'发行资料不同步：{relative}')

report = {
    '原文保持不变的核对数量': len(baseline),
    '来源记录核对数量': record_count,
    '来源记录中的中文译文数量': translation_count,
    '实际中文译文文件数量': len(actual_translations),
    '发行资料文件数量': len(license_files),
    '已核对发布目录': args.published,
    '错误': errors,
}
report_path = root / 'artifacts/license-checks/license-verification.json'
report_path.parent.mkdir(parents=True, exist_ok=True)
report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(json.dumps(report, ensure_ascii=False, indent=2))
raise SystemExit(1 if errors else 0)
