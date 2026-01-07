---
title: Rate of Change (ROC)
description: Rate of Change, also known as Momentum Oscillator, is the percent change of price over a lookback window.  Momentum is the raw price change equivalent.
---

# Rate of Change (ROC)

[Rate of Change](https://en.wikipedia.org/wiki/Momentum_(technical_analysis)), also known as Momentum Oscillator, is the percent change of price over a lookback window.  Momentum is the raw price change equivalent.  A [Rate of Change with Bands](/indicators/RocWb) variant, created by Vitali Apirine, is also available.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/242 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Roc.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<RocResult> results =
  quotes.ToRoc(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) to go back.  Must be greater than 0. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<RocResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ROC since there's not enough data to calculate.

### `RocResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Momentum` | double | Raw change in price over `N` periods |
| `Roc` | double | Percent change in price (%, not decimal) |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToRoc(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToRoc(..)
    .ToEma(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
RocList rocList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  rocList.Add(quote);
}

// based on `ICollection<RocResult>`
IReadOnlyList<RocResult> results = rocList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
RocHub observer = quoteHub.ToRocHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<RocResult> results = observer.Results;
```
