#!/usr/bin/env bash
# Run all linting checks (code + markdown) in sequence

cd "$(dirname "$0")/../.." || exit 1

bash tools/scripts/lint-code-check.sh || exit 1

echo ""
echo "=== Running markdown linting ==="
npm run lint:md || exit 1

echo ""
echo "✓ All linting checks passed!"
