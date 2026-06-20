---
title: Migration from v2 to v3
description: Complete guide for migrating from Stock Indicators v2 to v3, including breaking changes, API updates, and new streaming capabilities.
---

# Migration from v2 to v3

This guide provides a comprehensive migration path from v2 to v3 of the Stock Indicators library. It includes all technical changes to the public API, syntax changes, and specific examples of deprecated and breaking changes.

::: warning
Most of the obsolete v2 syntax has been shimmed in library version `3.0` with `[Obsolete]` code analysis flags to aid migrations. These shims are (or will be) removed in version `3.1`, so start with any `3.0.x` version before upgrading further.
:::

## Summary of breaking changes

### API method naming

- **All static time-series API methods**: Renamed from `GetX()` to `ToX()`
  - Example: `quotes.GetSma(20)` → `quotes.ToSma(20)`

### Market-data type renames (Quote → Bar)

To align with industry-standard terminology (an OHLCV aggregate is universally a *bar*; a *quote* is a bid/ask snapshot), the core market-data types were renamed in v3. The old names remain as deprecated aliases during a migration window, so existing code keeps working — update to the new names as the deprecation warnings guide you:

- **`Quote` → `Bar`** (and the built-in `Quote` record → `Bar` record)
- **`IQuote` → `IBar`** — custom market-data types now implement `IBar`
- **`QuoteHub` → `BarHub`**, **`QuoteProvider`/`IQuoteProvider` → `BarProvider`/`IBarProvider`**
- **`QuoteAggregatorHub` → `BarAggregatorHub`**, **`QuotePart`/`IQuotePart` → `BarPart`/`IBarPart`**
- **`PeriodSize` → `BarInterval`** (aggregation interval enum; member names unchanged)
- **`Tick`/`ITick` → `TradeTick`/`ITradeTick`** (single trade print; `TickHub` → `TradeTickHub`)
- Extension methods renamed accordingly: `ToQuoteHub()` → `ToBarHub()`, `ToQuotePart()` → `ToBarPart()`, etc.

The old `Quote`, `IQuote`, `PeriodSize`, `IReusableResult`, and `BasicData` names remain as **warning-level `[Obsolete]` aliases** of the new types, so existing code keeps compiling and running while deprecation warnings guide each rename. `Quote`/`IQuote` flow through the new generic API directly, and `PeriodSize` keeps working via obsolete `Aggregate(PeriodSize)`/`GetPivotPoints(PeriodSize)` forwarding overloads. Migrate at your own pace — and finish related member renames such as `Date` → `Timestamp` to clear all warnings. These shims will be removed in a future major version.

> **New:** `BarInterval` now has a bidirectional string-code map — `interval.ToCode()` (e.g. `BarInterval.FiveMinutes` → `"5m"`) and `"5m".ToBarInterval()` (case-insensitive, with aliases like `"5min"`/`"1day"`).

### Quote/Bar interface details

- **`Bar` type** (formerly `Quote`): Immutable `record` type
- **`IBar.Date` property**: Renamed to `IBar.Timestamp`. `Date` remains as an `[Obsolete]` alias in v3.x for backward compatibility and will be removed in v3.1 — update consumers to `Timestamp` now.
- **`IBar` interface** (formerly `IQuote`): Now a reusable (chainable) type
- **Custom bar types**: Must implement the `IReusable` interface
- **`IReusableResult`**: Renamed to `IReusable`
- **`IReusable.Value` property**: Changed to non-nullable, returns `double.NaN` instead of `null`

### Indicator return types

- **All indicator results**: Changed from `sealed class` to immutable `record` types
- **`BasicData` type**: Renamed to `TimeValue`
- **`AtrStopResult` values**: Changed from `decimal` to `double`
- **`UlcerIndexResult.UI` property**: Renamed to `UlcerIndex`
- **`SmaAnalysis` model**: Renamed to `SmaAnalysisResult`

### Removed features

- **`GetBaseQuote()` indicator**: Removed — use the `Use(CandlePart)` utility instead
- **`SyncSeries()` utility**: Removed along with `SyncType` enum
- **`Find()` and `FindIndex()` utilities**: Deprecated
- **`ToTupleCollection()` utility**: Deprecated
- **`ToCollection()` utility**: Deprecated

