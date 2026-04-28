#!/usr/bin/env bash
# Builds @facioquo/chartjs-chart-financial and @facioquo/indy-charts from source.
# These packages live in facioquo/stock-charts and must be compiled before the
# VitePress docs site can be built.  Run this once before `pnpm install`.
#
# Usage:
#   bash docs/build-local-packages.sh           # from repo root
#   bash build-local-packages.sh                # from docs/ directory

set -euo pipefail

STOCK_CHARTS_REF="${STOCK_CHARTS_REF:-6743-main}"
DOCS_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WORK_DIR="$(mktemp -d)"

echo "→ Cloning facioquo/stock-charts @ $STOCK_CHARTS_REF"
git clone --depth 1 --branch "$STOCK_CHARTS_REF" \
  https://github.com/facioquo/stock-charts.git "$WORK_DIR"

echo "→ Installing workspace dependencies (ignore-scripts skips postinstall)"
cd "$WORK_DIR"
pnpm install --frozen-lockfile --ignore-scripts

echo "→ Building @facioquo/chartjs-chart-financial"
pnpm --filter @facioquo/chartjs-chart-financial build

echo "→ Building @facioquo/indy-charts"
pnpm --filter @facioquo/indy-charts build

echo "→ Copying dist to local-packages"
rm -rf "$DOCS_DIR/local-packages/chartjs-chart-financial/dist"
cp -r "$WORK_DIR/libs/chartjs-financial/dist" \
  "$DOCS_DIR/local-packages/chartjs-chart-financial/dist"

rm -rf "$DOCS_DIR/local-packages/indy-charts/dist"
cp -r "$WORK_DIR/libs/indy-charts/dist" \
  "$DOCS_DIR/local-packages/indy-charts/dist"

echo "→ Cleaning up"
rm -rf "$WORK_DIR"

echo "✓ Local packages built successfully"
