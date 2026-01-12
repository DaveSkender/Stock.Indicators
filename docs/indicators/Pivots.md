---
title: Pivots
description: Pivots is an extended customizable version of Williams Fractal that includes identification of Higher High, Lower Low, Higher Low, and Lower Low trends between pivots in a lookback window.
---

# Pivots

Pivots is an extended customizable version of <a href="/indicators/Fractal/" rel="nofollow">Williams Fractal</a> that includes identification of Higher High, Lower Low, Higher Low, and Lower Low trends between pivots in a lookback window.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/436 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Pivots.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<PivotsResult> results =
  quotes.ToPivots(leftSpan, rightSpan, maxTrendPeriods, endType);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `leftSpan` | int | Left evaluation window span width (`L`).  Must be at least 2.  Default is 2. |
| `rightSpan` | int | Right evaluation window span width (`R`).  Must be at least 2.  Default is 2. |
| `maxTrendPeriods` | int | Number of periods (`N`) in evaluation window.  Must be greater than `leftSpan`.  Default is 20. |
| `endType` | EndType | Determines whether `Close` or `High/Low` are used to find end points.  See [EndType options](#endtype-options) below.  Default is `EndType.HighLow`. |

The total evaluation window size is `L+R+1`.

### Historical quotes requirements

You must have at least `L+R+1` periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

### EndType options

**`EndType.Close`** - Chevron point identified from `Close` price

**`EndType.HighLow`** - Chevron point identified from `High` and `Low` price (default)

## Response

```csharp
IReadOnlyList<PivotsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `L` and last `R` periods in `quotes` are unable to be calculated since there's not enough prior/following data.

::: warning 🖌️ Repaint warning
This price pattern looks forward and backward in the historical quotes so it will never identify a pivot in the last `R` periods of `quotes`.  Pivots are retroactively identified.
:::

### `PivotsResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `HighPoint` | decimal | Value indicates a **high** point; otherwise `null` is returned. |
| `LowPoint` | decimal | Value indicates a **low** point; otherwise `null` is returned. |
| `HighLine` | decimal | Drawn line between two high points in the `maxTrendPeriods` |
| `LowLine` | decimal | Drawn line between two low points in the `maxTrendPeriods` |
| `HighTrend` | PivotTrend | Enum that represents higher high or lower high.  See [PivotTrend values](#pivottrend-values) below. |
| `LowTrend` | PivotTrend | Enum that represents higher low or lower low.  See [PivotTrend values](#pivottrend-values) below. |

#### PivotTrend values

**`PivotTrend.HH`** - Higher high

**`PivotTrend.LH`** - Lower high

**`PivotTrend.HL`** - Higher low

**`PivotTrend.LL`** - Lower low

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
PivotsList pivotsList = new(leftSpan, rightSpan, maxTrendPeriods, endType);

foreach (IQuote quote in quotes)  // simulating stream
{
  pivotsList.Add(quote);
}

// based on `ICollection<PivotsResult>`
IReadOnlyList<PivotsResult> results = pivotsList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
PivotsHub observer = quoteHub.ToPivotsHub(leftSpan, rightSpan, maxTrendPeriods, endType);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<PivotsResult> results = observer.Results;
```
