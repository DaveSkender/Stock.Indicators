# Performance Baselines

This directory contains baseline performance metrics for regression detection.

For comprehensive performance analysis and findings, see `../PERFORMANCE_ANALYSIS.md`.

## Overview

Baseline files are JSON exports from BenchmarkDotNet that capture performance metrics for comparison with current test runs. These enable automated detection of performance regressions during development and CI/CD.

## File naming convention

- `baseline-v{version}.json` - Baseline for a specific version (e.g., `baseline-v3.0.0.json`)
- `baseline-latest.json` - Most recent baseline (symlink or copy)

## Creating a new baseline

After running performance tests, copy the JSON results to create a new baseline:

```bash
# Run performance tests
dotnet run -c Release

# Copy JSON results to baselines directory
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/baseline-v3.0.0.json

# Update latest baseline
cp baselines/baseline-v3.0.0.json baselines/baseline-latest.json
```

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
