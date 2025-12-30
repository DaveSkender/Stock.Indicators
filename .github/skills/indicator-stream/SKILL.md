---
name: indicator-stream
description: Implement StreamHub real-time indicators with O(1) performance. Use for ChainProvider, QuoteProvider, or PairsProvider implementations. Covers provider selection, RollbackState patterns, performance anti-patterns, and comprehensive testing with StreamHubTestBase.
---

# StreamHub indicator development

StreamHub indicators support real-time data processing with stateful operations, achieving ≤1.5x Series performance.

## Provider selection

| Provider Base | Input | Output | Use Case |
| ------------- | ----- | ------ | -------- |
| `ChainProvider<IReusable, TResult>` | Single value | IReusable | Chainable indicators |
| `ChainProvider<IQuote, TResult>` | OHLCV | IReusable | Quote-driven, chainable output |
| `QuoteProvider<IQuote, TResult>` | OHLCV | IQuote | Quote-to-quote transformation |
| `PairsProvider<IReusable, TResult>` | Dual stream | IReusable | Synchronized dual inputs |

## Performance requirements

**Target**: StreamHub ≤ 1.5x slower than Series

### Anti-pattern 1: O(n²) recalculation (FORBIDDEN)

```csharp
// WRONG - Rebuilds entire history on each tick
for (int k = 0; k <= i; k++) { subset.Add(cache[k]); }
var result = subset.ToIndicator();
```

### Correct: O(1) incremental update

```csharp
// CORRECT - Maintain state, update incrementally
_avgGain = ((_avgGain * (period - 1)) + gain) / period;
```

### Anti-pattern 2: O(n) window scans

Use `RollingWindowMax/Min` utilities instead of linear scans for max/min operations.

## RollbackState pattern

Override when maintaining stateful fields:

```csharp
protected override void RollbackState(DateTime timestamp)
{
    _window.Clear();
    int targetIndex = ProviderCache.IndexGte(timestamp) - 1;
    int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

    for (int p = startIdx; p <= targetIndex; p++)
        _window.Add(ProviderCache[p].Value);
}
```

## Testing requirements

1. Inherit from `StreamHubTestBase`
2. Implement exactly ONE observer interface:
   - `ITestChainObserver` (most common)
   - `ITestQuoteObserver` (quote-only providers)
   - `ITestPairsObserver` (dual-stream)
3. Implement at most ONE provider interface: `ITestChainProvider`
4. Comprehensive rollback validation (required):
   - Prefill warmup window before subscribing
   - Stream in-order including duplicates
   - Insert a late historical quote → verify recalculation
   - Remove a historical quote → verify recalculation
   - Compare results to Series with strict ordering

## Code completion checklist

- [ ] Source code: `src/**/{IndicatorName}.StreamHub.cs` file exists
  - [ ] Uses appropriate provider base (`ChainProvider`, `QuoteProvider`, or `PairsProvider`)
  - [ ] Validates parameters in constructor; calls `Reinitialize()` as needed
  - [ ] Implements O(1) state updates; avoids O(n²) recalculation
  - [ ] Overrides `RollbackState()` when maintaining stateful fields
  - [ ] Overrides `ToString()` with concise hub name
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.StreamHub.Tests.cs` exists
  - [ ] Inherits `StreamHubTestBase` with correct test interfaces
  - [ ] Comprehensive rollback validation present
  - [ ] Verifies Series parity

## Reference implementations

- Chain: `src/e-k/Ema/Ema.StreamHub.cs`
- Complex state: `src/a-d/Adx/Adx.StreamHub.cs`
- Pairs: `src/a-d/Correlation/Correlation.StreamHub.cs`
- Rolling window: `src/a-d/Chandelier/Chandelier.StreamHub.cs`

See `references/` for detailed patterns:

- `provider-selection.md` - Choosing the right provider base
- `rollback-patterns.md` - RollbackState implementation examples
- `performance-patterns.md` - O(1) optimization techniques

---
Last updated: December 30, 2025
