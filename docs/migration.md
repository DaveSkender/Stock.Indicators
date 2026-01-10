---
title: Migration from v2 to v3
description: Complete guide for migrating from Stock Indicators v2 to v3, including breaking changes, API updates, and new streaming capabilities.
---

# Migration from v2 to v3

This guide provides a comprehensive migration path from v2 to v3 of the Stock Indicators library. It includes all technical changes to the public API, syntax changes, and specific examples of deprecated and breaking changes.

## Summary of breaking changes

### API method naming

- **All static time-series API methods**: Renamed from `GetX()` to `ToX()`
  - Example: `quotes.GetSma(20)` → `quotes.ToSma(20)`

### Quote types and interfaces

- **`Quote` type**: Changed to immutable `record` type
- **`IQuote.Date` property**: Renamed to `IQuote.Timestamp`
- **`IQuote` interface**: Now a reusable (chainable) type
- **Custom quote types**: Must implement the `IReusable` interface
- **`IReusableResult`**: Renamed to `IReusable`
- **`IReusable.Value` property**: Changed to non-nullable, returns `double.NaN` instead of `null`

### Indicator return types

- **All indicator results**: Changed from `sealed class` to immutable `record` types
- **`UseResult` type**: Renamed to `QuotePart`
- **`AtrStopResult` values**: Changed from `decimal` to `double`
- **`UlcerIndexResult.UI` property**: Renamed to `UlcerIndex`
- **`SmaAnalysis` model**: Renamed to `SmaAnalysisResult`

### Removed features

- **`GetBaseQuote()` indicator**: Removed along with `BasicData` return types
- **`SyncSeries()` utility**: Removed along with `SyncType` enum
- **`Find()` and `FindIndex()` utilities**: Removed
- **`ToTupleCollection()` utility**: Deprecated

### Other changes

- **`Use()` method**: `candlePart` parameter now required (no default)
- **`Use()` return type**: Now returns chainable `QuotePart` instead of tuple
- **`Numerixs` class**: Renamed to `Numerical`
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

### Step 2: Update `IQuote` property names

Rename `Date` to `Timestamp` in all custom quote classes:

```csharp
public class MyQuote : IQuote
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

### Step 3: Update custom quote types

Change custom quote types to `record` or implement value based equality:

```csharp
// v2
public class MyQuote : IQuote
{
    // properties...

    [JsonIgnore]
    public double Value => (double)Close;
}

// v3 - option 1: use record
public record MyQuote : IQuote
{
    // properties...
}

// v3 - option 2: implement value-based equality
public class MyQuote : IQuote, IEquatable<MyQuote>
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

Handle new `QuotePart` return type:

```csharp
// v2
var (timestamp, value) = quotes.Use(CandlePart.Close);

// v3
IReadOnlyList<QuotePart> quoteParts = quotes.Use(CandlePart.Close);
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

- `Numerixs` → `Numerical`
- `UseResult` → `QuotePart`
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

foreach (IQuote quote in quotes)  // simulating stream
{
    smaList.Add(quote);
}

IReadOnlyList<SmaResult> results = smaList;
```

**Use when:**

- Growing datasets with frequent appends
- Accumulating historical data
- Incremental calculations without real-time subscriptions
- Memory-efficient processing

#### StreamHub style (real-time observable patterns)

The StreamHub style provides real-time processing with observable patterns and state management. Multiple indicators can subscribe to a single `QuoteHub` for coordinated real-time analysis.

```csharp
// v3 StreamHub style
QuoteHub quoteHub = new();

EmaHub emaFast = quoteHub.ToEmaHub(50);
EmaHub emaSlow = quoteHub.ToEmaHub(200);

// add quotes to quoteHub (from stream)
quoteHub.Add(newQuote);
// and the 2 EmaHub will be in sync

if(emaFast.Results[^2].Ema < emaSlow.Results[^2].Ema  // or .Value
&& emaFast.Results[^1].Ema > emaSlow.Results[^1].Ema)
{
    // cross over occurred
}
```

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
foreach (Quote newQuote in stream)
{
    smaList.Add(newQuote);
    SmaResult latest = smaList.LastOrDefault();
    // Use latest...
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
QuoteHub quoteHub = new();
var smaHub = quoteHub.ToSma(20);
var rsiHub = quoteHub.ToRsi(14);
var macdHub = quoteHub.ToMacd();

foreach (Quote newQuote in stream)
{
    quoteHub.Add(newQuote);  // Single update propagates to all observers
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

- Moving Averages: [SMA](/indicators/Sma), [EMA](/indicators/Ema), [WMA](/indicators/Wma)
- Oscillators: [RSI](/indicators/Rsi), [MACD](/indicators/Macd), [Stochastic](/indicators/Stoch)
- Channels: [Bollinger Bands](/indicators/BollingerBands), [Keltner](/indicators/Keltner)

## Quick reference table

| v2 API | v3 API | Notes |
| ------ | ------ | ----- |
| `quotes.GetSma(20)` | `quotes.ToSma(20)` | Method prefix changed |
| `IQuote.Date` | `IQuote.Timestamp` | Property renamed |
| `quotes.Use()` | `quotes.Use(CandlePart.Close)` | Parameter now required |
| `result.Value == null` | `double.IsNaN(result.Value)` | Null handling changed |
| `Numerixs` | `Numerical` | Class renamed |
| `UseResult` | `QuotePart` | Type renamed |
| `SmaAnalysis` | `SmaAnalysisResult` | Type renamed |
| `UlcerIndexResult.UI` | `UlcerIndexResult.UlcerIndex` | Property renamed |
| `SyncSeries()` | (removed) | Use manual alignment |
| `Find()` / `FindIndex()` | LINQ methods | Use `.FirstOrDefault()` etc. |
| `GetBaseQuote()` | `Use(CandlePart)` | Use utility instead |

## Need help?

- [Guide and Pro tips](/guide) - Getting started with v3
- [Indicators](/indicators) - Indicator-specific documentation
- [GitHub Discussions](https://github.com/DaveSkender/Stock.Indicators/discussions) - Ask questions and share ideas
- [GitHub Issues](https://github.com/DaveSkender/Stock.Indicators/issues) - Report bugs or request features

---
Last updated: January 10, 2026
