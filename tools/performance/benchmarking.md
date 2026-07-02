# Performance benchmarking guide

This document describes how to run performance benchmarks, refresh baselines, and do fast spot-checks for code changes.

## Overview

The repository uses [BenchmarkDotNet](https://benchmarkdotnet.org/) benchmarks under `tools/performance`:

- `Perf.Series.cs` - Series benchmarks
- `Perf.Buffer.cs` - BufferList benchmarks
- `Perf.Stream.cs` - StreamHub benchmarks
- `Perf.StyleComparison.cs` - cross-style ratio comparisons
- `Perf.StreamExternal.cs` - external streaming checks
- `Perf.Utility*.cs` - utility benchmarks
- `Perf.ManualTestDirect.cs` - direct, large-N scale tests

## Running benchmarks

From `tools/performance`:

```bash
# Full suite (can take several hours on many machines)
dotnet run -c Release

# Targeted suites
dotnet run -c Release -- --filter "*SeriesIndicators*"
dotnet run -c Release -- --filter "*BufferIndicators*"
dotnet run -c Release -- --filter "*StreamIndicators*"
dotnet run -c Release -- --filter "*StyleComparison*"

# Single benchmark method
dotnet run -c Release -- --filter "*.ToEmaHub"
```

Always pass BDN arguments after `--`.

## Fast spot-checks for development

Use `ManualTestDirect` when you need quick validation without running the full suite:

```bash
# Example: 500k bars for EMA across enabled styles
PERF_TEST_KEYWORD=ema PERF_TEST_PERIODS=500000 dotnet run -c Release -- --filter "Performance.ManualTestDirect*"

# Optional: force pruning path (cap < periods)
PERF_TEST_KEYWORD=adl PERF_TEST_PERIODS=500000 PERF_TEST_CAP=100000 dotnet run -c Release -- --filter "Performance.ManualTestDirect*"
```

`ManualTestDirect` avoids catalog reflection overhead and is useful for AI/developer spot checks during iterative work.

## Result artifacts

BenchmarkDotNet writes artifacts to `BenchmarkDotNet.Artifacts/results/`:

- `Performance.*-report-full.json` - machine-readable baseline and regression input
- `Performance.*-report-github.md` - human-readable tables

## Baseline refresh procedure (standard)

From `tools/performance` after benchmarks complete:

```bash
# Copy machine-readable baselines
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/

# Copy human-readable baseline reports
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-github.md baselines/
```

PowerShell equivalent:

```powershell
Copy-Item BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/
Copy-Item BenchmarkDotNet.Artifacts/results/Performance.*-report-github.md baselines/
```

Both file types are part of baseline refresh.

## Regression detection

Use `detect-regressions.ps1` from `tools/performance`:

```bash
# Auto-detect latest baseline/current files
pwsh detect-regressions.ps1

# Custom threshold
pwsh detect-regressions.ps1 -ThresholdPercent 15

# Explicit class-to-class comparison
pwsh detect-regressions.ps1 `
  -BaselineFile baselines/Performance.StreamIndicators-report-full.json `
  -CurrentFile BenchmarkDotNet.Artifacts/results/Performance.StreamIndicators-report-full.json `
  -ThresholdPercent 10
```

Exit codes:

- `0` - no regressions
- `1` - regressions detected

## CI workflows

Performance workflows:

- `.github/workflows/test-performance.yml` - full benchmark suite
- `.github/workflows/test-performance-comparison.yml` - style comparison run
- `.github/workflows/test-performance-manual.yml` - targeted manual workflow

## Best practices

- Use `-c Release` always.
- Compare like-for-like runs (same benchmark scope/configuration).
- Prefer filtered runs for development loops; use full runs for baseline refresh.
- Keep baseline updates paired with any intentional performance-shifting work.

## References

- [Baselines README](baselines/README.md)
- [BenchmarkDotNet documentation](https://benchmarkdotnet.org/)
