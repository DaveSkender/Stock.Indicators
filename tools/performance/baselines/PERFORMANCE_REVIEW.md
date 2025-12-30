# Performance Review: StreamHub & BufferList Implementations

**Date:** December 29, 2025
**Analysis:** Baseline performance comparison of Series vs Stream vs Buffer implementations

## Executive Summary

Analysis of performance baseline data shows **significant improvements** since the October 2025 baseline. Many O(n²) issues have been resolved, though some StreamHub implementations still show moderate overhead compared to their Series counterparts.

### Key Findings (December 2025)

- **77 StreamHub implementations** are >30% slower than Series (average: 4.76x slower)
- **19 BufferList implementations** are >30% slower than Series (average: 1.68x slower)
- **66 StreamHub implementations** are ≥2x slower (CRITICAL)
- **3 BufferList implementations** are ≥2x slower (CRITICAL)

### Improvements Since October 2025

| Metric                    | October 2025  | December 2025      | Change         |
| ------------------------- | ------------- | ------------------ | -------------- |
| Worst StreamHub slowdown  | 391x (RSI)    | 61.6x (ForceIndex) | **84% better** |
| Worst BufferList slowdown | 7.85x (Slope) | 3.41x (Slope)      | **57% better** |
| StreamHub avg slowdown    | 28.5x         | 4.76x              | **83% better** |
| BufferList avg slowdown   | 2.1x          | 1.68x              | **20% better** |
| Critical O(n²) issues     | 4             | 0                  | **100% fixed** |

### Top 10 StreamHub Issues (Current)

| Indicator       | Series (ns) | Stream (ns) | Slowdown    | Status          |
|-----------------|-------------|-------------|-------------|-----------------|
| **ForceIndex**  | 13,508      | 831,594     | **61.56x**  | 🔴 CRITICAL     |
| **Ema**         | 2,443       | 25,638      | **10.49x**  | 🔴 CRITICAL     |
| **Pvo**         | 6,187       | 64,756      | **10.47x**  | 🔴 CRITICAL     |
| **Tema**        | 3,496       | 34,418      | **9.85x**   | 🔴 CRITICAL     |
| **Smma**        | 2,931       | 25,500      | **8.70x**   | 🔴 CRITICAL     |
| **T3**          | 4,625       | 39,997      | **8.65x**   | 🔴 CRITICAL     |
| **Dema**        | 3,545       | 30,349      | **8.56x**   | 🔴 CRITICAL     |
| **Trix**        | 4,151       | 34,667      | **8.35x**   | 🔴 CRITICAL     |
| **Awesome**     | 15,533      | 119,340     | **7.68x**   | 🔴 CRITICAL     |
| **Slope**       | 47,859      | 358,366     | **7.49x**   | 🔴 CRITICAL     |

### Top 6 BufferList Issues (Current)

| Indicator     | Series (ns) | Buffer (ns) | Slowdown  | Status      |
|---------------|-------------|-------------|-----------|-------------|
| **Slope**     | 47,859      | 162,972     | **3.41x** | 🔴 CRITICAL |
| **Alligator** | 8,609       | 18,570      | **2.16x** | 🔴 CRITICAL |
| **Adx**       | 15,088      | 31,348      | **2.08x** | 🔴 CRITICAL |
| **Ema**       | 2,443       | 4,373       | **1.79x** | ⚠️ REVIEW   |
| **Smma**      | 2,931       | 5,106       | **1.74x** | ⚠️ REVIEW   |
| **Gator**     | 13,583      | 23,506      | **1.73x** | ⚠️ REVIEW   |

## Resolved Issues (Since October 2025)

The following critical O(n²) issues have been **fully resolved**:

| Indicator      | October 2025 | December 2025 | Improvement       |
| -------------- | ------------ | ------------- | ----------------- |
| **Rsi**        | 391.33x      | 3.26x         | **99% faster** ✅ |
| **StochRsi**   | 283.53x      | 4.55x         | **98% faster** ✅ |
| **Cmo**        | 257.78x      | 2.72x         | **99% faster** ✅ |
| **Chandelier** | 121.65x      | 5.35x         | **96% faster** ✅ |
| **Stoch**      | 15.69x       | 3.24x         | **79% faster** ✅ |

These indicators now use proper O(1) incremental updates instead of O(n) recalculations.

## Root Cause Analysis

### Pattern 1: EMA Family Overhead (8-11x slower)

Multiple EMA-based indicators show consistent 8-11x overhead:

- **Ema** (10.5x)
- **Smma** (8.7x)
- **Tema** (9.9x)
- **Dema** (8.6x)
- **T3** (8.7x)
- **Trix** (8.4x)
- **Pvo** (10.5x) - uses EMA
- **Macd** (7.3x) - uses EMA

**Root cause:** StreamHub framework overhead for simple indicators. The EMA calculation itself is O(1), but the hub subscription/notification infrastructure adds constant overhead that dominates for fast indicators.

**Context:** These timings are in nanoseconds (ns). An 8-10x overhead on a 2,500 ns operation means ~25,000 ns per quote, which is still **~40,000 quotes/second** throughput - adequate for real-time streaming use cases.

### Pattern 2: ForceIndex Anomaly (61.6x slower)

ForceIndex shows unexpectedly high overhead despite being a simple calculation.

