#!/usr/bin/env bash
# check-file-names.sh — scan for camelCase / mixed-case files that should be kebab-case.
#
# Usage:
#   bash check-file-names.sh [ROOT_DIR]     # defaults to current directory
#   bash check-file-names.sh --pshint       # print an equivalent PowerShell command
#
# Excludes: .agent.md files, *.test.* files, well-known tooling files,
# SKILL.md, PascalCase C# (.cs) and React component (.tsx/.jsx) files.

set -euo pipefail

if [[ "${1:-}" == "--pshint" ]]; then
  cat <<'EOF'
PowerShell equivalent (run from repo root):

Get-ChildItem -Recurse -File |
  Where-Object {
    $_.FullName -notmatch '\\(bin|obj|node_modules|\.git|dist|build)\\' -and
    $_.Name -notmatch '\.agent\.md$' -and
    $_.Name -notmatch '\.test\.' -and
    $_.Name -notmatch '^[A-Z][a-zA-Z0-9]*\.(cs|tsx|jsx)$' -and
    $_.Name -notmatch '^(SKILL\.md|README\.md|LICENSE|CHANGELOG\.md|Dockerfile|package(-lock)?\.json|tsconfig.*\.json|.*\.sln|.*\.csproj)$' -and
    $_.BaseName -cmatch '[a-z][A-Z]'
  } |
  ForEach-Object { $_.FullName }
EOF
  exit 0
fi

ROOT="${1:-.}"

# Well-known / convention-exempt basenames
EXEMPT_NAMES='^(SKILL\.md|README\.md|LICENSE.*|CHANGELOG\.md|CONTRIBUTING\.md|Dockerfile|Makefile|package(-lock)?\.json|tsconfig.*\.json|vite\.config\.ts|postcss\.config\.js|tailwind\.config\.js|.*\.sln|.*\.csproj|.*\.props|\.gitignore|\.gitattributes|\.editorconfig|index\.html)$'

find "$ROOT" -type f \
  -not -path '*/.git/*' \
  -not -path '*/node_modules/*' \
  -not -path '*/bin/*' \
  -not -path '*/obj/*' \
  -not -path '*/dist/*' \
  -not -path '*/build/*' \
  | while IFS= read -r file; do
      base="$(basename "$file")"

      # Exclusions
      case "$base" in
        *.agent.md) continue ;;                    # agent metadata files
        *.test.*)   continue ;;                    # test files
      esac
      [[ "$base" =~ $EXEMPT_NAMES ]] && continue   # well-known files
      # PascalCase C# / React component files are conventional
      [[ "$base" =~ ^[A-Z][a-zA-Z0-9]*\.(cs|tsx|jsx)$ ]] && continue

      # Flag camelCase / mixed-case basenames (lower followed by Upper)
      if [[ "$base" =~ [a-z][A-Z] ]]; then
        echo "$file"
      fi
    done