### Other changes

- **`Use()` method**: `candlePart` parameter now required (no default)
- **`Use()` return type**: Now returns chainable `TimeValue` instead of tuple
- **`Numerix` class**: Renamed to `Numerical`
- **Internal signals**: Deprecated for several indicators
- **GetX tuple interfaces**: Deprecated

### Mathematical corrections

- **ADXR calculation**: Fixed off-by-one error in lookback period
  - Now correctly averages current ADX with ADX from exactly `lookbackPeriods` ago
  - Previously used `lookbackPeriods - 1`
  - First ADXR value appears one period later
  - ADXR values will differ slightly from v2

## Migration steps

### Step 1: Update method names

Replace all `GetX()` method calls with `ToX()`:

```csharp
var smaResults = quotes.GetSma(20); // [!code --]
var smaResults = quotes.ToSma(20);  // [!code ++]
```

### Step 2: Update `IBar` property names

Rename `Date` to `Timestamp` in all custom bar classes (and implement `IBar` in place of the obsolete `IQuote`):

```csharp
public class MyBar : IBar
{
    public DateTime Date { get; set; }      // [!code --]
    public DateTime Timestamp { get; set; } // [!code ++]
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
```

### Step 3: Update custom bar types

Change custom bar types to `record` or implement value based equality:

```csharp
// v2
public class MyQuote : IQuote
{
    // properties...

    [JsonIgnore]
    public double Value => (double)Close;
}

// v3 - option 1: use record
public record MyBar : IBar
{
    // properties...
}

// v3 - option 2: implement value-based equality
public class MyBar : IBar, IEquatable<MyBar>
{
    // properties...
    
    // implement value-based equality
    public override bool Equals(object obj) { /* ... */ }
    public override int GetHashCode() { /* ... */ }
}
```

### Step 4: Update `Use()` method calls

Add explicit `candlePart` parameter:

```csharp
// v2 - candlePart defaulted to Close
var quoteParts = quotes.Use();

// v3 - candlePart required
var quoteParts = quotes.Use(CandlePart.Close);
```

Handle new `TimeValue` return type:

```csharp
// v2
var (timestamp, value) = quotes.Use(CandlePart.Close);

// v3
IReadOnlyList<TimeValue> quoteParts = quotes.Use(CandlePart.Close);
```

### Step 5: Update null handling

Replace `null` checks with `double.NaN` checks:

```csharp
// v2
if (result.Value != null)
{
    decimal value = result.Value.Value;
}

// v3
if (!double.IsNaN(result.Value))
{
    double value = result.Value;
}
```

### Step 6: Update class references

- `Numerix` → `Numerical`
- `BasicData` → `TimeValue`
- `SmaAnalysis` → `SmaAnalysisResult`
- `UlcerIndexResult.UI` → `UlcerIndexResult.UlcerIndex`

### Step 7: Remove deprecated utilities

Replace or remove calls to:

- `SyncSeries()` - manually align data instead
- `Find()` - use LINQ `.FirstOrDefault()`
- `FindIndex()` - use LINQ `.Select((item, index) => ...)` with `.FirstOrDefault()`
- `GetBaseQuote()` - use `Use(CandlePart)` utility instead

### Step 8: Update ADXR expectations

If using ADXR, expect slight changes in values and timing:

```csharp
// v2 - first ADXR at index 40 (for lookbackPeriods=14)
var adxResults = quotes.GetAdx(14);
var firstAdxr = adxResults[40].Adxr; // not null

// v3 - first ADXR at index 41
var adxResults = quotes.ToAdx(14);
var firstAdxr = adxResults[41].Adxr; // not NaN
// adxResults[40].Adxr is now NaN
```

## New v3 feature: streaming capabilities

v3 introduces comprehensive streaming capabilities for real-time and incremental data processing. Most indicators now support three calculation styles.

### Calculation styles

#### Series style (traditional batch processing)

The Series style processes complete historical datasets and returns all results at once. This is the same pattern from v2 and remains the default approach for backtesting and historical analysis.

```csharp
// v2 and v3 Series style
IReadOnlyList<SmaResult> results = quotes.ToSma(20);
```

**Use when:**

