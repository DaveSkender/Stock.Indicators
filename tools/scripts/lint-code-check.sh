#!/usr/bin/env bash
# Lint check-only using Roslynator and .NET formatter

cd "$(dirname "$0")/../.." || exit 1

roslynator analyze \
--severity-level info \
--verbosity normal \
--properties TargetFramework=net9.0

dotnet format \
--verify-no-changes \
--severity info \
--no-restore
