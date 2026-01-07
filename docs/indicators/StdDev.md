---
title: Standard Deviation (volatility)
description: Standard Deviation represents the volatility of historical financial market prices.  It is also known as Historical Volatility (HV). Z-Score is also returned.
---

# Standard Deviation (volatility)

[Standard Deviation](https://en.wikipedia.org/wiki/Standard_deviation) of price over a rolling lookback window.  Also known as Historical Volatility (HV).  Z-Score is also returned.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/239 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/StdDev.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax (series)
IReadOnlyList<StdDevResult> results =
  quotes.ToStdDev(lookbackPeriods);

// usage with streaming quotes
QuoteHub quoteHub = new();
StdDevHub observer = quoteHub.ToStdDevHub(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<StdDevResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `StdDevResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `StdDev` | double | Standard Deviation of price |
| `Mean` | double | Mean value of price |
| `ZScore` | double | Z-Score of current price (number of standard deviations from mean) |

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
    .ToStdDev(..);
```

Results can be further processed on `StdDev` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToStdDev(..)
    .ToSlope(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
StdDevList stdDevList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  stdDevList.Add(quote);
}

// based on `ICollection<StdDevResult>`
IReadOnlyList<StdDevResult> results = stdDevList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
StdDevHub observer = quoteHub.ToStdDevHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<StdDevResult> results = observer.Results;
```
