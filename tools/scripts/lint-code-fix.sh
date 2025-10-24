#!/usr/bin/env bash
# Lint fixer using Roslynator and .NET formatter

cd "$(dirname "$0")/../.." || exit 1

roslynator fix \
--severity-level info \
--verbosity normal \
--properties TargetFramework=net9.0

dotnet format \
--severity info \
--no-restore
