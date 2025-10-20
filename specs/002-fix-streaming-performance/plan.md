# Implementation Plan: Fix Streaming Performance Issues

**Feature ID:** 002  
**Status:** Planning  
**Created:** October 19, 2025

## Tech Stack

**Language & Runtime:**

- C# / .NET 9.0 and .NET 8.0 (multi-target)
- BenchmarkDotNet 0.15.4 for performance testing

**Testing:**

- xUnit for unit and regression tests
- BenchmarkDotNet for performance validation
- Existing regression test baselines for correctness validation

**Development Tools:**

- Roslynator for code analysis
- .editorconfig for code style enforcement
- Performance baseline JSON files for before/after comparison

## Project Structure

```text
src/
├── m-r/
│   ├── Rsi/
│   │   ├── Rsi.Series.cs          # Canonical reference (no changes)
│   │   ├── Rsi.StreamHub.cs       # [US1] Fix O(n²) → O(n)
│   │   └── Rsi.BufferList.cs
│   └── ...
├── s-z/
│   ├── Stoch/
│   │   ├── StochRsi.Series.cs     # Canonical reference
│   │   ├── StochRsi.StreamHub.cs  # [US2] Fix after RSI
│   │   └── ...
│   ├── Sma/
│   │   ├── Sma.StreamHub.cs       # [US7] Optimize window
│   │   └── ...
│   └── Slope/
│       ├── Slope.BufferList.cs    # [US8] Optimize regression
│       └── ...
├── a-d/
│   ├── Cmo/
│   │   ├── Cmo.StreamHub.cs       # [US3] Fix O(n²)
│   │   └── ...
│   ├── Chandelier/
│   │   ├── Chandelier.StreamHub.cs # [US4] Fix lookback
│   │   └── ...
│   └── ...
├── e-k/
│   ├── Ema/
│   │   ├── Ema.StreamHub.cs       # [US5] Fix state mgmt
│   │   └── ...
│   ├── Gator/
│   │   ├── Gator.BufferList.cs    # [US9] Optimize
│   │   └── ...
│   └── ...
└── _common/
    └── (shared utilities - may need circular buffer helper)

tests/
├── indicators/
│   ├── m-r/
│   │   ├── Rsi/
│   │   │   └── Rsi.Regression.Tests.cs  # Validate RSI fix
│   │   └── ...
│   ├── s-z/
│   │   ├── Stoch/
│   │   │   └── StochRsi.Regression.Tests.cs
│   │   └── ...
│   └── ...

tools/performance/
├── baselines/
│   ├── Performance.SeriesIndicators-report-full.json
│   ├── Performance.StreamIndicators-report-full.json
│   ├── Performance.BufferIndicators-report-full.json
│   ├── Performance.StyleComparison-report-full.json
│   ├── PERFORMANCE_REVIEW.md           # Analysis document
│   └── analyze_performance.py          # Analysis script
├── SeriesIndicators.cs                 # Benchmark definitions
├── StreamIndicators.cs
└── BufferIndicators.cs
```

## Implementation Strategy

### Phase 1: Setup & Infrastructure

1. **Baseline capture:** Preserve current performance baselines for comparison
2. **Test verification:** Ensure all regression tests pass before any changes
3. **Shared utilities:** Create circular buffer/deque helper if needed for window operations

### Phase 2: MVP - Critical O(n²) Fix (US1)

**Focus:** RSI StreamHub (391x slowdown → ≤1.5x)

**Pattern to establish:**

- Replace full recalculation with incremental state updates
- Implement Wilder's smoothing as rolling averages
- Track gain/loss incrementally
- Validate with regression tests
- Benchmark to confirm ≤1.5x Series performance

**Deliverable:** Working RSI StreamHub with O(n) complexity

### Phase 3: Expand O(n²) Fixes (US2-US4)

**Apply RSI pattern to:**

- StochRsi (depends on RSI fix)
- CMO (similar to RSI)
- Chandelier (add efficient max/min tracking)
- Stoch (efficient rolling max/min)

**Parallel opportunities:** CMO, Chandelier, Stoch can be done in parallel after RSI

### Phase 4: EMA Family Fixes (US5-US6)

**Focus:** Fix systemic EMA state management issue

**Pattern:**

