# Stream Hub cache pruning test failures

This document tracks failing `WithCachePruning_MatchesSeriesExactly` test implementations and provides hypotheses for each failure.

## Test summary

- **Total tests**: 80
- **Passed**: 74 (92.5%)
- **Failed**: 6 (7.5%)

## Failed tests and hypotheses

### 1. KvoHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/e-k/Kvo/Kvo.StreamHub.Tests.cs`

**Hypothesis**: KVO (Klinger Volume Oscillator) likely has complex state dependencies that require more historical data than the cache size allows. The oscillator may need extended lookback beyond the 50-quote cache to properly calculate its exponential moving averages and volume force calculations. The test may be comparing truncated results against full-series calculations that had complete warmup context.

**Next steps**: Verify warmup period requirements and adjust test expectations or increase cache size for proper validation.

---

### 2. IchimokuHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/e-k/Ichimoku/Ichimoku.StreamHub.Tests.cs`

**Hypothesis**: Ichimoku Cloud has multiple components (Tenkan-sen, Kijun-sen, Senkou Span A/B, Chikou Span) with varying lookback periods (typically 9, 26, 52 periods). With a 50-quote cache, the longer-period calculations (52-period) would not have enough historical data. Additionally, Senkou Span B is shifted forward 26 periods, creating complex state dependencies that may not work correctly with cache pruning.

**Next steps**: Increase cache size to accommodate the longest lookback period plus displacement, or document that Ichimoku requires special handling for cache pruning scenarios.

---

### 3. EpmaHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/e-k/Epma/Epma.StreamHub.Tests.cs`

**Hypothesis**: EPMA (Endpoint Moving Average) uses a unique calculation method that may be particularly sensitive to the starting point of the data series. When the cache is pruned, the EPMA calculation starts from a different initial state than the full series, leading to divergent results even in the steady-state portion of the indicator.

**Next steps**: Investigate EPMA's initialization logic and determine if state needs to be preserved across cache pruning or if the test expectations need adjustment.

---

### 4. PmoHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/m-r/Pmo/Pmo.StreamHub.Tests.cs`

**Hypothesis**: PMO (Price Momentum Oscillator) uses double exponential smoothing with potentially long lookback periods. The calculation depends heavily on previous EMA values, and cache pruning may discard critical state information needed for accurate continuation. The test failure suggests the pruned cache doesn't maintain sufficient historical context for the double-smoothed momentum calculations.

**Next steps**: Review PMO's state management in StreamHub implementation and verify that warmup state is properly maintained or that cache size is sufficient for the indicator's requirements.

---

### 5. RenkoHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/m-r/Renko/Renko.StreamHub.Tests.cs`

**Hypothesis**: Renko charts transform quotes into variable brick counts (non-1:1 timestamp mappings). The test failure shows only 3 results instead of the expected 50, indicating that Renko's brick formation logic fundamentally differs from standard indicators. When 100 quotes are streamed with a 50-quote cache, the Renko algorithm produces far fewer bricks than quotes. Cache pruning may be removing quotes that are still needed to complete in-progress bricks, or the test assumption that result count equals cache size is invalid for transformation indicators.

**Next steps**: Renko requires special test logic that accounts for quote-to-brick transformation ratios. The test may need to validate correctness of brick values rather than count, or skip cache pruning validation for transformation-based indicators.

---

### 6. SlopeHubTests.WithCachePruning_MatchesSeriesExactly

**File**: `tests/indicators/s-z/Slope/Slope.StreamHub.Tests.cs`

**Hypothesis**: Slope (Linear Regression Slope) uses a rolling window for least-squares calculations. The test failure shows numerical differences in `Intercept` values (e.g., expected 223.72193406593408 but found 223.7412967032967), suggesting precision differences rather than complete calculation failure. When the cache is pruned, the streaming version may be accumulating slightly different floating-point rounding errors compared to the batch calculation on the full series. This is particularly likely with the sum-of-squares calculations in linear regression.

**Next steps**: Verify if the differences are within acceptable floating-point tolerance. May need to use approximate equality checks rather than exact matching, or investigate if StreamHub's incremental calculation introduces systematic error.

---

## Implementation status

- [x] Interface updated with `WithCachePruning_MatchesSeriesExactly()` method signature
- [x] Tests added to all 80 StreamHub test classes
- [x] Build verification completed (no syntax errors)
- [x] Initial test run completed
- [ ] Fix failing tests (KvoHub, IchimokuHub, EpmaHub, PmoHub, RenkoHub, SlopeHub)

---
Last updated: January 26, 2026
