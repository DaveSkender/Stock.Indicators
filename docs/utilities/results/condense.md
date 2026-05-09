---
title: Condense results
description: Remove non-essential results to return only meaningful data records.
---

# Condense results

`results.Condense()` removes non-essential results so it only returns meaningful data records. For example, when used on [Candlestick Patterns](/indicators/Doji), it only returns records where a signal is generated.

## Syntax

```csharp
IReadOnlyList<TResult> condensedResults = results.Condense();
```

## Returns

**IReadOnlyList\<TResult\>** - A filtered list containing only meaningful results.

## Usage

```csharp
// example: only show Marubozu signals
IReadOnlyList<CandleResult> results =
  quotes.ToMarubozu(..).Condense();

// condensed results only include dates with signals
foreach (var result in results)
{
  Console.WriteLine($"{result.Timestamp}: Signal detected");
}
```

## How it works

`.Condense()` removes results where all indicator values are null or zero (depending on the indicator). The exact behavior varies by indicator type:

- **Candlestick patterns**: Returns only dates with pattern matches
- **Oscillators**: May remove periods before convergence
- **Moving averages**: Removes null values before the indicator has enough data

## Common use cases

### Candlestick pattern signals

Show only dates when specific patterns occur:

```csharp
// get all Doji patterns
var dojiSignals = quotes
  .ToDoji()
  .Condense();

Console.WriteLine($"Found {dojiSignals.Count} Doji patterns");
```

### Signal-based trading

Build signal lists for trading systems:

```csharp
// get Marubozu signals
var signals = quotes
  .ToMarubozu()
  .Condense();

// execute trades on signal dates
foreach (var signal in signals)
{
  if (signal.Signal > 0)
  {
    // bullish signal logic
  }
  else if (signal.Signal < 0)
  {
    // bearish signal logic
  }
}
```

### Data export

Reduce export size by including only meaningful data:

```csharp
// export only significant events
var condensed = quotes
  .ToMarubozu()
  .Condense();

ExportToCsv(condensed, "marubozu-signals.csv");
```

## Important considerations

::: warning Data reduction
`.Condense()` removes non-essential results and returns less data than the input `quotes`. The exact amount of reduction depends on the indicator and the data.

This is intentional behavior, but be aware that condensed results may have gaps in the timeline.
:::

::: tip When to use
Use `.Condense()` when you only care about specific events or signals, not the continuous time series. This is particularly useful for:

- Candlestick pattern detection
- Signal-based trading systems
- Event logging and reporting
- Reducing storage or export size
:::

## Performance considerations

`.Condense()` is a filtering operation with minimal performance cost. It does not re-calculate the indicator, only filters the existing results.

## Indicator compatibility

Most indicators support `.Condense()`, but the behavior varies:

| Indicator type | Condensed behavior |
|----------------|-------------------|
| Candlestick patterns | Returns only pattern matches |
| Signal-based indicators | Returns only signal points |
| Continuous indicators | May remove warmup period nulls |

## Related utilities

- [Result utilities overview](/utilities/results/)
- [Remove warmup periods](/utilities/results/remove-warmup-periods) - Remove initial convergence periods
- [Find by date](/utilities/results/find-by-date) - Lookup specific results
