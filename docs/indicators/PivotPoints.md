---
title: Pivot Points
description: Pivot Points depict classic support and resistance levels, based on prior calendar windows.  You can specify window size (e.g. month, week, day, etc) and any of the traditional Floor Trading, Camarilla, Demark, Fibonacci, and Woodie variants.
---

# Pivot Points

[Pivot Points](https://en.wikipedia.org/wiki/Pivot_point_(technical_analysis)) depict support and resistance levels, based on prior calendar windows.  You can specify window size (e.g. month, week, day, etc) and any of the traditional Floor Trading, Camarilla, Demark, Fibonacci, and Woodie variants.

[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/274 "Community discussion about this indicator")

```csharp
// C# usage syntax
IReadOnlyList<PivotPointsResult> results =
  quotes.ToPivotPoints(windowSize, pointType);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `windowSize` | PeriodSize | Size of the lookback window |
| `pointType` | PivotPointType | Type of Pivot Point.  Default is `PivotPointType.Standard` |

### Historical quotes requirements

You must have at least `2` windows of `quotes` to cover the warmup periods.  For example, if you specify a `Week` window size, you need at least 14 calendar days of `quotes`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

### PeriodSize options (for windowSize)

**`PeriodSize.Month`** - Use the prior month's data to calculate current month's Pivot Points

**`PeriodSize.Week`** - [..] weekly

**`PeriodSize.Day`** - [..] daily.  Commonly used for intraday data.

**`PeriodSize.OneHour`** - [..] hourly

### PivotPointType options

**`PivotPointType.Standard`** - Floor Trading (default)

**`PivotPointType.Camarilla`** - Camarilla

**`PivotPointType.Demark`** - Demark

**`PivotPointType.Fibonacci`** - Fibonacci

**`PivotPointType.Woodie`** - Woodie

## Response

```csharp
IReadOnlyList<PivotPointsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first window will have `null` values since there's not enough data to calculate.

::: warning
The second window may be inaccurate if the first window contains incomplete data.  For example, this can occur if you specify a `Month` window size and only provide 45 calendar days (1.5 months) of `quotes`.
:::

::: warning 🖌️ Repaint warning
The last window will be repainted if it does not contain a full window of data.
:::

### `PivotPointsResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `R3` | decimal | Resistance level 3 |
| `R2` | decimal | Resistance level 2 |
| `R1` | decimal | Resistance level 1 |
| `PP` | decimal | Pivot Point |
| `S1` | decimal | Support level 1 |
| `S2` | decimal | Support level 2 |
| `S3` | decimal | Support level 3 |

### Utilities

- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
PivotPointsList pivotPointsList = new(windowSize, pointType);

foreach (IQuote quote in quotes)  // simulating stream
{
  pivotPointsList.Add(quote);
}

// based on `ICollection<PivotPointsResult>`
IReadOnlyList<PivotPointsResult> results = pivotPointsList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
PivotPointsHub observer = quoteHub.ToPivotPointsHub(windowSize, pointType);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<PivotPointsResult> results = observer.Results;
```
