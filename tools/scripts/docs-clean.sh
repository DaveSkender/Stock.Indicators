#!/usr/bin/env bash
set -euo pipefail

echo ""
echo "╔═══════════════════════════════════╗"
echo "║  Cleaning docs (VitePress) cache  ║"
echo "╚═══════════════════════════════════╝"

# find root directory and docs dir
ROOT_DIR="$(cd "$(dirname "$0")/../.." && pwd)"
DOCS_DIR="$ROOT_DIR/docs"

echo "🧹 evaluating ${DOCS_DIR}..."

TARGETS=(
  "$DOCS_DIR/node_modules"
  "$DOCS_DIR/dist"
  "$DOCS_DIR/.vitepress/dist"
  "$DOCS_DIR/.vitepress/cache"
  "$DOCS_DIR/_site"
)

echo "→ Deleting docs cache directories and node modules (if present)..."
for t in "${TARGETS[@]}"; do
  if [ -e "$t" ]; then
    rm -rf "$t" && echo "  Removed: $t"
  fi
done

echo "→ Deleting docs lock files (if present)..."
for f in pnpm-lock.yaml package-lock.json yarn.lock; do
  if [ -f "$DOCS_DIR/$f" ]; then
    rm -f "$DOCS_DIR/$f" && echo "  Removed: $DOCS_DIR/$f"
  fi
done

echo ""
echo "✅ Docs cache deletes completed!"