- Processing complete historical datasets
- Backtesting strategies
- One-time calculations
- No real-time requirements

#### BufferList style (incremental processing)

The BufferList style allows incremental calculations with efficient buffer management. Use this when you need to add data points incrementally without maintaining a full hub infrastructure.

```csharp
// v3 BufferList style
SmaList smaList = new(20);

foreach (IBar bar in bars)  // simulating stream
{
    smaList.Add(bar);
}

IReadOnlyList<SmaResult> results = smaList;
```

**Use when:**

- Growing datasets with frequent appends
- Accumulating historical data
- Incremental calculations without real-time subscriptions
- Memory-efficient processing

#### StreamHub style (real-time observable patterns)

The StreamHub style provides real-time processing with observable patterns and state management. Multiple indicators can subscribe to a single `BarHub` for coordinated real-time analysis.

```csharp
// v3 StreamHub style
BarHub barHub = new();

EmaHub emaFast = barHub.ToEmaHub(50);
EmaHub emaSlow = barHub.ToEmaHub(200);

// add bars to barHub (from stream)
barHub.Add(newBar);
// and the 2 EmaHub will be in sync

if(emaFast.Results[^2].Ema < emaSlow.Results[^2].Ema  // or .Value
&& emaFast.Results[^1].Ema > emaSlow.Results[^1].Ema)
{
    // cross over occurred
}
```

