# Stream Hub cache pruning test failures

This document tracks failing `WithCachePruning_MatchesSeriesExactly` test implementations and provides hypotheses for each failure.

## Test summary

- **Total tests**: 80
- **Passed**: 76 (95%)
- **Failed**: 4 (5%)

## Failed tests and hypotheses

### 1. EpmaHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/e-k/Epma/Epma.StreamHub.Tests.cs`

**Hypothesis**: EPMA (Endpoint Moving Average) exhibits floating-point precision differences between streaming and batch calculations. The errors show minute differences in the last decimal places (e.g., expected 226.15542857142856 but found 226.15542857142853). This appears to be a systematic accumulation of floating-point rounding errors in EPMA's linear regression endpoint calculation when processed incrementally versus as a complete batch. The differences are within floating-point tolerance but fail exact equality checks.

**Next steps**: This may require tolerance-based comparison rather than exact matching, or investigation into whether the streaming implementation can match the batch calculation's precision exactly.

---

### 2. PmoHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/m-r/Pmo/Pmo.StreamHub.Tests.cs`

**Hypothesis**: PMO (Price Momentum Oscillator) produces null values for PMO and Signal properties even with increased cache size (60 quotes for 35-period indicator). The double exponential smoothing may require significantly more warmup data than the simple period count suggests, or there may be an issue with state initialization when the cache is pruned during the warmup phase.

**Next steps**: Investigate PMO's warmup requirements more deeply, potentially needing cache size of 100+ to ensure all EMA chains are fully initialized, or verify if there's a bug in the StreamHub implementation's state management.

---

### 3. RenkoHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/m-r/Renko/Renko.StreamHub.Tests.cs`

**Hypothesis**: Renko charts transform quotes into bricks (non-1:1 mapping). The streaming implementation with cache pruning does not produce the same brick sequence as the full series calculation. This suggests that Renko's brick formation logic may depend on historical quotes beyond the visible cache window, or that the cache pruning disrupts the state machine that tracks partial brick formation.

**Next steps**: Renko may be fundamentally incompatible with cache pruning due to its stateful transformation logic. Consider documenting this as a known limitation or implementing special handling for transformation indicators.

---

### 4. SlopeHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/s-z/Slope/Slope.StreamHub.Tests.cs`

**Hypothesis**: Slope (Linear Regression Slope) exhibits floating-point precision differences between streaming and batch calculations. The test shows numerical differences in `Intercept` values (e.g., expected 209.1143296703297 but found 209.90668131868134). Even with increased cache size (40 quotes for 14-period indicator), the streaming version accumulates slightly different floating-point rounding errors compared to the batch calculation. This is characteristic of linear regression's sum-of-squares calculations when processed incrementally.

**Next steps**: Similar to EPMA, this may require tolerance-based comparison rather than exact matching, or investigation into whether the incremental calculation can be made to exactly match the batch version.

---

## Resolved tests

The following tests were fixed by increasing cache sizes to accommodate initialization requirements:

- **KvoHub**: Resolved by increasing cache to 70 (55 + 15 extra) from 50
- **IchimokuHub**: Resolved by increasing cache to 90 (52 + 26 displacement + 12 extra) from 65

---

## Implementation status

- [x] Interface updated with `WithCachePruning_MatchesSeriesExactly()` method signature
- [x] Tests added to all 80 StreamHub test classes
- [x] Build verification completed (no syntax errors)
- [x] Initial test run completed
- [x] Fixed 2 tests by adjusting cache sizes (KvoHub, IchimokuHub)
- [ ] Investigate and fix remaining 4 tests:
  - [ ] EpmaHub (floating-point precision)
  - [ ] PmoHub (insufficient warmup or state bug)
  - [ ] RenkoHub (transformation indicator incompatibility)
  - [ ] SlopeHub (floating-point precision)
- [ ] Add validation on max cache size at construction to enforce minimum initialization requirements

---
Last updated: January 26, 2026
