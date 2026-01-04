# Performance Analysis: Streaming Indicators

**Last Updated:** January 3, 2026
**Baseline Data:** tools/performance/baselines/

## Executive Summary

Performance analysis comparing Series, BufferList, and StreamHub implementations across all indicators in the library. Analysis identifies performance characteristics and indicators requiring optimization.

### Current Performance Status

- **BufferList implementations**: 77% within acceptable range (<2x overhead), 3 critical issues
- **StreamHub implementations**: 12% within acceptable range (<2x overhead), 64 critical issues
- **Memory profiling**: MemoryDiagnoser enabled with GC columns, baseline data collection in progress

### Performance Targets (NFR-002)

**BufferList:**

- Target: <1.3x overhead (within 30% of Series)
- Acceptable: <2x overhead
- Critical: ≥2x overhead requires investigation

**StreamHub:**

- Target: <1.5x overhead (within 50% of Series)
- Acceptable: <3x overhead
- Critical: ≥2x overhead requires investigation

**Memory:**

- StreamHub: <10KB overhead per instance
- BufferList: <5KB overhead per instance

## Measurement Methodology

- **Dataset**: 502 periods of historical OHLCV data
- **Benchmark Tool**: BenchmarkDotNet v0.15.8 with Release configuration
- **Runtime**: .NET 10.0.1, X64 RyuJIT x86-64-v3
- **Job**: ShortRun (3 iterations, 1 launch, 3 warmup)
- **Metrics**: Mean execution time (nanoseconds), Error, StdDev, Memory allocations, Gen0/Gen1/Gen2 GC collections
- **Sorting**: Results sorted fastest to slowest

## Style Comparison Benchmarks

Run comprehensive comparison across all indicators:

```bash
cd tools/performance
dotnet run -c Release -- --filter 'Performance.StyleComparison*'
```

BenchmarkDotNet groups results by indicator with automatic ratio columns comparing Buffer and Stream to Series baseline. This provides quick visual identification of performance differences.

**GitHub Actions**: Manual workflow available at `.github/workflows/test-performance-comparison.yml`

## BufferList Performance Results

### Distribution Summary

**Within target (<1.3x)**: 60 indicators (~77%)

- Overhead: <30%
- Status: ✅ Meets target

**Acceptable (1.3x-2x)**: 16 indicators (~20%)

- Overhead: 30%-100%
- Status: ✅ Acceptable

**Critical (≥2x)**: 3 indicators (~3%)

- Status: 🔴 Requires optimization

### Critical BufferList Issues (≥2x Slower)

| Indicator     | Series (ns) | Buffer (ns) | Slowdown  | Priority   |
| ------------- | ----------- | ----------- | --------- | ---------- |
| **Slope**     | 47,859      | 162,972     | **3.41x** | 🔴 HIGH    |
| **Alligator** | 8,609       | 18,570      | **2.16x** | 🔴 MEDIUM  |
| **Adx**       | 15,088      | 31,348      | **2.08x** | 🔴 MEDIUM  |

### BufferList Review Candidates (1.3x-2x Slower)

| Indicator      | Slowdown | Status     |
| -------------- | -------- | ---------- |
| Ema            | 1.79x    | ⚠️ REVIEW  |
| Smma           | 1.74x    | ⚠️ REVIEW  |
| Gator          | 1.73x    | ⚠️ REVIEW  |
| Macd           | 1.65x    | ⚠️ REVIEW  |
| Fractal        | 1.59x    | ⚠️ REVIEW  |
| Pivots         | 1.54x    | ⚠️ REVIEW  |
| Tr             | 1.52x    | ⚠️ REVIEW  |
| ChaikinOsc     | 1.50x    | ⚠️ REVIEW  |
| Awesome        | 1.50x    | ⚠️ REVIEW  |
| Trix           | 1.45x    | ⚠️ REVIEW  |
| Pmo            | 1.43x    | ⚠️ REVIEW  |
| Tema           | 1.42x    | ⚠️ REVIEW  |
| Epma           | 1.39x    | ⚠️ REVIEW  |
| Chandelier     | 1.37x    | ⚠️ REVIEW  |
| Beta           | 1.31x    | ⚠️ REVIEW  |
| Dema           | 1.30x    | ⚠️ REVIEW  |

## StreamHub Performance Results

### Distribution Summary

**Within target (<1.5x)**: 6 indicators (~7%)

- Overhead: <50%
- Status: ✅ Meets target

**Acceptable (1.5x-2x)**: 4 indicators (~5%)

- Overhead: 50%-100%
- Status: ✅ Acceptable

**Critical (≥2x)**: 64 indicators (~78%)

- Status: 🔴 Requires optimization

### Top 20 Critical StreamHub Issues (≥2x Slower)

