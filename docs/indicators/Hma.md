---
title: Hull Moving Average (HMA)
description: Created by Alan Hull, the Hull Moving Average is a modified weighted average of price that reduces lag.
---

# Hull Moving Average (HMA)

Created by Alan Hull, the [Hull Moving Average](https://alanhull.com/hull-moving-average) is a modified weighted average of price that reduces lag.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/252 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Hma.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<HmaResult> results =
  quotes.ToHma(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 1. |

### Historical quotes requirements

You must have at least `N+(integer of SQRT(N))-1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<HmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N+(integer of SQRT(N))-1` periods will have `null` values since there's not enough data to calculate.

### `HmaResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Hma` | double | Hull moving average |

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
    .ToHma(..);
```

Results can be further processed on `Hma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToHma(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
HmaList hmaList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  hmaList.Add(quote);
}

// based on `ICollection<HmaResult>`
IReadOnlyList<HmaResult> results = hmaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
HmaHub observer = quoteHub.ToHmaHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<HmaResult> results = observer.Results;
```
