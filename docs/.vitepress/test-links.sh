#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "${SCRIPT_DIR}/.."

pnpm run docs:build

if command -v docker >/dev/null 2>&1; then
  tar -C .vitepress/dist -cf - . | \
    docker run --rm -i ruby:3.4 bash -lc \
      "mkdir -p /site && \
       tar -xf - -C /site && \
       gem install --no-document html-proofer && \
       /usr/local/bundle/bin/htmlproofer /site \
         --no-enforce-https \
         --no-check-external-hash \
         --ignore-urls "fonts.gstatic.com,github.com/DaveSkender/Stock.Indicators"
else
  echo "Error: Docker is required for htmlproofer"
  exit 1
fi