- EMA: Single state variable, incremental formula
- SMMA, DEMA, TEMA, T3, TRIX: Apply same pattern
- MACD: Benefits from EMA fix

**Parallel opportunities:** After EMA is fixed, all dependent indicators can be updated in parallel

### Phase 5: Window Optimizations (US7-US9)

**Focus:** Efficient sliding window implementations

**Pattern:**

- Circular buffers for fixed-size windows
- SMA: Running sum (add new, subtract old)
- WMA: Incremental weighted sum updates
- Slope: Incremental regression where possible

**Parallel opportunities:** SMA/WMA/VWMA/ALMA (StreamHub) and Slope/Alligator/Gator/Fractal (BufferList) are independent

### Phase 6: Fine-tuning (US10)

**Focus:** Minor optimizations across remaining indicators

**Pattern:**

- Reduce allocations
- Use spans in hot paths
- Cache intermediate values
- Remove unnecessary LINQ

**Parallel opportunities:** All 27 indicators are independent

### Phase 7: Validation & Documentation

1. **Re-run benchmarks:** Generate new baseline files
2. **Compare results:** Verify all indicators ≤1.5x Series
3. **Update analysis:** Re-run analyze_performance.py
4. **Update documentation:** Code comments, inline XML docs

## Testing Approach

**Per User Story:**

1. Run existing regression tests (must pass - no formula changes)
2. Run performance benchmarks (verify ≤1.5x target)
3. Complexity test: Run with 10x data, verify linear scaling
4. Memory test: Long-running stream validation

**Final Validation:**

1. Full benchmark suite across all fixed indicators
2. Generate new baseline JSON files
3. Run analysis script to confirm all targets met
4. Regression test suite (full)

## Key Constraints

### CRITICAL: No Formula Changes

From `src/agents.md`:

- **NEVER modify existing indicator formulas** without explicit authorization
- Series implementations are the canonical source of truth
- StreamHub/BufferList must match Series results bit-for-bit
- Only performance optimizations allowed (structure, state management, algorithms)

### Code Quality Standards

From `.github/instructions/source-code-completion.instructions.md`:

- Maintain `.editorconfig` conventions
- Pass Roslynator analysis
- XML documentation for public APIs
- Use `/// <inheritdoc />` where appropriate

### Performance Targets

- All StreamHub indicators ≤1.5x slower than Series
- All BufferList indicators ≤1.5x slower than Series
- O(n) time complexity (verified with 10x data test)
- Linear memory usage (proportional to window size only)

## Anti-Patterns to Avoid

**❌ WRONG: Recalculating from scratch**

```csharp
public void Add(Quote quote)
{
    quotes.Add(quote);
    var result = quotes.ToRsi(period); // O(n) work every time!
    results.Add(result.Last());
}
```

**✅ CORRECT: Incremental update**

```csharp
private double prevGainAvg, prevLossAvg;
public void Add(Quote quote)
{
    double gain = Math.Max(0, quote.Close - prevClose);
    double loss = Math.Max(0, prevClose - quote.Close);
    
    // Wilder's smoothing (incremental)
    double avgGain = (prevGainAvg * (period - 1) + gain) / period;
    double avgLoss = (prevLossAvg * (period - 1) + loss) / period;
    
    prevGainAvg = avgGain;
    prevLossAvg = avgLoss;
    prevClose = quote.Close;
    
    double rs = avgGain / avgLoss;
    double rsi = 100 - (100 / (1 + rs));
    results.Add(rsi);
}
```

## Dependencies

- **US2 depends on US1:** StochRsi uses RSI internally
- **US6 depends on US5:** EMA-family indicators depend on EMA
- All other user stories are independent

## Success Metrics

- **Before:** 44 StreamHub indicators averaging 28.5x slower
- **After:** All StreamHub indicators ≤1.5x slower
- **Before:** 6 critical BufferList indicators 2-8x slower
- **After:** All BufferList indicators ≤1.5x slower
- **Regression:** 0 test failures (bit-for-bit parity maintained)

## Risk Mitigation

1. **One indicator at a time:** Minimize blast radius
2. **Regression tests first:** Catch any formula changes immediately
3. **Series as baseline:** Always compare against canonical reference
4. **Performance validation:** Benchmark after each fix
5. **Git workflow:** Feature branch per user story for easy rollback
