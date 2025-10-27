#!/usr/bin/env bash
# Lint fixer using Roslynator and .NET formatter

echo ""
echo "=== Running code fixers ==="
cd "$(dirname "$0")/../.." || exit 1

echo ""
echo "=== Running Roslynator fixer ==="
roslynator fix \
--severity-level hidden \
--verbosity normal \
--properties TargetFramework=net9.0 || exit 1

echo ""
echo "=== Running .NET format fixer ==="
dotnet format \
--severity info \
--no-restore || exit 1

echo ""
echo "✓ Fixing completed!"
