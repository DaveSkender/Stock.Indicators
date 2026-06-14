# Performance Baselines

This directory contains baseline performance metrics for regression detection.

For comprehensive performance analysis and findings, see `../PERFORMANCE_ANALYSIS.md`.

## Overview

Baseline files are JSON exports from BenchmarkDotNet that capture performance metrics for comparison with current test runs. These enable automated detection of performance regressions during development and CI/CD.

## File naming convention

- `baseline-v{version}.json` - Baseline for a specific version (e.g., `baseline-v3.0.0.json`)
- `baseline-latest.json` - Most recent baseline (symlink or copy)

## Creating a new baseline

> **Generate on a consistent reference host.** These baselines are
> hardware-sensitive absolute timings, not CI artifacts. The committed set is
> produced **locally** on the reference machine (13th-gen Intel i9-13900H,
> Windows 11 build 26200) with the **ShortRun** job — *not* on a hosted CI
> runner, whose different (and shared) hardware would make the numbers
> non-comparable. Always regenerate on the same machine so run-to-run deltas
> reflect code changes, not hardware.

Run the core suite with `--job short` (matching the committed baselines'
methodology) and copy each report over its same-named baseline:

```bash
cd tools/performance

# Run the three core styles with the ShortRun job
# (--filter is multi-value; repeated here to match the manual workflow)
dotnet run -c Release -- --job short \
  --filter 'Performance.SeriesIndicators*' \
  --filter 'Performance.BufferIndicators*' \
  --filter 'Performance.StreamIndicators*'

# Copy each report-full.json over its baseline (one file per report)
cp BenchmarkDotNet.Artifacts/results/Performance.SeriesIndicators-report-full.json baselines/
cp BenchmarkDotNet.Artifacts/results/Performance.BufferIndicators-report-full.json baselines/
cp BenchmarkDotNet.Artifacts/results/Performance.StreamIndicators-report-full.json baselines/

# Optionally tag a versioned snapshot at a release
cp baselines/Performance.StreamIndicators-report-full.json baselines/baseline-v3.0.0-stream.json
```

### Regenerating the other baseline classes

The three core classes above are the common refresh. The remaining committed
baselines come from separate benchmark classes — regenerate them the same way
(same reference host, `--job short`), then copy each `*-report-full.json` into
`baselines/`:

```bash
cd tools/performance

# Cross-style comparison — Series vs Buffer vs Stream per indicator.
# This is the authoritative "which style is fastest" source. It covers all
# ~86 indicators x 3 styles, so it is a LONG run (~2h on the reference host).
dotnet run -c Release -- --job short --filter 'Performance.StyleComparison*'
cp BenchmarkDotNet.Artifacts/results/Performance.StyleComparison-report-full.json baselines/

# Utility kernels (fast)
dotnet run -c Release -- --job short --filter 'Performance.Utility*'
cp BenchmarkDotNet.Artifacts/results/Performance.Utility-report-full.json       baselines/
cp BenchmarkDotNet.Artifacts/results/Performance.UtilityStdDev-report-full.json baselines/

# Stream-external (SSE) — needs network access to the offline emulator / feed;
# skip unless you specifically intend to refresh it.
# dotnet run -c Release -- --job short --filter 'Performance.StreamExternal*'
# cp BenchmarkDotNet.Artifacts/results/Performance.StreamExternal-report-full.json baselines/
```

> **Note:** the per-class `SeriesIndicators` benchmarks were changed to return
> their result (instead of `void`) so BenchmarkDotNet cannot dead-code-eliminate
> the call. Regenerate `Performance.SeriesIndicators-report-full.json` with the
> core command above after pulling that change; the currently committed Series
> baseline was captured just before it and is provisional until then.

## Using baselines for regression detection

The `detect-regressions.ps1` script compares current results with a baseline:

```bash
# Compare with specific baseline
pwsh detect-regressions.ps1 -BaselineFile baselines/baseline-v3.0.0.json -ThresholdPercent 10

# Auto-detect latest baseline and results
pwsh detect-regressions.ps1
```

## Baseline management best practices

- Create baselines for each major/minor release
- Update baselines when intentional performance changes are made
- Keep at least the last 3 version baselines for historical comparison
- Document significant performance changes in release notes

## What triggers a regression?

By default, a performance regression is flagged when:

- Mean execution time increases by more than 10% compared to baseline
- This threshold can be adjusted using the `-ThresholdPercent` parameter

## Integration with CI/CD

The GitHub Actions workflow (`test-performance.yml`) runs:

1. Full benchmark suite on manual trigger
2. Publishes results to GitHub Summary
3. Uploads artifacts for historical tracking

For regression detection integration, see `../PERFORMANCE_ANALYSIS.md`.
