# Feature Specification: Fix Streaming Performance Issues

**Feature ID:** 002  
**Status:** Planning  
**Created:** October 19, 2025

## Overview

Fix critical performance regressions in StreamHub and BufferList indicator implementations that show significant slowdowns (up to 391x) compared to their Series counterparts. Analysis reveals O(n²) complexity issues, missing incremental state management, and inefficient window operations.

## User Stories

### Priority 1 (P1) - Critical: Fix O(n²) Complexity Issues

**US1: As a developer, I need RSI StreamHub to perform with O(n) complexity so that streaming real-time data is feasible**

- **Current State:** RSI StreamHub is 391x slower than Series (7,073ns vs 2,767,983ns)
- **Root Cause:** Recalculating entire indicator history on each new quote instead of incremental updates
- **Acceptance Criteria:**
  - RSI StreamHub performance ≤1.5x slower than Series
  - Time complexity verified as O(n) by testing with 10x data
  - Regression tests pass (bit-for-bit parity with Series)
  - Memory usage linear with window size, not total quote count

**US2: As a developer, I need StochRsi StreamHub to perform with O(n) complexity**

- **Current State:** StochRsi StreamHub is 284x slower (31,449ns vs 8,916,843ns)
- **Root Cause:** Likely calling RSI which already has O(n²) issue, compounding the problem
- **Acceptance Criteria:**
  - StochRsi StreamHub performance ≤1.5x slower than Series
  - Depends on US1 (RSI fix)
  - Regression tests pass
  - O(n) complexity verified

**US3: As a developer, I need CMO StreamHub to perform with O(n) complexity**

- **Current State:** CMO StreamHub is 258x slower (15,321ns vs 3,949,622ns)
- **Root Cause:** Similar pattern to RSI, likely O(n²) windowing
- **Acceptance Criteria:**
  - CMO StreamHub performance ≤1.5x slower than Series
  - Implements incremental gain/loss tracking
  - Regression tests pass
  - O(n) complexity verified

**US4: As a developer, I need Chandelier StreamHub to perform with O(n) complexity**

- **Current State:** Chandelier StreamHub is 122x slower (27,156ns vs 3,303,438ns)
- **Root Cause:** Excessive lookback operations, rescanning entire window on each quote
- **Acceptance Criteria:**
  - Chandelier StreamHub performance ≤1.5x slower than Series
  - Uses efficient max/min tracking (deque or circular buffer)
  - Regression tests pass
  - O(n) complexity verified

### Priority 2 (P2) - High: Fix EMA Family State Management

**US5: As a developer, I need EMA StreamHub to use proper incremental state management**

- **Current State:** EMA StreamHub is 10.6x slower (2,749ns vs 29,165ns)
- **Root Cause:** Missing proper EMA state management, likely recalculating from scratch
- **Acceptance Criteria:**
  - EMA StreamHub performance ≤1.5x slower than Series
  - Uses single state variable: `EMA[t] = α × Price[t] + (1 - α) × EMA[t-1]`
  - No recalculation needed
  - Regression tests pass

**US6: As a developer, I need all EMA-family indicators to use efficient state management**

- **Affected Indicators:** SMMA (10.4x), DEMA (9.3x), TEMA (10.7x), T3 (9.9x), TRIX (9.2x), MACD (6.9x)
- **Root Cause:** All depend on EMA; systemic issue with EMA state management
- **Acceptance Criteria:**
  - All EMA-family StreamHub implementations ≤1.5x slower than Series
  - Depends on US5 (EMA fix)
  - Each maintains proper incremental state
  - Regression tests pass for all

### Priority 3 (P3) - Medium: Optimize Window Operations

**US7: As a developer, I need SMA/WMA/VWMA/ALMA StreamHub to use efficient sliding windows**

- **Current State:** SMA (3.0x), WMA (2.5x), VWMA (3.8x), ALMA (7.6x) slower than Series
- **Root Cause:** Not using circular buffers or efficient sliding window techniques
- **Acceptance Criteria:**
  - All window-based StreamHub implementations ≤1.5x slower than Series
  - Uses circular buffers for window management
  - SMA: tracks running sum (add new value, subtract old value)
  - WMA: tracks weighted sums efficiently
  - Regression tests pass

**US8: As a developer, I need Slope BufferList to optimize regression calculations**

