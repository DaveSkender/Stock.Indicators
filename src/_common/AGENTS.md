# Common framework code

## Purpose of this file

This file (AGENTS.md) carries the **operational guidance for AI agents and contributors** editing files in `src/_common/`: framework-level invariants, repo-specific file locations, registration conventions, and boundaries. Its companion [README.md](README.md) carries the **public directory inventory and per-file descriptions** for anyone browsing the source on GitHub. The two files are deliberately non-overlapping — keep behavioral rules here and descriptive inventory there.

This folder holds the streaming framework (`StreamHub/`, `BufferLists/`), the catalog system (`Catalog/`), core types (`Quotes/`, `Reusable/`, `QuotePart/`), and shared utilities.

Before changing anything stateful in this directory, consult [docs/plans/streaming-indicators.plan.md](../../docs/plans/streaming-indicators.plan.md) — it is the source of truth for active streaming work, release gates, and v3.1+ architecture decisions (retiring `BaseProvider<T>`, multi-input `JoinHub`, Rx / `IAsyncEnumerable` adapters).

## Skills to load

| Working on | Load |
| ---------- | ---- |
| `StreamHub/`, `Quotes/Quote.StreamHub.cs`, `Quotes/*.AggregatorHub.cs`, any `**/*.StreamHub.cs` | `#skill:indicator-stream` |
| `BufferLists/`, any `**/*.BufferList.cs` | `#skill:indicator-buffer` |
| `Catalog/`, any `**/*.Catalog.cs` | `#skill:indicator-catalog` |
| `Quotes/`, new aggregator hubs | `#skill:indicator-stream` + sections below |

Skills carry the portable patterns. The sections below carry the repository-specific specifics the skills intentionally do not duplicate.

## Streaming framework specifics

### Self-rooted source hubs

Hubs that originate a stream (no upstream provider) bootstrap their base class with an inert `BaseProvider<T>` sentinel. The sentinel:

- Lives at `src/_common/StreamHub/Providers/BaseProvider.cs`
- Carries no cache (`Results` is `Array.Empty<T>().AsReadOnly()`)
- Throws on `Subscribe(...)` and is a no-op on `Unsubscribe`/`EndTransmission`
- Masks `Properties` bit 0 (`0b11111110`) so downstream hubs become proper observers even though the sentinel itself is not

Canonical examples:

- `src/_common/Quotes/Quote.StreamHub.cs:24` — `QuoteHub` (default IQuote source)
- `src/_common/Quotes/Tick.StreamHub.cs:24` — `TickHub` (default ITick source)

`BaseProvider<T>` is acknowledged in its source comments as a workaround pending a cleaner `StreamSource<T>` root class — that refactor is queued for v3.1 in the streaming plan. Do not extend `BaseProvider<T>` beyond the existing self-rooted sources.

### Aggregator hubs

`Quote.AggregatorHub.cs` and `Tick.AggregatorHub.cs` quantize incoming bars/ticks into larger time periods. They:

- Derive from `QuoteProvider<TIn, IQuote>`
- Accept a `PeriodSize` (or raw `TimeSpan`) plus an optional `fillGaps` flag
- Emit closed bars at period boundaries; reject `PeriodSize.Month` (use `TimeSpan` overload for custom periods)
- Inherit the standard `RollbackState(int)` semantics so out-of-order ticks reconstruct correctly

When implementing a new quantizer, prefer extending the aggregator pattern over writing bespoke bucketing.

### Thread safety contract

`StreamHub<TIn, TOut>` (`src/_common/StreamHub/StreamHub.cs`) is thread-safe by holding `CacheLock` (private `object` monitor at line 16) for the duration of every cache-mutating operation. Two invariants matter when subclassing:

1. **Observer notification happens inside `CacheLock`.** `Rebuild` and `RemoveAt` call `NotifyObserversOnRebuild` / `NotifyObserversOnPrune` inside the lock specifically to prevent new items from being added between cache mutation and downstream notification. Subclasses must not release the lock before notifying.
2. **`_isRebuilding` flag (line 23) suppresses self-rebuild during `Rebuild`.** While `Rebuild` is replaying provider items through `OnAdd`/`AppendCache`, the flag forces `Act.Add` instead of recursing into another `Rebuild`. Observer cascading is still allowed and desired. Do not bypass this flag from subclass code.

