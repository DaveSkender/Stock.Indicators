---
title: SMA with extended analysis
description: Simple Moving Average (SMA) extended with Mean Absolute Deviation (MAD), Mean Square Error (MSE), and Mean Absolute Percentage Error (MAPE).
---

# SMA with extended analysis

[Simple Moving Average](https://en.wikipedia.org/wiki/Moving_average#Simple_moving_average) with extended statistical analysis including Mean Absolute Deviation (MAD), Mean Square Error (MSE), and Mean Absolute Percentage Error (MAPE).  See also [Simple Moving Average](/indicators/Sma).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/240 "Community discussion about this indicator")

```csharp
// C# usage syntax
IReadOnlyList<SmaAnalysisResult> results =
  quotes.ToSmaAnalysis(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback window. Must be greater than 0. |

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<SmaAnalysisResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `SmaAnalysisResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Sma` | double | Simple moving average |
| `Mad` | double | Mean absolute deviation |
| `Mse` | double | Mean square error |
| `Mape` | double | Mean absolute percentage error |

### Utilities

- [.Condense()](/utilities/results/condense)
- [.Find(lookupDate)](/utilities/results/find-by-date)
- [.RemoveWarmupPeriods()](/utilities/results/remove-warmup-periods)
- [.RemoveWarmupPeriods(removePeriods)](/utilities/results/remove-warmup-periods)

See [Utilities and helpers](/utilities/results/) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToSmaAnalysis(..);
```

Results can be further processed on `Sma` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToSmaAnalysis(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
SmaAnalysisList smaAnalysisList = new(lookbackPeriods);

foreach (IReusable value in quotes)  // simulating stream
{
  smaAnalysisList.Add(value);
}

// based on `ICollection<SmaAnalysisResult>`
IReadOnlyList<SmaAnalysisResult> results = smaAnalysisList;
```

Subscribe to a chain-enabled hub for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
SmaAnalysisHub observer = quoteHub.ToSmaAnalysisHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<SmaAnalysisResult> results = observer.Results;
```
