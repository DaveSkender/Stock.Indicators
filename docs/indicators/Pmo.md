---
title: Price Momentum Oscillator (PMO)
description: Created by Carl Swenlin, the DecisionPoint Price Momentum Oscillator is double-smoothed momentum indicator, based on Rate of Change (ROC).
---

# Price Momentum Oscillator (PMO)

Created by Carl Swenlin, the DecisionPoint [Price Momentum Oscillator](https://school.stockcharts.com/doku.php?id=technical_indicators:dppmo) is double-smoothed momentum indicator based on Rate of Change (ROC).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/244 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Pmo.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<PmoResult> results =
  quotes.ToPmo(timePeriods, smoothPeriods, signalPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `timePeriods` | int | Number of periods (`T`) for first ROC smoothing.  Must be greater than 1.  Default is 35. |
| `smoothPeriods` | int | Number of periods (`S`) for second PMO smoothing.  Must be greater than 0.  Default is 20. |
| `signalPeriods` | int | Number of periods (`G`) for Signal line EMA.  Must be greater than 0.  Default is 10. |

### Historical quotes requirements

You must have at least `N` periods of `quotes`, where `N` is the greater of `T+S`, `2×T`, or `T+100` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses multiple smoothing operations, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<PmoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `T+S-1` periods will have `null` values for PMO since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `T+S+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `PmoResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Pmo` | double | Price Momentum Oscillator |
| `Signal` | double | Signal line is EMA of PMO |

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
    .ToPmo(..);
```

Results can be further processed on `Pmo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToPmo(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
PmoList pmoList = new(timePeriods, smoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  pmoList.Add(quote);
}

// based on `ICollection<PmoResult>`
IReadOnlyList<PmoResult> results = pmoList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
PmoHub observer = quoteHub.ToPmoHub(timePeriods, smoothPeriods, signalPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<PmoResult> results = observer.Results;
```
