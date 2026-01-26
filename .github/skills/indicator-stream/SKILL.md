---
name: indicator-stream
description: Implement StreamHub real-time indicators with O(1) performance. Use for ChainHub or QuoteProvider implementations. Covers provider selection, RollbackState patterns, performance anti-patterns, and comprehensive testing with StreamHubTestBase.
---

# StreamHub indicator development

## Provider selection

| Provider Base | Input | Output | Use Case |
| ------------- | ----- | ------ | -------- |
| `ChainHub<IReusable, TResult>` | Single value | IReusable | Chainable indicators |
| `ChainHub<IQuote, TResult>` | OHLCV | IReusable | Quote-driven, chainable output |
| `QuoteProvider<IQuote, TResult>` | OHLCV | IQuote | Quote-to-quote transformation |
| `StreamHub<TProviderResult, TResult>` | Any hub result | Any result | Compound hubs (internal hub dependency) |

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
    int targetIndex = ProviderCache.IndexGte(timestamp);

    _window.Clear();

    if (targetIndex <= 0) return;

    int restoreIndex = targetIndex - 1;  // Rebuild up to but NOT including timestamp
    int startIdx = Math.Max(0, restoreIndex + 1 - LookbackPeriods);

    for (int p = startIdx; p <= restoreIndex; p++)
        _window.Add(ProviderCache[p].Value);
}
```

**Critical**: Replay up to `targetIndex - 1` (exclusive of rollback timestamp). The quote at the rollback timestamp will be recalculated when it arrives via normal processing.

## Testing requirements

1. Inherit from `StreamHubTestBase`
2. Implement exactly ONE observer interface:
   - `ITestChainObserver` (most common)
   - `ITestQuoteObserver` (quote-only providers)
3. Implement at most ONE provider interface: `ITestChainProvider`
4. Comprehensive rollback validation (required):
   - Prefill warmup window before subscribing
   - Stream in-order including duplicates
   - Insert a late historical quote → verify recalculation
   - Remove a historical quote → verify recalculation
   - Compare results to Series with strict ordering

## Required implementation

- [ ] Source code: `src/**/{IndicatorName}.StreamHub.cs` file exists
  - [ ] Uses appropriate provider base (ChainHub or QuoteProvider)
  - [ ] Validates parameters in constructor; calls Reinitialize() as needed
  - [ ] Implements O(1) state updates; avoids O(n²) recalculation
  - [ ] Overrides RollbackState() when maintaining stateful fields
  - [ ] Overrides ToString() with concise hub name
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.StreamHub.Tests.cs` exists
  - [ ] Inherits StreamHubTestBase with correct test interfaces
  - [ ] Comprehensive rollback validation present
  - [ ] Verifies Series parity
- [ ] **Catalog registration**: Registered in [Catalog.Listings.cs](../../../src/_common/Catalog/Catalog.Listings.cs)
- [ ] **Performance benchmark**: Add to #file:../../../tools/performance/Perf.Stream.cs
- [ ] **Public documentation**: Update `docs/indicators/{IndicatorName}.md`
- [ ] **Regression tests**: Add to `tests/indicators/**/{IndicatorName}.Regression.Tests.cs`
- [ ] **Migration guide**: Update [docs/migration.md](../../../docs/migration.md) for notable and breaking changes from v2

## Examples

- Chain: `src/e-k/Ema/Ema.StreamHub.cs`
- Complex state: `src/a-d/Adx/Adx.StreamHub.cs`
- Rolling window: `src/a-d/Chandelier/Chandelier.StreamHub.cs`
- Compound hub: `src/s-z/StochRsi/StochRsi.StreamHub.cs`, `src/e-k/Gator/Gator.StreamHub.cs`

See #folder:references for detailed patterns:

- #file:references/provider-selection.md - Choosing the right provider base
- #file:references/rollback-patterns.md - RollbackState implementation examples
- #file:references/performance-patterns.md - O(1) optimization techniques
- #file:references/compound-hubs.md - Internal hub dependencies and construction patterns

## Common pitfalls

- Null or empty quotes causing stateful streaming regressions (always validate input sequences)
- Index out of range and buffer reuse issues in streaming indicators (guard shared spans and caches)
- Performance regressions from O(n) or O(n²) patterns instead of O(1) incremental updates
- Improper rollback state replay (must replay up to targetIndex - 1, exclusive of rollback timestamp)

---
Last updated: January 25, 2026
