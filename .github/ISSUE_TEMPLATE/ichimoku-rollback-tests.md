---
name: Add comprehensive rollback validation tests for Ichimoku StreamHub
about: Implement full Insert/Remove test pattern for Ichimoku indicator
title: 'test: Add comprehensive rollback validation for Ichimoku StreamHub'
labels: enhancement, test, StreamHub
assignees: ''
---

## Problem

The Ichimoku StreamHub indicator has a complex ChikouSpan calculation that fails when comprehensive rollback validation tests (with Insert/Remove operations) are applied. The current test implementation only validates basic streaming functionality without testing late arrival (Insert) and removal scenarios.

## Root Cause

The ChikouSpan value is shifted by `kijun` periods (26 by default). After an Insert operation at index 80, the ChikouSpan values for indices 54-63 are off by one index (using quotes[81] instead of quotes[80]). This indicates an issue with either:
1. The Insert operation placement in ProviderCache
2. The interaction between OnAdd backfill and ToIndicator during rebuilds
3. The ChikouSpan backfill pattern in OnAdd

## Required Test Cases

The Ichimoku QuoteObserver test should include (following the canonical EMA/ADX pattern):

1. **Prefill warmup**: Add 20-30 quotes before observer initialization
2. **Streaming with skip**: Stream remaining quotes, skip quote at index 80
3. **Duplicate handling**: Resend duplicates for indices 101-104
4. **Late arrival (Insert)**: Insert the skipped quote and verify Series parity
5. **Removal**: Remove quote at `removeAtIndex` (495) and verify revised Series parity
6. **Strict ordering**: Use `BeEquivalentTo(series, o => o.WithStrictOrdering())`
7. **Cleanup**: Call Unsubscribe() and EndTransmission()

## Reference

- **Project task**: T177 (Comprehensive Rollback Validation)
- **Task file**: `.specify/specs/001-develop-streaming-indicators/tasks.md`
- **Canonical pattern**: `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` QuoteObserver method
- **Current test**: `tests/indicators/e-k/Ichimoku/Ichimoku.StreamHub.Tests.cs`
- **Implementation**: `src/e-k/Ichimoku/Ichimoku.StreamHub.cs`

## Investigation Needed

1. Debug Insert operation to verify correct ProviderCache placement
2. Trace OnAdd backfill logic during rebuilds (ToIndicator calls)
3. Consider alternative ChikouSpan backfill approach (perhaps only in ToIndicator)
4. Verify RollbackState implementation correctly handles shifted values

## Success Criteria

- All comprehensive rollback test cases pass
- ChikouSpan values match Series calculations after Insert/Remove operations
- No off-by-one errors in shifted value indices
