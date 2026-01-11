#!/bin/bash
set -e

echo "Building VitePress docs..."
pnpm run docs:build > /tmp/vp-build.log

echo "Starting preview server..."
pnpm run docs:preview --host --port 4173 --strictPort > /tmp/vp-preview.log 2>&1 &
PREVIEW_PID=$!
trap 'kill $PREVIEW_PID 2>/dev/null || true' EXIT

echo "Waiting for preview server to be ready..."
for i in {1..10}; do
  if curl -sSf http://localhost:4173/sitemap.xml >/dev/null 2>&1; then
    echo "Preview server ready"
    break
  fi
  sleep 2
  if [ $i -eq 10 ]; then
    echo "Preview server not ready after 20 seconds"
    cat /tmp/vp-preview.log
    exit 1
  fi
done

echo "Running pa11y-ci..."
pnpm dlx pa11y --environment
pnpm dlx pa11y-ci \
  --sitemap http://localhost:4173/sitemap.xml \
  --sitemap-find https://dotnet.stockindicators.dev \
  --sitemap-replace http://localhost:4173
