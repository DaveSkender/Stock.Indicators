---
title: Ulcer Index (UI)
description: Created by Peter Martin, the Ulcer Index is a measure of downside price volatility.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
---

# Ulcer Index (UI)

Created by Peter Martin, the [Ulcer Index](https://en.wikipedia.org/wiki/Ulcer_index) is a measure of downside price volatility over a lookback window.  Often called the "heart attack" score, it measures the amount of pain seen from drawdowns in financial market prices and portfolio value.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/232 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/UlcerIndex.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<UlcerIndexResult> results =
  quotes.ToUlcerIndex(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) for review.  Must be greater than 0.  Default is 14. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<UlcerIndexResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `UlcerIndexResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `UI` | double | Ulcer Index |

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
    .ToUlcerIndex(..);
```

Results can be further processed on `UI` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToUlcerIndex(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
UlcerIndexList ulcerIndexList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  ulcerIndexList.Add(quote);
}

// based on `ICollection<UlcerIndexResult>`
IReadOnlyList<UlcerIndexResult> results = ulcerIndexList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
UlcerIndexHub observer = quoteHub.ToUlcerIndexHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<UlcerIndexResult> results = observer.Results;
```
