# Performance baselines

This directory stores committed baseline artifacts used for performance review and regression checks.

## What is stored here

For each benchmark class we keep two baseline files:

- `Performance.*-report-full.json` - machine-readable regression input
- `Performance.*-report-github.md` - human-readable benchmark tables

The `-github.md` files are intentionally committed for easier review.

## Standard baseline refresh

After running benchmarks from `tools/performance`:

```bash
# Machine-readable baseline files
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/

# Human-readable baseline files
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-github.md baselines/
```

PowerShell equivalent:

```powershell
Copy-Item BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/
Copy-Item BenchmarkDotNet.Artifacts/results/Performance.*-report-github.md baselines/
```

## Regression detection

Run from `tools/performance`:

```bash
# Auto-detect most recent baseline/current files
pwsh detect-regressions.ps1

# Custom threshold
pwsh detect-regressions.ps1 -ThresholdPercent 15

# Explicit comparison
pwsh detect-regressions.ps1 `
  -BaselineFile baselines/Performance.BufferIndicators-report-full.json `
  -CurrentFile BenchmarkDotNet.Artifacts/results/Performance.BufferIndicators-report-full.json
```

Exit codes:

- `0` - no regressions
- `1` - regressions found

## Notes

- Historical pre-fix snapshots were retired from this folder; use git history/tags to inspect older baselines.
- Keep baseline refreshes tied to intentional performance-shifting work or release-gate refreshes.
