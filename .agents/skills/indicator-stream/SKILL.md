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
| `QuoteProvider<TIn, TOut>` + `BaseProvider<T>` | None (self-rooted) | IQuote / ITick | Source hubs with no upstream (e.g., `QuoteHub`, `TickHub`, aggregators) |
| `StreamHub<TProviderResult, TResult>` | Any hub result | Any result | Compound hubs (internal hub dependency) |

### Self-rooted source hubs

Hubs that originate a stream (no upstream provider) bootstrap their base class with an inert `BaseProvider<T>` sentinel. `BaseProvider<T>` lives at `src/_common/StreamHub/Providers/BaseProvider.cs` and exists only to satisfy the base-class constructor signature; it carries no cache, rejects subscriptions, and masks `Properties` bit 0 so downstream hubs become proper observers. See `src/_common/Quotes/Quote.StreamHub.cs:24` and `src/_common/Quotes/Tick.StreamHub.cs:24` for the canonical pattern.

### Aggregator hubs (PR #1875)

`QuoteAggregatorHub` (`src/_common/Quotes/Quote.AggregatorHub.cs`) and `Tick.AggregatorHub` quantize incoming quotes/ticks into larger time periods. They derive from `QuoteProvider<IQuote, IQuote>` and `QuoteProvider<ITick, IQuote>` respectively, expose a `PeriodSize` or `TimeSpan` parameter, support optional gap-fill, and emit new bars at period boundaries. Use these instead of writing a bespoke bucketing layer.

## Performance targets

Bands are defined in [tools/performance/PERFORMANCE_ANALYSIS.md](../../../tools/performance/PERFORMANCE_ANALYSIS.md); that document is the source of truth for current measurements and category bands.

| Band | StreamHub overhead | Status |
| ---- | ------------------ | ------ |
| Target | ≤ 1.5x | ✅ meets target |
| Acceptable | 1.5x – 3x | ✅ acceptable |
| Review | 3x – framework floor | ⚠️ investigate |
| Critical | indicator-specific algorithmic issue (e.g. O(n²)) | 🔴 fix |

The "framework floor" is the per-tick overhead inherent to the observer pattern, cache management, and read-only collection wrappers. Simple stateless indicators (ROC, EMA) measure 6-11x against Series; this is acceptable because absolute throughput remains ~40,000 quotes/sec on the reference hardware. Optimization effort should target indicator-specific algorithmic issues, not the framework floor.

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

## Thread safety contract

StreamHub is thread-safe across `Add`, `Rebuild`, `RemoveRange`, and `RemoveAt` by holding `CacheLock` (a private `object` monitor declared at `src/_common/StreamHub/StreamHub.cs:16`) for the duration of every cache-mutating operation. Two invariants matter for indicator implementations:

1. **Observer notification happens inside `CacheLock`.** This prevents new items from being added between cache mutation and downstream notification, which would otherwise desynchronize subscribers. Subclasses must not release the lock before calling `NotifyObserversOn...`.
2. **`_isRebuilding` flag (line 23) suppresses self-rebuild during `Rebuild`.** While `Rebuild` is replaying provider items through `OnAdd`/`AppendCache`, the flag forces `Act.Add` instead of recursing into another `Rebuild`. Observer cascading is still allowed and desired. Do not bypass this flag from subclass code.

The public `Results` surface returns `Cache.AsReadOnly()`, which is a **live read-only view**, not an immutable snapshot. Consumers that iterate while upstream may emit must snapshot first (`.ToList()`). Subclass code accessing `Cache[i-1]` directly is safe because it executes inside `ToIndicator`, which holds the lock transitively via `OnAdd`.

## RollbackState pattern

Override when maintaining stateful fields.
The base class computes `restoreIndex` via `IndexBefore` before calling this method.
`restoreIndex` is the last `ProviderCache` index to preserve, or `-1` to reset everything.

```csharp
protected override void RollbackState(int restoreIndex)
{
    _window.Clear();
    if (restoreIndex < 0) return;
    int startIdx = Math.Max(0, restoreIndex + 1 - LookbackPeriods);
    for (int p = startIdx; p <= restoreIndex; p++)
        _window.Add(ProviderCache[p].Value);
}
```

Replay up to `restoreIndex` (inclusive). The item at the rollback timestamp is recalculated via normal processing.

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
- Self-rooted source: `src/_common/Quotes/Quote.StreamHub.cs`
- Aggregator: `src/_common/Quotes/Quote.AggregatorHub.cs`

- [Provider selection](references/provider-selection.md)
- [Rollback patterns](references/rollback-patterns.md)
- [Performance patterns](references/performance-patterns.md)
- [Compound hubs](references/compound-hubs.md)

## Plan reference

Active and historical streaming work is tracked in [docs/plans/streaming-indicators.plan.md](../../../docs/plans/streaming-indicators.plan.md) — consult this for current performance targets, open release gates, architectural decisions, and v3.1+ roadmap (e.g., retiring `BaseProvider<T>` via `StreamSource<T>`, multi-input `JoinHub`, Rx/`IAsyncEnumerable` adapters).

## Constraints

- O(n²) recalculation is forbidden; all updates must be O(1)
- `RollbackState(int restoreIndex)` receives the last index to preserve (`-1` = reset all); replay is inclusive of `restoreIndex`, exclusive of the rollback timestamp
- Series parity required: results must be numerically identical to StaticSeries
