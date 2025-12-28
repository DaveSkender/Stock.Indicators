#!/usr/bin/env bash
# DEPRECATED: This script is no longer used by VS Code tasks.
# Use "Lint: All (fix)" task in VS Code instead, which runs Roslynator and dotnet format as composite tasks.
# This script is kept for backwards compatibility but may be removed in the future.
#
# Lint fixer using Roslynator and .NET formatter

echo ""
echo "⚠️  DEPRECATED: Use 'Lint: All (fix)' task in VS Code instead"
echo ""
echo "=== Running code fixers ==="
cd "$(dirname "$0")/../.." || exit 1

echo ""
echo "=== Running Roslynator fixer ==="
roslynator fix \
--severity-level hidden \
--verbosity normal \
--properties TargetFramework=net10.0 || exit 1

echo ""
echo "=== Running Roslynator formatter ==="
roslynator format || exit 1

echo ""
echo "=== Running .NET formatter ==="
dotnet format \
--severity info \
--no-restore || exit 1

echo ""
echo "✓ Fixing completed!"