**Root cause:** Likely inefficient lookback or unnecessary allocations in the StreamHub implementation. Needs investigation.

### Pattern 3: Slope Regression (7.5x Stream, 3.4x Buffer)

Slope calculation involves linear regression which has inherent computational cost.

**Root cause:** May be recalculating regression coefficients from scratch rather than using incremental formulas.

### Pattern 4: Complex Multi-Indicator Chains

Indicators that chain multiple sub-indicators show compounded overhead:

- **Awesome** (7.7x): Uses SMA
- **StochRsi** (4.6x): Uses RSI + Stoch
- **ConnorsRsi** (1.7x): Uses multiple RSI variants

## Performance by Category

### ✅ Excellent (<1.5x slower)

These StreamHub implementations perform within acceptable overhead:

- StdDev (1.31x)
- Epma (1.47x)
- Mama (1.46x)
- Pivots (1.46x)

### ⚠️ Acceptable (1.5x-2x slower)

These need review but are functional:

- BollingerBands (1.65x)
- ConnorsRsi (1.72x)
- Renko (1.71x)
- Donchian (1.90x)
- HeikinAshi (1.91x)
- Hma (1.93x)
- RollingPivots (1.95x)

### 🔴 Needs Optimization (≥2x slower)

77 indicators fall into this category. Priority should be given to:

1. **ForceIndex** (61.6x) - Investigate anomaly
2. **EMA family** (8-11x) - Framework overhead, acceptable for real-time use
3. **Slope** (7.5x) - Consider incremental regression

## Recommendations

### IMMEDIATE (P0)

1. **ForceIndex** - Investigate and fix the 61.6x overhead anomaly

### HIGH PRIORITY (P1)

1. **Slope** - Implement incremental regression for StreamHub
2. Document that EMA-family 8-11x overhead is acceptable for real-time streaming

### MEDIUM PRIORITY (P2)

1. Review indicators in 2-5x range for optimization opportunities
2. Consider framework-level optimizations to reduce subscription overhead

### LOW PRIORITY (P3)

1. Fine-tune indicators in 1.5-2x range
2. Reduce allocations where possible

## Code Patterns to Look For

When reviewing streaming implementations, check for these anti-patterns:

### ❌ WRONG: Recalculating from scratch

```csharp
// Anti-pattern: O(n) work on every quote
public void Add(Quote quote)
{
    quotes.Add(quote);
    var result = quotes.ToRsi(period); // Recalculates entire RSI!
    results.Add(result.Last());
}
```

### ✅ CORRECT: Incremental update

```csharp
// Correct: O(1) work per quote
private double previousEma;
public void Add(Quote quote)
{
    double alpha = 2.0 / (period + 1);
    double newEma = alpha * quote.Close + (1 - alpha) * previousEma;
    previousEma = newEma;
    results.Add(newEma);
}
```

### ❌ WRONG: Full window scan

```csharp
// Anti-pattern: O(n) work per quote
public void Add(Quote quote)
{
    window.Add(quote);
    if (window.Count > period)
        window.RemoveAt(0);
    
    double max = window.Max(q => q.High); // Full scan every time!
}
```

### ✅ CORRECT: Efficient tracking with RollingWindow utilities

```csharp
// Correct: O(1) amortized with monotonic deque
private readonly RollingWindowMax<double> _highWindow;

public void Add(Quote quote)
{
    _highWindow.Add(quote.High);  // O(1) amortized
    double max = _highWindow.Max;  // O(1) retrieval
}
```

## Testing Strategy

For each fixed indicator:

1. **Correctness**: Run regression tests against Series baseline
2. **Performance**: Verify <1.5x slowdown vs Series (or document acceptable overhead)
3. **Complexity**: Verify O(n) time complexity by testing with 10x data
4. **Memory**: Check for memory leaks in long-running streams

## Success Criteria

- ✅ No O(n²) or worse complexity issues (ACHIEVED)
- ⚠️ All StreamHub indicators ≤1.5x slower than Series (77 still exceed)
- ⚠️ All BufferList indicators ≤1.5x slower than Series (19 still exceed)
- ✅ Memory usage linear with window size, not total quote count

## Benchmark Environment

**Hardware:**

- CPU: 13th Gen Intel Core i9-13900H 2.60GHz
- Cores: 20 logical, 14 physical
- RAM: High-performance configuration

**Software:**

- OS: Windows 11 (10.0.26200.7462/25H2)
- Runtime: .NET 10.0.1 (10.0.125.57005)
- BenchmarkDotNet: v0.15.8
- Job: ShortRun (IterationCount=3, LaunchCount=1, WarmupCount=3)

---

## Changelog

### December 29, 2025

- Updated baselines with full benchmark run (307 benchmarks, 1h 16m runtime)
- Documented resolution of critical O(n²) issues (RSI, StochRsi, CMO, Chandelier)
- Average StreamHub slowdown improved from 28.5x to 4.76x
- Worst case improved from 391x (RSI) to 61.6x (ForceIndex)

### October 19, 2025

- Initial baseline analysis
- Identified 4 critical O(n²) issues
- Documented patterns and recommendations

---

**Generated by:** `analyze_performance.py`
**Baseline data:** December 29, 2025 benchmarks
**Last reviewed:** December 29, 2025
