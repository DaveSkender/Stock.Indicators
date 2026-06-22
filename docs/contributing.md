---
title: Contributing guidelines
description: >-
  This NuGet package is an open-source project.
  Learn how to contribute issues, fixes, new indicators, new features, or to our discussions.
permalink: /contributing/
relative_path: pages/contributing.md
layout: page
---

# Contributing guidelines for v2

> This abbreviated guidelines augments the main [contributing guidelines](https://github.com/facioquo/stock-indicators-dotnet#contributing-ov-file) and is tailored for maintenance of v2

## Testing

- Review the `tests/indicators` folder for examples of unit tests.  Just copy one of these.
- New indicators should be tested against manually calculated, proven, accurate results.  It is helpful to include your manual calculations spreadsheet in the appropriate indicator test folder when [submitting changes](https://github.com/facioquo/stock-indicators-dotnet#contributing-ov-file#submitting-changes).
- Historical Stock Quotes are automatically added to unit test methods.  A `Data.Quotes.xlsx` Excel file is included in the `tests/_common` folder that is an exact copy of what is used in the unit tests.  Use a copy of this file for your manual calculations to ensure that it is correct.  Do not commit changes to the original file.
- We expect all unit tests to execute successfully and all _Errors and Warning_ resolved before you submit your code.
- Failed builds or unit testing will block acceptance of your Pull Request when submitting changes.

### Performance benchmarking

Running the performance benchmark application in `Release` mode will produce [benchmark performance data](https://v2.dotnet.stockindicators.dev/performance/) that we include on our documentation site.

```bash
# from /tests/performance folder
# run all performance benchmarks
dotnet run -c Release

# run individual performance benchmark
dotnet run -c Release --filter *.GetAdx
```

## Documentation

Our documentation site code is in the `docs` folder.
When adding or updating indicators:

- Add or update the `/docs/_indicators/` documentation files.
- Page image assets go here: `/docs/assets/`

See [docs/README.md] for more information about setup and usage.

### Package versioning and deployment

Packages are deployed via two separate GitHub Actions workflows using two different modes:

1. **Incremental CI packages** (`2.7.2-ci.1234` versions) are available via [GitHub Packages](https://github.com/facioquo/stock-indicators-dotnet/pkgs/nuget/Skender.Stock.Indicators) and are intended for contributor and internal use.
2. **Generally available packages** (`2.7.3` versions) are available on [NuGet.org](https://www.nuget.org/packages/Skender.Stock.Indicators) and are intentionally deployed for public use.

#### Internal and CI package deploy (automatic)

**Trigger:** Push to `main` or `v*` branches

- Published to GitHub Packages only
- Version format: `{Major}.{Minor}.{Patch}-ci.{run_number}`
- Examples: `2.7.2-ci.1234`, `3.0.0-ci.567`
- Idempotent: Each commit gets unique incrementing run number
- No Git tags created

**Workflow:** `.github/workflows/deploy-package-github.yml`

#### Production package deploy (manual)

**Trigger:** Creating a [GitHub Release](https://github.com/facioquo/stock-indicators-dotnet/releases)

- Published to `nuget.org` only
- Version comes directly from Release tag (strips 'v' prefix)
- Examples:
  - Release named `2.8.0` → deploys `2.8.0` (stable)
  - Release named `3.0.0-preview.2` → deploys `3.0.0-preview.2` (preview)
- Draft releases: Dry-run mode (build only, no deploy)
- Published releases: Full deployment to nuget.org
- Git tag auto generated (from release creation)
- DLL version properties auto-applied via [GitVersion](https://gitversion.net)

**Workflow:** `.github/workflows/deploy-package-nuget.yml`

#### Versioning

We use a standard [semantic versioning](https://semver.org) convention

| Component | Format    | Description |
| --------- | --------- | ----------- |
| Major     | `x.-.-`   | Breaking changes. Use `+semver: major` in commit message. |
| Minor     | `-.x.-`   | New features, backward compatible. Use `+semver: minor` in commit message. |
| Patch     | `-.-.x`   | Bug fixes, documentation. Use `+semver: patch` (or default auto-increment). |
| Suffix    | `-label.N` | Pre-release identifier. Automatically set based on branch and build type. |

##### Version examples

| Scenario | Trigger | Version | Registry | Notes |
| :------- | :------ | :------ | :------- | :---- |
| CI build | Push to main | `2.7.2-ci.1234` | GitHub Packages | Run 1234 |
| CI build | Push to main | `2.7.2-ci.1235` | GitHub Packages | Run 1235 (next commit) |
| CI build | Push to v3 | `3.0.0-ci.567` | GitHub Packages | Run 567 |
| Production | Release tag `2.8.0` | `2.8.0` | nuget.org | Stable version |
| Production | Release tag `3.0.0-preview.2` | `3.0.0-preview.2` | nuget.org | Preview version |
| Draft release | Release tag `2.8.1` **(draft)** | `2.8.1` | **None** (dry-run) | Build only, no deploy |

For technical details, see:

- GitVersion configuration: [`src/gitversion.yml`](https://github.com/facioquo/stock-indicators-dotnet/blob/main/src/gitversion.yml)
- CI workflow: [`deploy-package-github.yml`](https://github.com/facioquo/stock-indicators-dotnet/blob/v2/.github/workflows/deploy-package-github.yml)
- Production workflow: [`deploy-package-nuget.yml`](https://github.com/facioquo/stock-indicators-dotnet/blob/v2/.github/workflows/deploy-package-nuget.yml)

## Development environment setup

In order to develop, you can either use our repo-defined Dev Container or install prerequisites on your computer. You'll need:

- [.NET SDK](https://dotnet.microsoft.com/download) versions 8,9,10 for developing the main NuGet library
- [Jekyll](https://jekyllrb.com/docs/installation) for updating the documentation site

## Getting help

[Start a new discussion](https://github.com/facioquo/stock-indicators-dotnet/discussions) or [submit an issue](https://github.com/facioquo/stock-indicators-dotnet/issues) if it is publicly relevant.
