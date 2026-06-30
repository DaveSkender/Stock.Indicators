#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "${SCRIPT_DIR}/.."

pnpm run docs:build

if command -v docker >/dev/null 2>&1; then
  run_htmlproofer() {
    tar -C .vitepress/dist -cf - . | \
      docker run --rm -i ruby:3.4 bash -lc \
        "mkdir -p /site && \
         tar -xf - -C /site && \
         gem install --no-document html-proofer && \
         /usr/local/bundle/bin/htmlproofer /site \
           --no-enforce-https \
           --no-check-external-hash \
           --ignore-status-codes '302,401,402,403,406,429,999' \
           --ignore-urls '/fonts.gstatic.com/,/github\.com\/(DaveSkender\/Stock\.Indicators|facioquo\/stock-indicators-dotnet)\/(edit|blob|tree|discussions)\//,/((www\.)?google\.[^\/]+\/search\?)/'"
  }

  max_attempts=2
  for attempt in $(seq 1 $max_attempts); do
    if run_htmlproofer; then
      exit 0
    fi

    if [ "$attempt" -lt "$max_attempts" ]; then
      echo "htmlproofer failed on attempt $attempt/$max_attempts; retrying in 5 seconds..."
      sleep 5
    fi
  done

  echo "htmlproofer failed after $max_attempts attempts."
  exit 1
else
  echo "Error: Docker is required for htmlproofer"
  exit 1
fi