`Results` returns `Cache.AsReadOnly()` — a **live read-only view**, not an immutable snapshot. The view forbids mutation (closing #1585's deviant-mutation hole) but enumeration during a concurrent `Add` will throw `InvalidOperationException`. Consumers iterating while upstream may emit must snapshot first (`.ToList()`). Subclass code accessing `Cache[i-1]` directly is safe because it executes inside `ToIndicator`, which holds the lock transitively via `OnAdd`.

A `Snapshot()` method returning an immutable copy under the lock is queued for v3.1 (see the streaming plan).

### `RollbackState(int restoreIndex)` index contract

Implemented by ~55 hubs. The base contract is:

- The base class computes `restoreIndex` via `IndexBefore` **before** calling `RollbackState`
- `restoreIndex` is the last `ProviderCache` index to **preserve**, or `-1` to reset all state
- Existing cache entries at `[restoreIndex + 1, Count)` have already been removed before this method is invoked
- The item at the rollback timestamp will be recalculated via normal `ToIndicator` processing — do not re-emit it from `RollbackState`

Audit of overrides against the formalized contract is queued for v3.1. When adding new hubs, follow the canonical pattern in `src/_common/StreamHub/StreamHub.cs:377` and the examples in `references/rollback-patterns.md` of the indicator-stream skill.

## BufferList framework specifics

`BufferList<TResult>` (`src/_common/BufferLists/BufferList.cs`) is a standalone `IReadOnlyList` for synchronous incremental compute. `MaxListSize` enables pruning when long-running. Two interfaces drive incremental adds:

- `IIncrementFromChain` — `Add(DateTime, double)`, `Add(IReusable)`, `Add(IReadOnlyList<IReusable>)` — for chainable single-value indicators
- `IIncrementFromQuote` — `Add(IQuote)`, `Add(IReadOnlyList<IQuote>)` — for indicators requiring full OHLCV

The implementation uses the C# `field` keyword at `BufferList.cs:54,56`, which is the sole reason `<EnablePreviewFeatures>true</EnablePreviewFeatures>` remains in `src/Indicators.csproj`. C# 14 / .NET 10 ships `field` as GA — removing the preview flag is queued as a quick-win cleanup in the streaming plan.

## Catalog framework specifics

`PopulateCatalog()` in `src/_common/Catalog/Catalog.Listings.cs` registers all indicator listings. Convention enforced by the existing file:

- Indicators grouped alphabetically by full name
- Each indicator block has a comment header `// {ABBR} ({Full Name})`
- Within each block: **Buffer → Series → Stream** registration order
- Blank line between indicator blocks

Backing field in this repository is `_listings` (private static `List<IndicatorListing>`). The catalog test `tests/indicators/_common/Catalog/Catalog.Metrics.Tests.cs` asserts the current counts (Series=85, Stream=79, Buffer=79); sharpening the loose `BeGreaterThan` assertions to exact counts is queued as a follow-up in the streaming plan.

## NaN handling policy

See the parent [src/AGENTS.md](../AGENTS.md#nan-handling-policy) for the canonical policy. In this folder specifically: rolling-window utilities (`CircularDoubleBuffer`, `RollingWindowMax/Min`) accept NaN values and return NaN for Min/Max when NaN is present in the window.

## Boundaries

✅ Always preserve the `CacheLock` / `_isRebuilding` invariants when subclassing `StreamHub`

✅ Always provide a `RollbackState(int)` override when adding stateful fields to a hub

✅ Always register new indicators in `Catalog.Listings.cs` in Buffer → Series → Stream order

⚠️ Ask before adding new derivations of `BaseProvider<T>` — the class is a documented workaround scheduled for replacement; current usage is limited to `QuoteHub` and `TickHub`

⚠️ Ask before adding `#pragma warning disable` directives in this folder — current footprint is exactly one intentional suppression (`IDE0010` at `StreamHub/StreamHub.cs:2`, covering the `Act` enum switch in `AppendCache` whose `default => throw` is a deliberate "would never happen" safety net for `Act.Ignore`); `BufferLists/` carries zero. New pragmas anywhere under `_common/` require explicit justification

🚫 Never expose `Cache` mutation from a subclass — go through `AppendCache`, `RemoveRange`, or `RemoveAt`

🚫 Never release `CacheLock` before notifying observers in `Rebuild` or `RemoveAt`