- **Current State:** Slope BufferList is 7.9x slower (43,086ns vs 338,188ns)
- **Root Cause:** May be recalculating regression on entire history
- **Acceptance Criteria:**
  - Slope BufferList performance ≤1.5x slower than Series
  - Uses incremental regression calculation where possible
  - Regression tests pass

**US9: As a developer, I need Alligator/Gator/Fractal BufferList to perform efficiently**

- **Current State:** Alligator (5.0x), Gator (3.9x), Fractal (3.8x) slower than Series
- **Root Cause:** Architectural issues with BufferList for certain patterns
- **Acceptance Criteria:**
  - Each BufferList implementation ≤1.5x slower than Series
  - Review and optimize buffer management strategies
  - Regression tests pass

### Priority 4 (P4) - Low: Fine-tune Remaining Implementations

**US10: As a developer, I need to optimize indicators with 1.3x-2x slowdown**

- **Affected:** 6 StreamHub and 21 BufferList implementations showing moderate slowdown
- **Root Cause:** Minor inefficiencies (allocations, LINQ in hot paths, missing span optimizations)
- **Acceptance Criteria:**
  - Reduce allocations where possible
  - Use spans instead of collections in hot paths
  - Cache intermediate calculations
  - All implementations ≤1.5x slower than Series

## Success Criteria

### Algorithmic Success (NON-NEGOTIABLE)

- ✅ All StreamHub indicators eliminate O(n²) complexity (achieve O(1) per-quote incremental updates)
- ✅ All BufferList indicators eliminate O(n²) complexity
- ✅ Memory usage linear with window size, not total quote count
- ✅ All regression tests pass (bit-for-bit parity with Series)

### Performance Success (TARGET)

- ⚠️ All StreamHub indicators ≤1.5x slower than Series baseline (architectural overhead target)
- ⚠️ All BufferList indicators ≤1.5x slower than Series baseline (architectural overhead target)
- ✅ Performance benchmarks updated with new baseline

**Note on Success Criteria**: The primary success metric is **eliminating O(n²) complexity** and achieving **O(1) per-quote incremental updates**. The ≤1.5x slowdown threshold represents an aspirational **architectural overhead target** for streaming implementations (StreamHub/BufferList) compared to batch processing (Series). This is not a performance regression allowance within a single implementation style—the constitution's requirement that "regressions >2% mean... MUST be justified" applies to changes within the same implementation (Series-to-Series), not cross-style comparisons (Series-to-StreamHub).

Streaming architectures maintain incremental state and provide real-time updates, which inherently requires some overhead compared to batch processing that can optimize for full-dataset analysis. **Indicators that achieve O(1) incremental updates but exceed ≤1.5x due to architectural constraints are still considered successful** if they provide usable streaming performance.

## Testing Strategy

For each fixed indicator:

1. **Correctness:** Run regression tests against Series baseline
2. **Performance:** Verify <1.5x slowdown vs Series via benchmarks
3. **Complexity:** Verify O(n) time complexity by testing with 10x data
4. **Memory:** Check for memory leaks in long-running streams

## MVP Scope

**MVP = User Story 1 (RSI fix)**

The RSI fix is the highest-impact single change:

- Most critical slowdown (391x)
- Blocks StochRsi fix (US2)
- Demonstrates the pattern for fixing other O(n²) issues
- Immediately makes RSI streaming usable

After MVP, incrementally deliver each user story independently.

## Dependencies

- US2 (StochRsi) depends on US1 (RSI)
- US6 (EMA-family) depends on US5 (EMA)
- All other user stories are independent and can be parallelized

## Reference Documentation

- **Performance Analysis:** `tools/performance/baselines/PERFORMANCE_REVIEW.md`
- **Baseline Data:** `tools/performance/baselines/*.json`
- **Constitution:** `.specify/memory/constitution.md` (Mathematical Precision requirements)
- **Implementation Guides:**
  - `src/agents.md` (Formula change rules)
  - `.github/instructions/indicator-stream.instructions.md`
  - `.github/instructions/indicator-buffer.instructions.md`
  - `.github/instructions/source-code-completion.instructions.md`

## Out of Scope

- New indicator features
- API changes or breaking changes
- Documentation updates (except inline code comments)
- Formula modifications (only performance optimization)