> **Mutate the root hub only.** Feed and correct data through the `BarHub`
> (or `TradeTickHub`) you created — it cascades to every dependent hub. Calling
> `Add`, `RemoveAt`, `RemoveRange`, `Remove`, or `Reinitialize` on a subscribed
> hub such as an `EmaHub` throws `InvalidOperationException`. See the
> [streaming guide](/guide/styles/stream#thread-safety) for details.

**Use when:**

- Live data feeds and WebSocket integration
- Multiple indicators need coordinated updates
- Trading applications requiring low latency
- Real-time dashboards and monitoring

### Migration examples

#### From v2 Series to v3 BufferList

If you were building up results incrementally in v2, you can now use BufferList for better performance:

```csharp
// v2 approach (inefficient for incremental updates)
List<Quote> quotes = new();
foreach (Quote newQuote in stream)
{
    quotes.Add(newQuote);
    var results = quotes.ToSma(20);  // Recalculates everything!
    // Use results...
}

// v3 BufferList (efficient incremental updates)
SmaList smaList = new(20);
foreach (Bar newBar in stream)
{
    smaList.Add(newBar);
    
    // Note: smaList[^1] throws ArgumentOutOfRangeException if empty
    if (smaList.Count > 0)
    {
        SmaResult latest = smaList[^1];
        // Use latest...
    }
}
```

#### From v2 Series to v3 StreamHub

If you need to coordinate multiple indicators with live data:

```csharp
// v2 approach (requires maintaining separate lists)
List<Quote> quotes = new();
foreach (Quote newQuote in stream)
{
    quotes.Add(newQuote);
    var smaResults = quotes.ToSma(20);
    var rsiResults = quotes.ToRsi(14);
    var macdResults = quotes.ToMacd();
    // Process results...
}

// v3 StreamHub (coordinated real-time updates)
BarHub barHub = new();
SmaHub smaHub = barHub.ToSmaHub(20);
RsiHub rsiHub = barHub.ToRsiHub(14);
MacdHub macdHub = barHub.ToMacdHub();

foreach (Bar newBar in stream)
{
    barHub.Add(newBar);  // Single update propagates to all observers
    // Access latest results from each hub
}
```

### Performance considerations

- **Series**: Best throughput for complete datasets, optimized for batch processing
- **BufferList**: Balanced performance for incremental updates, ~10-20% overhead vs Series
- **StreamHub**: Low latency per quote for real-time scenarios, ~20-30% overhead vs Series

### Streaming documentation

For indicator-specific streaming examples, see the documentation for each indicator. Indicators with streaming support include a "Streaming" section with BufferList and StreamHub examples.

Popular indicators with complete streaming documentation:

- Moving Averages: [SMA](/indicators/sma), [EMA](/indicators/ema), [WMA](/indicators/wma)
- Oscillators: [RSI](/indicators/rsi), [MACD](/indicators/macd), [Stochastic](/indicators/stoch)
- Channels: [Bollinger Bands](/indicators/bollinger-bands), [Keltner](/indicators/keltner)

## Known issues and tips

A few v3 known sub-optimal behaviors worth noting before reporting an issue. These are in our backlog.

### Streaming cache caps results at 100,000 by default

Both the BufferList (`MaxListSize`) and StreamHub (`maxCacheSize`) styles keep a rolling cache that defaults to **100,000 elements** and automatically prunes the oldest results once that limit is exceeded. If you replay a large historical dataset through a hub or buffer list — for example, several years of minute bars — and expect every result back, the early results will have been pruned away.

::: tip ✨ Raise the cache size for large datasets
Set the limit explicitly when the full history must stay resident:

```csharp
// StreamHub: size the root hub (it cascades to chained hubs)
BarHub barHub = new(maxCacheSize: 250_000);

// BufferList: size the list
SmaList smaList = new(20) { MaxListSize = 250_000 };
```

`maxCacheSize` is inherited by every chained hub, so size it for the **largest warmup in the whole chain**. Don't shrink it down to a single indicator's lookback — each hub validates the size against its own warmup floor at construction and throws `ArgumentOutOfRangeException` if it is too small. See [Stream hub memory management](/guide/styles/stream#memory-management) for details.
:::

### Corrections after pruning are approximate

Once the cache has pruned old bars, a late arrival or correction that triggers a rebuild can no longer see the pruned history, so the rebuilt values for stateful indicators won't exactly match a hub that received the same data in order. Keep the cache large enough to cover your expected late-arrival window.

### Mutate the root hub only

Feed and correct streaming data through the `BarHub` (or `TradeTickHub`) you created — it cascades to every dependent hub. Calling `Add`, `RemoveAt`, `RemoveRange`, `Remove`, or `Reinitialize` on a subscribed hub such as an `EmaHub` throws `InvalidOperationException`. See the [streaming guide](/guide/styles/stream#thread-safety).

### `null` versus `NaN` results

v3 returns `NaN` (not `null`) for incalculable values on `double` result properties. Update comparisons from `result.Value == null` to `double.IsNaN(result.Value)`. See [Step 5](#step-5-update-null-handling).

### ADXR warmup shifted

`Adxr` now begins one period later than in v2 to correct an off-by-one in the warmup. Expect the first non-`NaN` `Adxr` value at a slightly later index. See [Step 8](#step-8-update-adxr-expectations).

## Quick reference table

| v2 API | v3 API | Notes |
| ------ | ------ | ----- |
| `quotes.GetSma(20)` | `quotes.ToSma(20)` | Method prefix changed |
| `Quote` | `Bar` | Type renamed (OHLCV bar) |
| `IQuote` | `IBar` | Interface renamed |
| `QuoteHub` | `BarHub` | Streaming hub renamed |
| `PeriodSize` | `BarInterval` | Enum renamed |
| `Tick` / `ITick` | `TradeTick` / `ITradeTick` | Type renamed (trade print) |
| `IBar.Date` | `IBar.Timestamp` | Property renamed |
| `quotes.Use()` | `quotes.Use(CandlePart.Close)` | Parameter now required |
| `result.Value == null` | `double.IsNaN(result.Value)` | Null handling changed |
| `Numerix` | `Numerical` | Class renamed |
| `BasicData` | `TimeValue` | Type renamed |
| `SmaAnalysis` | `SmaAnalysisResult` | Type renamed |
| `UlcerIndexResult.UI` | `UlcerIndexResult.UlcerIndex` | Property renamed |
| `SyncSeries()` | (removed) | Use manual alignment |
| `Find()` / `FindIndex()` | LINQ methods | Use `.FirstOrDefault()` etc. |
| `GetBaseQuote()` | `Use(CandlePart)` | Use utility instead |

## Need help?

- [Guide and Pro tips](/guide/getting-started) - Getting started with v3
- [Indicators](/indicators) - Indicator-specific documentation
- [GitHub Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions) - Ask questions and share ideas
- [GitHub Issues](https://github.com/DaveSkender/Stock.Indicators/issues) - Report bugs or request features
