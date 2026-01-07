---
title: Average Directional Index (ADX)
description: Created by J. Welles Wilder, the Average Directional Index (ADX) is part of the Directional Movement system (commonly referred to as DMI). This system includes the Positive and Negative Directional Indicators (+DI and −DI), the Directional Index (DX), and ADX, and is used to measure the strength of price trends.
---

# Average Directional Index (ADX)

Created by J. Welles Wilder, the [Average Directional Movement Index](https://en.wikipedia.org/wiki/Average_directional_movement_index) (ADX) is part of the Directional Movement system (commonly referred to as DMI). This system includes the Positive and Negative Directional Indicators (+DI and −DI), the Directional Index (DX), and ADX, and is used to measure the strength of price trends.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/270 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Adx.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) to consider.  Must be greater than 1.  Default is 14. |

### Historical quotes requirements

You must have at least `2×N+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  We generally recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AdxResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `2×N-1` periods will have `null` values for `Adx` since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `AdxResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Pdi` | double | Plus Directional Index (+DI) |
| `Mdi` | double | Minus Directional Index (-DI) |
| `Dx` | double | Directional Index (DX) |
| `Adx` | double | Average Directional Index (ADX) |
| `Adxr` | double | Average Directional Index Rating (ADXR) |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Adx` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToAdx(..)
    .ToRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations:

```csharp
AdxList adxList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  adxList.Add(quote);
}

// based on `ICollection<AdxResult>`
IReadOnlyList<AdxResult> results = adxList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
AdxHub observer = quoteHub.ToAdxHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<AdxResult> results = observer.Results;
```
