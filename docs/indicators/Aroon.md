---
title: Aroon
description: Created by Tushar Chande, Aroon is a oscillator view of how long ago the new high or low price occurred.
---

# Aroon

Created by Tushar Chande, [Aroon](https://school.stockcharts.com/doku.php?id=technical_indicators:aroon) is a oscillator view of how long ago the new high or low price occurred.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/266 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Aroon.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<AroonResult> results =
  quotes.ToAroon(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) for the lookback evaluation.  Must be greater than 0.  Default is 25. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AroonResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Aroon` since there's not enough data to calculate.

### `AroonResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `AroonUp` | double | Based on last High price |
| `AroonDown` | double | Based on last Low price |
| `Oscillator` | double | AroonUp - AroonDown |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToAroon(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
AroonList aroonList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  aroonList.Add(quote);
}

// based on `ICollection<AroonResult>`
IReadOnlyList<AroonResult> results = aroonList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
AroonHub observer = quoteHub.ToAroonHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<AroonResult> results = observer.Results;
```
