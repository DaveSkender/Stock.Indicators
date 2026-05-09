#!/bin/bash
set -e

pnpm run docs:build

pnpm run docs:preview &
PREVIEW_PID=$!
trap 'kill $PREVIEW_PID 2>/dev/null || true' EXIT

echo "Waiting for preview server..."
for i in {1..10}; do
  if curl -sf http://localhost:4173/ >/dev/null 2>&1; then
    echo "Preview server ready"
    break
  fi
  sleep 2
  if [ $i -eq 10 ]; then
    echo "Preview server not ready after 20 seconds"
    exit 1
  fi
done

pnpm dlx pa11y-ci \
  --sitemap http://localhost:4173/sitemap.xml \
  --sitemap-find https://dotnet.stockindicators.dev \
  --sitemap-replace http://localhost:4173
