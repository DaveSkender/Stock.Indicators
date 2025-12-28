#!/usr/bin/env bash
# DEPRECATED: This script is no longer used by VS Code tasks.
# Use "Lint: All" task in VS Code instead, which runs Roslynator and dotnet format as composite tasks.
# This script is kept for backwards compatibility but may be removed in the future.
#
# Lint check-only using Roslynator and .NET formatter

echo ""
echo "⚠️  DEPRECATED: Use 'Lint: All' task in VS Code instead"
echo ""
echo "=== Running code linting ==="
cd "$(dirname "$0")/../.." || exit 1

echo ""
echo "=== Running Roslynator analysis ==="
roslynator analyze \
--severity-level hidden \
--verbosity normal \
--properties TargetFramework=net10.0 || exit 1

echo ""
echo "=== Running .NET format check ==="
dotnet format \
--verify-no-changes \
--severity info \
--no-restore || exit 1

echo ""
echo "✓ Code linting completed!"
