---
applyTo: "**/*.csproj,Directory.Packages.props"
description: "Guidelines for managing NuGet packages"
---

# Guidelines for managing NuGet packages

- always update packages to their latest compatible versions
- NEVER use preview or rollback from current version levels for primary packages
- use `dotnet-outdated` CLI to determine the latest version of packages
- sort package references alphabetically in `.csproj` and `.props` files
- ALWAYS verify updates with `dotnet build` and resolve all errors