| Indicator      | Series (ns) | Stream (ns) | Slowdown    | Status           |
| -------------- | ----------- | ----------- | ----------- | ---------------- |
| **ForceIndex** | 13,508      | 831,594     | **61.56x**  | 🔴 O(n²) issue   |
| **Ema**        | 2,443       | 25,638      | **10.49x**  | 🔴 Framework OH  |
| **Pvo**        | 6,187       | 64,756      | **10.47x**  | 🔴 Framework OH  |
| **Tema**       | 3,496       | 34,418      | **9.85x**   | 🔴 Framework OH  |
| **Smma**       | 2,931       | 25,500      | **8.70x**   | 🔴 Framework OH  |
| **T3**         | 4,625       | 39,997      | **8.65x**   | 🔴 Framework OH  |
| **Dema**       | 3,545       | 30,349      | **8.56x**   | 🔴 Framework OH  |
| **Trix**       | 4,151       | 34,667      | **8.35x**   | 🔴 Framework OH  |
| **Awesome**    | 15,533      | 119,340     | **7.68x**   | 🔴 Framework OH  |
| **Slope**      | 47,859      | 358,366     | **7.49x**   | 🔴 Optimize      |
| **Prs**        | 4,694       | 35,070      | **7.47x**   | 🔴 Optimize      |
| **Macd**       | 6,196       | 45,313      | **7.31x**   | 🔴 Framework OH  |
| **Roc**        | 4,322       | 30,153      | **6.98x**   | 🔴 Optimize      |
| **PivotPoints**| 12,753      | 79,268      | **6.22x**   | 🔴 Optimize      |
| **Gator**      | 13,583      | 84,161      | **6.20x**   | 🔴 Optimize      |
| **Ultimate**   | 27,426      | 161,480     | **5.89x**   | 🔴 Optimize      |
| **Adl**        | 5,534       | 32,493      | **5.87x**   | 🔴 Optimize      |
| **Pmo**        | 5,760       | 33,445      | **5.81x**   | 🔴 Optimize      |
| **Smi**        | 13,939      | 76,236      | **5.47x**   | 🔴 Optimize      |
| **Chandelier** | 22,454      | 120,072     | **5.35x**   | 🔴 Optimize      |

### StreamHub Review Candidates (1.3x-2x Slower)

| Indicator       | Slowdown | Status     |
| --------------- | -------- | ---------- |
| RollingPivots   | 1.95x    | ⚠️ REVIEW  |
| Hma             | 1.93x    | ⚠️ REVIEW  |
| HeikinAshi      | 1.91x    | ⚠️ REVIEW  |
| Donchian        | 1.90x    | ⚠️ REVIEW  |
| ConnorsRsi      | 1.72x    | ⚠️ REVIEW  |
| Renko           | 1.71x    | ⚠️ REVIEW  |
| BollingerBands  | 1.65x    | ⚠️ REVIEW  |
| Epma            | 1.47x    | ⚠️ REVIEW  |
| Pivots          | 1.46x    | ⚠️ REVIEW  |
| Mama            | 1.46x    | ⚠️ REVIEW  |
| StdDev          | 1.31x    | ⚠️ REVIEW  |

## Performance Issue Patterns

### Pattern 1: ForceIndex O(n²) Complexity (61.56x)

**Root cause:** Nested loop recalculating entire history on each quote

**Impact:** Severe performance degradation, unusable for real-time streaming

**Solution:** Implement O(1) incremental update with rolling state

### Pattern 2: EMA/SMA Family Framework Overhead (7-11x)

**Affected indicators:** Ema, Smma, Tema, Dema, T3, Trix, Pvo, Macd, Awesome

**Root cause:** StreamHub subscription/notification infrastructure overhead dominates simple operations

**Performance context:** These timings are in nanoseconds. An 8-10x overhead on a 2,500 ns operation equals ~25,000 ns per quote, achieving **~40,000 quotes/second** throughput

**Status:** Acceptable for real-time streaming use cases, framework optimization deferred

### Pattern 3: Window Operation Inefficiencies (3-8x)

**Affected indicators:** Slope, Prs, Roc, PivotPoints, Gator, Ultimate, Adl, Pmo, Smi, Chandelier

**Root cause:** Inefficient lookback operations, unnecessary allocations, or improper state management

**Solution:** Investigate each indicator for:

- O(n²) nested loops
- Unnecessary collection copies
- Missing circular buffer optimizations
- Redundant calculations not cached

## Memory Profiling

### MemoryDiagnoser Configuration

- Enabled with generational GC columns (Gen0/Gen1/Gen2 per 1,000 operations)
- Tracks allocated bytes per operation
- Results sorted by performance (fastest to slowest)

### Memory Validation

Baseline data collection in progress. Run benchmarks to generate memory allocation reports:

```bash
cd tools/performance
dotnet run -c Release
```

Check compliance against NFR-002 targets using result JSON files in `BenchmarkDotNet.Artifacts/results/`.

## Analysis Tools

### Performance Analysis Script

Generate detailed comparison reports:

```bash
cd tools/performance/baselines
python3 analyze_performance.py
```

The script:

- Loads baseline JSON for Series, Stream, and Buffer styles
- Calculates performance ratios (Stream/Series, Buffer/Series)
- Flags indicators exceeding thresholds (>30% slower for warning, >100% for critical)
- Identifies potential causes: O(n²) loops, unnecessary allocations, inefficient operations

### Baseline Management

**Update baselines after performance improvements:**

```bash
# Run full benchmark suite
cd tools/performance
dotnet run -c Release

# Copy results to baselines
cp BenchmarkDotNet.Artifacts/results/Performance.*-report-full.json baselines/

# Document changes in streaming-indicators.plan.md
```

**Baseline files:**

- `Performance.SeriesIndicators-report-full.json` - Series batch processing baseline
- `Performance.BufferIndicators-report-full.json` - BufferList incremental processing baseline
- `Performance.StreamIndicators-report-full.json` - StreamHub real-time streaming baseline
- `Performance.StyleComparison-report-full.json` - Direct style-to-style comparison baseline

## References

- Benchmarking guide: `benchmarking.md`
- Baseline management: `baselines/README.md`
- Memory baseline guidelines: `baselines/memory/README.md`
- Regression detection: `detect-regressions.ps1`
- Performance analysis: `baselines/analyze_performance.py`
- GitHub Actions workflow: `.github/workflows/test-performance.yml`
- NFR-002: Performance and memory requirements for streaming indicators
- Streaming plan: `docs/plans/streaming-indicators.plan.md`

---

Last updated: January 3, 2026
