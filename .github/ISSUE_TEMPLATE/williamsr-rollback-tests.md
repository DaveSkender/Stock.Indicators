---
name: Add comprehensive rollback validation tests for WilliamsR StreamHub
about: Implement full Insert/Remove test pattern for WilliamsR indicator
title: 'test: Add comprehensive rollback validation for WilliamsR StreamHub'
labels: enhancement, test, StreamHub
assignees: ''
---

## Problem

The WilliamsR StreamHub indicator currently has basic rollback validation but doesn't follow the full comprehensive pattern established for other indicators. The test should be enhanced to match the canonical EMA/ADX pattern.

## Required Test Cases

The WilliamsR QuoteObserver test should include (following the canonical EMA/ADX pattern):

1. **Prefill warmup**: Add 20-30 quotes before observer initialization
2. **Streaming with skip**: Stream remaining quotes, skip quote at index 80
3. **Duplicate handling**: Resend duplicates for indices 101-104
4. **Late arrival (Insert)**: Insert the skipped quote and verify Series parity
5. **Removal**: Remove quote at `removeAtIndex` constant (495) from base class, not hardcoded value
6. **Revised series parity**: Use RevisedQuotes from base class for comparison
7. **Strict ordering**: Use `BeEquivalentTo(series, o => o.WithStrictOrdering())`
8. **Cleanup**: Call Unsubscribe() and EndTransmission()

## Key Requirements

- Must use base class `removeAtIndex` constant (495) for consistency across tests
- Must use base class `RevisedQuotes` for revised series comparison
- Should follow exact pattern from `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs`

## Reference

- **Project task**: T177 (Comprehensive Rollback Validation)
- **Task file**: `.specify/specs/001-develop-streaming-indicators/tasks.md`
- **Canonical pattern**: `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` QuoteObserver method
- **Current test**: `tests/indicators/s-z/WilliamsR/WilliamsR.StreamHub.Tests.cs`
- **Implementation**: `src/s-z/WilliamsR/WilliamsR.StreamHub.cs`

## Success Criteria

- All comprehensive rollback test cases pass
- Uses base class constants (removeAtIndex, RevisedQuotes) consistently
- Follows exact pattern from canonical EMA test
- No hardcoded indices or test data
