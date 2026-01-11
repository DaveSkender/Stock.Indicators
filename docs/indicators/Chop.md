---
title: Choppiness Index
description: Created by E.W. Dreiss, the Choppiness Index measures the trendiness or choppiness on a scale of 0 to 100, to depict steady trends versus conditions of choppiness.
---

# Choppiness Index

Created by E.W. Dreiss, the Choppiness Index measures the trendiness or choppiness on a scale of 0 to 100, to depict steady trends versus conditions of choppiness.  [[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/357 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Chop.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<ChopResult> results =
  quotes.ToChop(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) for the lookback evaluation.  Must be greater than 1.  Default is 14. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ChopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### `ChopResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Chop` | double | Choppiness Index |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Chop` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToChop(..)
    .ToEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
ChopList chopList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  chopList.Add(quote);
}

// based on `ICollection<ChopResult>`
IReadOnlyList<ChopResult> results = chopList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
ChopHub observer = quoteHub.ToChopHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ChopResult> results = observer.Results;
```
