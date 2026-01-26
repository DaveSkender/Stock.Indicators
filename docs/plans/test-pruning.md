# Stream Hub cache pruning test failures

This document tracks failing `WithCachePruning_MatchesSeriesExactly` test implementations and provides hypotheses for each failure.

## Test summary

- **Total tests**: 80
- **Passed**: 77 (96.25%)
- **Failed**: 3 (3.75%)

## Failed tests and hypotheses

### 1. EpmaHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/e-k/Epma/Epma.StreamHub.Tests.cs`

**Current cache**: 100 quotes (20-period + 80 extra)

**Hypothesis**: EPMA (Endpoint Moving Average) exhibits floating-point precision differences between streaming and batch calculations even with large cache (100 quotes for 20-period indicator). The errors show minute differences in the last decimal places (e.g., expected 225.54157142857142 but found 225.54157142857144). This suggests a fundamental difference in the calculation path between incremental streaming and batch processing, rather than insufficient warmup. The EPMA algorithm uses linear regression endpoints which may accumulate rounding errors differently when computed incrementally versus in one pass.

**Evidence needed**: Investigation into EPMA streaming vs batch algorithm to identify where calculation paths diverge. Not a tolerance/approximation issue per guidance - must be mathematical equivalence problem.

---

### 2. RenkoHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/m-r/Renko/Renko.StreamHub.Tests.cs`

**Current cache**: 100 quotes (sufficient for transformation)

**Hypothesis**: Renko charts transform quotes into bricks (non-1:1 mapping). The streaming implementation with cache pruning does not produce the same brick sequence as the full series calculation. Renko's brick formation logic depends on price movement patterns, and cache pruning may be removing quotes needed to maintain proper brick state. The algorithm may need timestamp-based pruning strategy rather than count-based pruning to preserve brick formation integrity.

**Evidence needed**: Investigation into whether Renko pruning needs timestamp-based approach as suggested in review comments, or if there's a state preservation issue in the StreamHub implementation.

---

### 3. SlopeHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/s-z/Slope/Slope.StreamHub.Tests.cs`

**Current cache**: 100 quotes (14-period + 86 extra)

**Hypothesis**: Slope (Linear Regression Slope) exhibits floating-point precision differences between streaming and batch calculations even with large cache (100 quotes for 14-period indicator). The test shows numerical differences in `Intercept` values (e.g., expected 209.1143296703297 but found 209.90668131868134). These are not tiny last-decimal-place differences like EPMA, but larger variations suggesting the streaming algorithm follows a different calculation path. Linear regression's sum-of-squares calculations may be computed differently in incremental vs batch mode.

**Evidence needed**: Investigation into Slope/linear regression streaming vs batch algorithm to identify where calculation paths diverge. Not a tolerance/approximation issue per guidance - must be mathematical equivalence problem.

---

## Resolved tests

The following tests were fixed by increasing cache sizes to accommodate initialization requirements:

- **KvoHub**: Resolved by increasing cache to 70 (55-period + 15 extra) from 50
- **IchimokuHub**: Resolved by increasing cache to 90 (52-period + 26 displacement + 12 extra) from 65
- **PmoHub**: Resolved by increasing cache to 100 (35-period + 65 extra) from 60
- **HurstHub**: Resolved by increasing cache to 120 (100-period + 20 extra) from 50

---

## Implementation status

- [x] Interface updated with `WithCachePruning_MatchesSeriesExactly()` method signature
- [x] Tests added to all 80 StreamHub test classes
- [x] Build verification completed (no syntax errors)
- [x] Initial test run completed
- [x] Fixed 4 tests by adjusting cache sizes (KvoHub, IchimokuHub, PmoHub, HurstHub)
- [x] Fixed build errors from base branch PR #1939 (MaxCacheSize now constructor parameter)
- [x] Format checks passed
- [ ] Investigate and resolve remaining 3 tests (require algorithm investigation, not cache adjustments):
  - [ ] EpmaHub (floating-point precision - algorithm divergence)
  - [ ] RenkoHub (timestamp-based pruning strategy needed)
  - [ ] SlopeHub (floating-point precision - algorithm divergence)
- [ ] Implement validation on max cache size at construction to enforce minimum initialization requirements for all StreamHub indicators

---
Last updated: January 26, 2026
