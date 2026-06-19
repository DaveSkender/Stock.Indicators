---
title: Batch (Series) style
description: Learn how to use Series batch style indicators for one-time bulk conversions of historical bar data
---

# Series style indicators for batch processing

Series batch style is the fastest and simplest way to calculate indicators from complete historical datasets. Use this when you have all your historical price bars available and need to convert them to indicators in one operation.

## When to use batch style

**Ideal for:**

- Complete historical datasets ready for conversion
- One-time indicator calculations
- Backtesting and historical analysis
- Performance-critical batch processing
- Simplest implementation with minimal code

**Not ideal for:**

- Real-time or streaming data (use [Stream hubs](/guide/styles/stream))
- Incremental bar-by-bar processing (use [Buffer lists](/guide/styles/buffer))
- Live data feeds requiring continuous updates

## Basic usage

All series-style indicators produce complete results for the entire bar history provided. The results are returned as a time series dataset, not just a single value.

```csharp
using Skender.Stock.Indicators;

// fetch historical price bars from your data source
IReadOnlyList<Bar> bars = GetBarsFromFeed("MSFT");

// calculate 20-period SMA
IReadOnlyList<SmaResult> results = bars.ToSma(20);

// iterate through all results
foreach (SmaResult r in results)
{
    Console.WriteLine($"SMA on {r.Timestamp:d} was ${r.Sma:N4}");
}
```

## Chaining indicators

Create sophisticated analysis by chaining indicators together. For the broader concept, see [Chaining indicators](/guide/chaining).

```csharp
// calculate RSI of On-Balance Volume
IReadOnlyList<RsiResult> results = bars
    .ToObv()
    .ToRsi(14);

// or use alternate candle price
IReadOnlyList<EmaResult> results = bars
    .Use(CandlePart.HL2)
    .ToEma(20);
```

## Performance characteristics

- **Speed:** Fastest indicator style (~baseline performance)
- **Memory:** Minimal overhead, returns `IReadOnlyList<TResult>`
- **Thread safety:** Results are immutable and thread-safe
- **Scalability:** Limited to single calculation per dataset

## Processing results

### Find specific dates

```csharp
IReadOnlyList<SmaResult> results = bars.ToSma(20);
DateTime lookupDate = DateTime.Parse("2024-01-15");
SmaResult? result = results.Find(lookupDate);
```

### Remove warmup periods

```csharp
// auto-remove recommended warmup
IReadOnlyList<AdxResult> results = bars
    .ToAdx(14)
    .RemoveWarmupPeriods();

// or specify custom amount
IReadOnlyList<AdxResult> results = bars
    .ToAdx(14)
    .RemoveWarmupPeriods(100);
```

### Condense results

Remove non-essential data points (useful for candlestick patterns):

```csharp
// only return records with signals
IReadOnlyList<CandleResult> signals = bars
    .ToMarubozu()
    .Condense();
```

## See also

- [Buffer lists](/guide/styles/buffer) for incremental processing
- [Stream hubs](/guide/styles/stream) for real-time data
- [Guide](/guide/) for detailed examples
- [Indicators](/indicators) for available indicators
