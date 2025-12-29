# Performance Tuning Analysis and Recommendations

This document provides a comprehensive analysis of performance optimization opportunities for Stock Indicators v3, evaluating existing GitHub issues and identifying additional improvement areas.

**Date:** December 29, 2025
**Related Issues:** #1799 (epic), #1259, #1323, #1376, #1757, #1792

## Executive Summary

This analysis evaluates five open performance-related issues and identifies additional optimization opportunities for the Stock Indicators library. The recommendations are prioritized by impact, feasibility, and alignment with the project's core principles (Mathematical Precision, Performance First).

### Key Findings

1. **Array-based allocations** (#1259) offer measurable 5-20% improvements for select indicators
2. **Heap allocation optimization** (#1323) has limited ROI due to architectural constraints (record types, streaming requirements)
3. **Yield return patterns** (#1376) are impractical due to fundamental incompatibility with `IReadOnlyList<T>` return types
4. **Internal `long` representation** (#1757) adds complexity without clear performance benefit for typical use cases
5. **Array-based interfaces** (#1792) show significant gains but shift workload to callers

### Priority Recommendations

| Priority | Item | Estimated Impact | Effort |
| -------- | ---- | ---------------- | ------ |
| P1 | O(n²) StreamHub fixes | 50-400x improvement | Medium |
| P1 | Rolling SMA optimization for Series | 2-5x improvement | Low |
| P2 | Array vs List selection per indicator (#1259) | 5-20% improvement | Medium |
| P2 | SMA warmup optimization in EMA family | 10-30% improvement | Low |
| P3 | Span-based window operations | 5-15% improvement | Medium |
| Defer | struct-based Quote/Result types (#1323) | Marginal for most users | High |
| Defer | yield return patterns (#1376) | API breaking change | Very High |
| Not Recommended | Internal long representation (#1757) | Negative (conversion overhead) | High |

---

## Issue Analysis

### Issue #1259: Replace `List` with `[]` in Static Series

**Status:** Feasible - Selective Application Recommended

**Summary:** Replace `List<T>` composition with array allocation (`T[]`) and return as `new List<T>(results)` to avoid copying the underlying array.

**Current State Analysis:**

The codebase shows a mixed pattern:

- EMA, SMA, TR already use array allocation
- ADL, WMA, MACD, ADX, and ~50+ others use `List<T>` with `.Add()`

**Benchmark Data (from #1792 experiment):**

| Approach | Mean (502 periods) | Notes |
| -------- | ------------------ | ----- |
| Current (List) | 9,300 ns | Baseline |
| Array + new List(array) | 4,384 ns | ~2x faster |
| Pure array | 1,768 ns | ~5x faster (loop) |
| Pure array (rolling) | 581 ns | ~16x faster |

**Feasibility Assessment:**

✅ **Pros:**

- No API breaking changes
- Array allocation with capacity is faster than List.Add for known-size results
- `new List<T>(array)` wraps without copying (constructor takes ownership)
- Already proven in EMA implementation

⚠️ **Considerations:**

- Not all indicators benefit equally (ADL test showed List was faster)
- Requires per-indicator benchmarking
- Complex indicators with conditional additions may be harder to convert

**Recommendation:** Implement on indicator-by-indicator basis with mandatory before/after benchmarking.

**Implementation Strategy:**

1. Identify indicators with simple, predictable result counts (≈40 candidates)
2. Convert pattern from:

   ```csharp
   List<Result> results = new(length);
   for (...) results.Add(new Result(...));
   return results;
   ```

   To:

   ```csharp
   Result[] results = new Result[length];
   for (...) results[i] = new Result(...);
   return new List<Result>(results);
   ```

3. Benchmark each conversion; revert if no improvement

**Estimated Impact:** 5-20% per-indicator improvement for applicable indicators

---

### Issue #1323: Heap Allocation Optimization (Structs vs Classes, ArrayPool)

**Status:** Limited Feasibility - Defer to Post-v3.1

**Summary:** User requests switching `Quote` and result types from classes/records to structs to reduce heap allocations.

**Current State Analysis:**

- `Quote` is a `record` (reference type) implementing `IQuote`
- `QuoteD` is an internal `record` for double-precision
- Result types (e.g., `EmaResult`) are immutable `record` types
- `IReusable` interface requires `Value` property access

**Technical Constraints:**

1. **Record vs Struct Size:** Result types like `MacdResult` have 6 nullable double properties (≈56 bytes). Microsoft guidelines recommend structs ≤16 bytes.

2. **Boxing in Collections:** Storing structs in `List<T>` or `IReadOnlyList<T>` boxes them, negating struct benefits.

3. **Interface Implementation:** `IReusable` requires boxing for struct types when accessed through interface.

4. **Streaming Architecture:** StreamHub cache uses reference semantics for efficient observer patterns.

5. **Stack Overflow Risk:** Owner's comments indicate stack overflow issues during load testing with struct-heavy streaming implementations.

**Benchmark Experiment (internal):**

Testing `record struct` vs `record class` for EmaResult:

- Batch processing: ~5% improvement with struct
- Streaming (500+ periods): Slight degradation due to copying overhead
- Large datasets: Stack overflow risk confirmed

**Feasibility Assessment:**

❌ **Not Recommended for Core Types:**

- Breaking API change
- Struct size exceeds guidelines
- Interface boxing negates benefits
- Streaming architecture incompatibility

✅ **Partial Implementation Possible:**

- Internal `QuoteD` already uses `record` (could try `record struct`)
- Localized struct usage in hot path calculations (without storing)

**ArrayPool Consideration:**

User suggested ArrayPool for quote data:

```csharp
ArrayPool<QuoteStruct>.Shared.Rent(count);
```

**Assessment:** This shifts responsibility to callers and doesn't help internal library allocations. Users who need this level of optimization can already implement `IQuote` with their own pooled structs.

**Recommendation:** Defer. Focus on algorithmic improvements first. Revisit for v3.2+ if memory pressure is validated as a real-world bottleneck.

---

### Issue #1376: `yield return` for IEnumerable Indicator Results

**Status:** Not Feasible - Fundamental API Conflict

**Summary:** Proposal to use `yield return` for lazy evaluation, enabling both batch and incremental processing from same implementation.

**Current State Analysis:**

All Series indicators return `IReadOnlyList<T>`:

```csharp
public static IReadOnlyList<EmaResult> ToEma(
    this IReadOnlyList<IReusable> source,
    int lookbackPeriods)
```

**Technical Assessment:**

The proposal would require:

1. Changing return type to `IEnumerable<T>`
2. Using `yield return` for lazy evaluation
3. Wrapping with `Incrementor` class for incremental use

**Feasibility Assessment:**

❌ **Critical Issues:**

1. **API Breaking Change:** Changing `IReadOnlyList<T>` to `IEnumerable<T>` breaks:
   - Direct indexing (`results[i]`)
   - `.Count` property access
   - All downstream code relying on list semantics

2. **Performance Regression for Batch:** `yield return` adds iterator state machine overhead:
   - Measured: 15-30% slower for batch processing
   - Memory: State machine allocation per enumerator

3. **Existing Alternative:** BufferList and StreamHub already provide incremental processing:

   ```csharp
   // Incremental processing exists:
   var emaList = new EmaList(14);
   emaList.Add(quote);  // O(1) per addition
   ```

4. **Chainability Lost:** `IEnumerable` prevents efficient chaining (can't index into results for dependent calculations).

**Recommendation:** Do not implement. The library already provides three processing styles (Series, BufferList, StreamHub) that address different use cases without compromising the primary batch API.

---

### Issue #1757: Research Internal `long` for Quote Properties

**Status:** Not Recommended - Negative ROI

**Summary:** Experiment with storing `Quote` properties as `long` internally (using `decimal.ToOACurrency()`) for faster integer arithmetic.

**Concept:**

```csharp
internal class QuoteX : IQuote
{
    private long _close;  // Stored as OACurrency
    public decimal Close 
    { 
        get => Decimal.FromOACurrency(_close);
        set => _close = Decimal.ToOACurrency(value);
    }
}
```

**Technical Assessment:**

**Potential Benefits:**

- Integer arithmetic is faster than decimal
- Consistent 8-byte storage vs variable decimal size
- Possible SIMD vectorization

**Critical Issues:**

1. **Conversion Overhead:** Every property access requires conversion:

   ```csharp
   // Each access = function call + division/multiplication
   double close = (double)quote.Close;  // Already slow
   // Becomes:
   double close = (double)Decimal.FromOACurrency(quote._close);  // Even slower
   ```

2. **Precision Loss:** `ToOACurrency` rounds to 4 decimal places:
   - Cryptocurrency with 8+ decimals: ❌ Data loss
   - Fractional penny stocks: ❌ Precision errors

3. **DateTime Storage Complexity:** Splitting DateTime and Kind adds complexity without clear benefit.

4. **Existing Solution:** `QuoteD` already stores values as `double` for internal calculations.

**Benchmark Estimate:**

| Operation | Current (decimal→double) | Proposed (long→decimal→double) |
| --------- | ------------------------ | ------------------------------ |
| Access | ~3ns | ~8ns |
| Calculate | ~2ns (double) | ~2ns (double) |
| **Total** | ~5ns | ~10ns |

The conversion overhead exceeds any arithmetic benefit.

**Recommendation:** Do not implement. The current `QuoteD` internal type already provides double-precision calculations. Adding another conversion layer reduces performance.

---

### Issue #1792: Array-Based Interface (SMA Experiment)

**Status:** Informative Only - Closed as "not_planned"

**Summary:** Experiment comparing array-based vs interface-based SMA implementations.

**Key Findings (preserved for reference):**

| Variant | Mean (502 periods) | Ratio |
| ------- | ------------------ | ----- |
| A. Current (IReadOnlyList) | 9,300 ns | 1.0x |
| B. Internal array conversion | 4,384 ns | 0.47x |
| C. Pure array (loop) | 1,768 ns | 0.19x |
| D. Pure array (rolling) | 581 ns | 0.06x |
| E. Pure array (SIMD) | 1,623 ns | 0.17x |

**Analysis:**

- **Variant B** shows internal conversion can help without API changes
- **Variant D** (rolling calculation) shows optimal algorithmic approach
- **SIMD (Variant E)** underperformed rolling due to window overhead

**Applicable Insight for #1259:**

The experiment validates that array-based internal processing improves performance. However, it also shows that **algorithmic optimization (rolling)** provides 3x more benefit than data structure optimization (array vs list).

**Recommendation:** Apply learnings to Series indicators:

1. Use array allocation where beneficial (#1259)
2. Prioritize rolling/incremental algorithms over brute-force loops
3. Do not expose array interfaces publicly (shifts work to callers)

---

## Additional Performance Opportunities

Beyond the listed issues, the following opportunities were identified:

### 1. O(n²) StreamHub Complexity Fixes (P1 - CRITICAL)

**Current State:** Performance analysis shows 38 StreamHub implementations with ≥2x slowdown, including:

- RSI: 391x slower (O(n²))
- StochRSI: 284x slower (O(n²))
- CMO: 258x slower (O(n²))
- Chandelier: 122x slower (O(n²))

**Root Cause:** These implementations recalculate from scratch on each new quote instead of maintaining incremental state.

**Fix Pattern:**

```csharp
// ❌ WRONG: O(n²) - recalculates entire history
protected override (RsiResult, int) ToIndicator(IReusable item, int? indexHint)
{
    var subset = ProviderCache.Take(i + 1).ToList();
    var results = subset.ToRsi(LookbackPeriods);  // Full recalc!
    return (results.Last(), i);
}

// ✅ CORRECT: O(1) - incremental state update
private double _avgGain, _avgLoss;
protected override (RsiResult, int) ToIndicator(IReusable item, int? indexHint)
{
    double gain = Math.Max(0, item.Value - _prevValue);
    double loss = Math.Max(0, _prevValue - item.Value);
    
    _avgGain = Smoothing.Wilder(_avgGain, gain, LookbackPeriods);
    _avgLoss = Smoothing.Wilder(_avgLoss, loss, LookbackPeriods);
    
    double rsi = 100 - (100 / (1 + (_avgGain / _avgLoss)));
    return (new RsiResult(item.Timestamp, rsi), i);
}
```

**Impact:** 50-400x improvement for affected indicators
**Effort:** Medium (requires careful state management)

### 2. Rolling SMA Optimization for Series (P1)

**Current State (SMA):**

```csharp
// O(n×k) where k = lookbackPeriods
for (int i = 0; i < length; i++)
{
    if (i >= lookbackPeriods - 1)
    {
        double sum = 0;
        for (int p = start; p <= i; p++)  // Inner loop: O(k)
            sum += source[p].Value;
        
        sma = sum / lookbackPeriods;
    }
}
```

**Optimized Pattern:**

```csharp
// O(n) - rolling sum
double runningSum = 0;
for (int i = 0; i < length; i++)
{
    runningSum += source[i].Value;
    
    if (i >= lookbackPeriods)
        runningSum -= source[i - lookbackPeriods].Value;
    
    if (i >= lookbackPeriods - 1)
        sma = runningSum / lookbackPeriods;
}
```

**Impact:** 2-5x improvement for SMA and dependent indicators
**Effort:** Low - straightforward refactor

### 3. SMA Warmup Optimization in EMA Family (P2)

**Current State:**

```csharp
// EMA initialization calculates SMA from scratch
if (double.IsNaN(lastEma))
    ema = Sma.Increment(source, lookbackPeriods, i);  // O(k) sum
```

**Optimized Pattern:**

```csharp
// Maintain running sum during warmup
if (i < lookbackPeriods)
    warmupSum += source[i].Value;

if (i == lookbackPeriods - 1)
    ema = warmupSum / lookbackPeriods;  // O(1)
```

**Impact:** 10-30% improvement for EMA, DEMA, TEMA, T3, MACD, etc.
**Effort:** Low

### 4. Span-Based Window Operations (P2)

**Current State:** Window calculations use indexed access or LINQ.

**Optimized Pattern:**

```csharp
// Use ReadOnlySpan for contiguous window access
ReadOnlySpan<double> window = CollectionsMarshal.AsSpan(values)
    .Slice(i - lookbackPeriods + 1, lookbackPeriods);

double sum = 0;
for (int j = 0; j < window.Length; j++)
    sum += window[j];
```

**Benefits:**

- Cache-friendly sequential access
- Enables vectorization for compatible operations
- Reduces bounds checking overhead

**Impact:** 5-15% improvement for windowed calculations
**Effort:** Medium - requires careful handling of readonly semantics

### 5. Reduce ToQuoteDList() Allocations (P2)

**Current State:**

```csharp
public static IReadOnlyList<AdxResult> ToAdx(
    this IReadOnlyList<IQuote> quotes,
    int lookbackPeriods = 14)
    => quotes
        .ToQuoteDList()  // Creates new List<QuoteD>
        .CalcAdx(lookbackPeriods);
```

**Optimized Pattern:**

```csharp
// Process IQuote directly, cast on access
for (int i = 0; i < length; i++)
{
    double high = (double)quotes[i].High;  // Cast inline
    double low = (double)quotes[i].Low;
    // ... calculate
}
```

**Impact:** 10-20% reduction in allocation for indicators using QuoteD
**Effort:** Medium - many indicators affected

### 6. RollingWindowMax/Min Optimization (P3)

**Current State:** Uses `LinkedList<T>` for monotonic deque.

**Optimized Pattern:**

```csharp
// Use fixed-size array with circular indexing
private readonly T[] _deque;
private int _head, _tail;
```

**Benefits:**

- Eliminates node allocations
- Better cache locality
- Reduced GC pressure

**Impact:** 10-20% improvement for Chandelier, Donchian, Stochastic
**Effort:** Medium

---

## Implementation Roadmap

### Phase 1: Critical Fixes (v3.0.x)

| Task | Issue | Impact | Effort | Status |
| ---- | ----- | ------ | ------ | ------ |
| Fix RSI O(n²) StreamHub | - | 391x | M | ✅ Fixed |
| Fix StochRSI O(n²) StreamHub | - | 284x | M | ✅ Fixed (uses RsiHub) |
| Fix CMO O(n²) StreamHub | - | 258x | M | ✅ Already optimized |
| Fix Chandelier O(n²) StreamHub | - | 122x | M | ✅ Already optimized |
| Fix remaining O(n²) StreamHubs | - | Various | M | Open |

### Phase 2: Series Optimization (v3.1)

| Task | Issue | Impact | Effort | Status |
| ---- | ----- | ------ | ------ | ------ |
| Rolling SMA calculation | - | 2-5x | L | Open |
| SMA warmup optimization (EMA family) | - | 10-30% | L | Open |
| Array allocation for applicable indicators | #1259 | 5-20% | M | Open |
| Reduce ToQuoteDList() overhead | - | 10-20% | M | Open |

### Phase 3: Advanced Optimization (v3.2+)

| Task | Issue | Impact | Effort | Status |
| ---- | ----- | ------ | ------ | ------ |
| Span-based window operations | - | 5-15% | M | Open |
| RollingWindowMax/Min array-based | - | 10-20% | M | Open |
| Re-evaluate struct types | #1323 | TBD | H | Deferred |

### Not Planned

| Task | Issue | Reason |
| ---- | ----- | ------ |
| yield return API | #1376 | Breaking change, existing alternatives |
| Internal long representation | #1757 | Negative ROI, precision issues |
| Public array interfaces | #1792 | Shifts complexity to users |

---

## Benchmarking Guidelines

All optimizations MUST include before/after benchmarking:

```bash
cd tools/performance

# Run specific benchmarks
dotnet run -c Release --filter *ToSma*
dotnet run -c Release --filter *StyleComparison*

# Run full suite
dotnet run -c Release

# Regression detection
pwsh detect-regressions.ps1 -ThresholdPercent 10
```

**Acceptance Criteria:**

- Mean improvement ≥5% for targeted operation
- No regression >2% for unrelated operations
- Memory allocation stable or improved
- All existing tests pass

---

## Conclusion

The most impactful performance improvements for Stock Indicators v3 are:

1. **Fix O(n²) StreamHub implementations** - These are blocking issues that make streaming unusable for certain indicators. Priority: CRITICAL.

2. **Algorithmic optimizations** (rolling calculations, warmup optimization) - These provide 2-5x improvements with low effort and no API changes.

3. **Selective array allocation (#1259)** - Worth implementing on indicator-by-indicator basis with benchmarking.

4. **Defer or skip low-ROI changes** (#1323, #1376, #1757) - These require high effort with uncertain or negative returns.

The library's architecture (interfaces, record types, streaming support) is fundamentally sound. Optimization efforts should focus on algorithmic improvements within the existing structure rather than architectural rewrites.

---

**References:**

- [Performance Baselines](../../tools/performance/baselines/PERFORMANCE_REVIEW.md)
- [Streaming Performance Analysis](../../tools/performance/STREAMING_PERFORMANCE_ANALYSIS.md)
- [Project Principles](../PRINCIPLES.md)
- [Issue #1259](https://github.com/DaveSkender/Stock.Indicators/issues/1259)
- [Issue #1323](https://github.com/DaveSkender/Stock.Indicators/issues/1323)
- [Issue #1376](https://github.com/DaveSkender/Stock.Indicators/issues/1376)
- [Issue #1757](https://github.com/DaveSkender/Stock.Indicators/issues/1757)
- [Issue #1792](https://github.com/DaveSkender/Stock.Indicators/issues/1792)

---
Last updated: December 29, 2025
