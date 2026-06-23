#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DOCS_DIR="${SCRIPT_DIR}/.."
SITEMAP_PATH="${SCRIPT_DIR}/dist/sitemap.xml"

cd "${DOCS_DIR}"

pnpm run docs:build

pnpm run docs:preview &
PREVIEW_PID=$!
trap 'kill $PREVIEW_PID 2>/dev/null || true' EXIT

echo "Waiting for preview server..."
for i in {1..10}; do
  if curl -sf http://localhost:4173/ >/dev/null 2>&1 \
    && curl -sf http://localhost:4173/sitemap.xml >/dev/null 2>&1; then
    echo "Preview server and sitemap are ready"
    break
  fi
  sleep 2
  if [ $i -eq 10 ]; then
    echo "Preview server/sitemap not ready after 20 seconds"
    exit 1
  fi
done

mapfile -t URLS < <(
  grep -oE '<loc>[^<]+' "${SITEMAP_PATH}" \
    | sed 's#<loc>##' \
    | sed 's#https://dotnet.stockindicators.dev#http://localhost:4173#'
)

if [ ${#URLS[@]} -eq 0 ]; then
  echo "No URLs parsed from sitemap: ${SITEMAP_PATH}"
  exit 1
fi

if [ -z "${PUPPETEER_EXECUTABLE_PATH:-}" ]; then
  CACHED_CHROME=""
  if [ -d "$HOME/.cache/puppeteer/chrome" ]; then
    CACHED_CHROME="$(find "$HOME/.cache/puppeteer/chrome" -type f \( -name 'chrome.exe' -o -name 'chrome' \) 2>/dev/null | head -n 1)"
  fi

  for candidate in \
    "$CACHED_CHROME" \
    "/c/Program Files/Google/Chrome/Application/chrome.exe" \
    "/mnt/c/Program Files/Google/Chrome/Application/chrome.exe" \
    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
  do
    if [ -n "$candidate" ] && [ -x "$candidate" ]; then
      export PUPPETEER_EXECUTABLE_PATH="$candidate"
      break
    fi
  done

  if [ -z "${PUPPETEER_EXECUTABLE_PATH:-}" ]; then
    for browser_cmd in google-chrome-stable google-chrome chromium chromium-browser; do
      if command -v "$browser_cmd" >/dev/null 2>&1; then
        export PUPPETEER_EXECUTABLE_PATH="$(command -v "$browser_cmd")"
        break
      fi
    done
  fi
fi

pnpm dlx pa11y-ci "${URLS[@]}"
