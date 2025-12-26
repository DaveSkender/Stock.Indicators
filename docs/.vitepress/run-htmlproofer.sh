#!/usr/bin/env bash
set -euo pipefail

# Run from repo root regardless of invocation location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
cd "${REPO_ROOT}/docs"

pnpm run docs:build

if command -v docker >/dev/null 2>&1; then
  tar -C .vitepress/dist -cf - . | \
    docker run --rm -i ruby:3.4 bash -lc "mkdir -p /site && tar -xf - -C /site && gem install --no-document html-proofer && export PATH=\"/usr/local/bundle/bin:$PATH\" && htmlproofer /site --no-enforce-https --no-check-external-hash --ignore-status-codes \"0,302,403,406,408,429,503,999\" --ignore-urls \"/fonts.gstatic.com/\""
else
  gem install --no-document html-proofer
  htmlproofer .vitepress/dist --no-enforce-https --no-check-external-hash --ignore-status-codes "0,302,403,406,408,429,503,999" --ignore-urls "/fonts.gstatic.com/"
fi
