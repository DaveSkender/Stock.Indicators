# Memory baselines

This folder stores versioned memory-focused baseline snapshots.

## Current file

- `baseline-memory-v3.1.0-stylecomparison.json` - historical StyleComparison memory snapshot.

## Collecting a new memory snapshot

From `tools/performance`:

```bash
dotnet run -c Release -- --filter "*StyleComparison*"
cp BenchmarkDotNet.Artifacts/results/Performance.StyleComparison-report-full.json baselines/memory/baseline-memory-<version>-stylecomparison.json
```

## Notes

- BenchmarkDotNet `MemoryDiagnoser` metrics are embedded in the `*-report-full.json` artifact.
- Keep memory snapshots versioned and immutable once committed.
- Use `tools/performance/detect-regressions.ps1` for timing regressions; memory drift review is currently manual.

## References

- `../../benchmarking.md`
- `../../../docs/plans/streaming-indicators.plan.md`
