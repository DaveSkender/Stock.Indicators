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

Target: StreamHub ≤ 1.5x slower than Series.

Forbid O(n²) recalculation — rebuild entire history on each tick:

```csharp
// WRONG
for (int k = 0; k <= i; k++) { subset.Add(cache[k]); }
var result = subset.ToIndicator();
```

O(1) incremental update:

```csharp
// CORRECT
_avgGain = ((_avgGain * (period - 1)) + gain) / period;
```

Use `RollingWindowMax/Min` utilities instead of O(n) linear scans.

## RollbackState pattern

Override when maintaining stateful fields:

```csharp
protected override void RollbackState(DateTime timestamp)
{
    int targetIndex = ProviderCache.IndexGte(timestamp);
    _window.Clear();
    if (targetIndex <= 0) return;
    int restoreIndex = targetIndex - 1;
    int startIdx = Math.Max(0, restoreIndex + 1 - LookbackPeriods);
    for (int p = startIdx; p <= restoreIndex; p++)
        _window.Add(ProviderCache[p].Value);
}
```

Replay up to `targetIndex - 1` (exclusive). The rollback timestamp is recalculated via normal processing.

## Testing requirements

- Inherit `StreamHubTestBase`
- Abstract method (compile error if missing): `ToStringOverride_ReturnsExpectedName()`
- Implement ONE observer interface:
  - `ITestChainObserver` (most indicators — chain input): inherits `ITestQuoteObserver`, adds `ChainObserver_ChainedProvider_MatchesSeriesExactly()`
  - `ITestQuoteObserver` (direct quote input only): `QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()`, `WithCachePruning_MatchesSeriesExactly()`
- If hub acts as chain provider, also implement `ITestChainProvider`: `ChainProvider_MatchesSeriesExactly()`

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
- [ ] **Catalog registration**: Registered in `Catalog.Listings.cs`
- [ ] **Performance benchmark**: Add to `tools/performance/Perf.Stream.cs`
- [ ] **Public documentation**: Update `docs/indicators/{IndicatorName}.md`
- [ ] **Regression tests**: Add to `tests/indicators/**/{IndicatorName}.Regression.Tests.cs`
- [ ] **Migration guide**: Update `docs/migration.md` for notable and breaking changes from v2

## Examples

- Chain: `src/e-k/Ema/Ema.StreamHub.cs`
- Complex state: `src/a-d/Adx/Adx.StreamHub.cs`
- Rolling window: `src/a-d/Chandelier/Chandelier.StreamHub.cs`
- Compound hub: `src/s-z/StochRsi/StochRsi.StreamHub.cs`

- [Provider selection](references/provider-selection.md)
- [Rollback patterns](references/rollback-patterns.md)
- [Performance patterns](references/performance-patterns.md)
- [Compound hubs](references/compound-hubs.md)

## Constraints

- O(n²) recalculation is forbidden; all updates must be O(1)
- `RollbackState()` replay is exclusive of rollback timestamp
- Series parity required: results must be numerically identical to StaticSeries
