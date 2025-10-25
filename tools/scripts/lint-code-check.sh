#!/usr/bin/env bash
# Lint check-only using Roslynator and .NET formatter

echo ""
echo "=== Running code linting ==="
cd "$(dirname "$0")/../.." || exit 1

echo ""
echo "=== Running Roslynator analysis ==="
roslynator analyze \
--severity-level hidden \
--verbosity normal \
--properties TargetFramework=net9.0

echo ""
echo "=== Running .NET format check ==="
dotnet format \
--verify-no-changes \
--severity info \
--no-restore

echo ""
echo "✓ Code linting completed!"
