---
title: ROC with Bands
description: Rate of Change with Bands, created by Vitali Apirine, is a volatility banded variant of the basic Rate of Change (ROC) indicator.
---

# ROC with Bands

Rate of Change (ROC) with Bands, created by Vitali Apirine, is a volatility banded variant of [Rate of Change (ROC)](/indicators/Roc).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/242 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/RocWb.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<RocWbResult> results =
  quotes.ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) to go back.  Must be greater than 0.  Typical values range from 10-20. |
| `emaPeriods` | int | Number of periods for the ROC EMA line.  Must be greater than 0.  Standard is 3. |
| `stdDevPeriods` | int | Number of periods the standard deviation for upper/lower band lines.  Must be greater than 0 and not more than `lookbackPeriods`.  Standard is to use same value as `lookbackPeriods`. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<RocWbResult>
```

### `RocWbResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Roc` | double | Rate of Change over `N` lookback periods (%, not decimal) |
| `RocEma` | double | Exponential moving average (EMA) of `Roc` |
| `UpperBand` | double | Upper band of ROC (overbought indicator) |
| `LowerBand` | double | Lower band of ROC (oversold indicator) |

### Utilities

See [Utilities and helpers](/utilities/results) for more information.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
RocWbList rocWbList = new(lookbackPeriods, emaPeriods, stdDevPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  rocWbList.Add(quote);
}

// based on `ICollection<RocWbResult>`
IReadOnlyList<RocWbResult> results = rocWbList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
RocWbHub observer = quoteHub.ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<RocWbResult> results = observer.Results;
```

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToRocWb(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToRocWb(..)
    .ToEma(..);
```
