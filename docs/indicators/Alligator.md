---
title: Williams Alligator
description: Created by Bill Williams, Alligator is a depiction of three smoothed moving averages of median price, showing chart patterns that compared to an alligator's feeding habits when describing market movement. The moving averages are known as the Jaw, Teeth, and Lips, which are calculated using lookback and offset periods.  The related Gator Oscillator depicts periods of eating and resting.
---

# Williams Alligator

Created by Bill Williams, Alligator is a depiction of three smoothed moving averages of median price, showing chart patterns that compared to an alligator's feeding habits when describing market movement. The moving averages are known as the Jaw, Teeth, and Lips, which are calculated using lookback and offset periods.  See also the [Gator Oscillator](/indicators/Gator).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/385 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Alligator.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<AlligatorResult> results =
  quotes.ToAlligator(jawPeriods,jawOffset,teethPeriods,teethOffset,lipsPeriods,lipsOffset);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `jawPeriods` | int | Number of periods (`JP`) for the Jaw moving average.  Must be greater than `teethPeriods`.  Default is 13. |
| `jawOffset` | int | Number of periods (`JO`) for the Jaw offset.  Must be greater than 0.  Default is 8. |
| `teethPeriods` | int | Number of periods (`TP`) for the Teeth moving average.  Must be greater than `lipsPeriods`.  Default is 8. |
| `teethOffset` | int | Number of periods (`TO`) for the Teeth offset.  Must be greater than 0.  Default is 5. |
| `lipsPeriods` | int | Number of periods (`LP`) for the Lips moving average.  Must be greater than 0.  Default is 5. |
| `lipsOffset` | int | Number of periods (`LO`) for the Lips offset.  Must be greater than 0.  Default is 3. |

### Historical quotes requirements

You must have at least `JP+JO+100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods. Since this uses a smoothing technique, we recommend you use at least `JP+JO+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AlligatorResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `JP+JO` periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `JP+JO+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `AlligatorResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Jaw` | double | Alligator's Jaw |
| `Teeth` | double | Alligator's Teeth |
| `Lips` | double | Alligator's Lips |

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
    .ToAlligator();
```

Results **cannot** be further chained with additional transforms.

## Streaming

Subscribe to a `QuoteHub` for streaming scenarios:

```csharp
QuoteHub quoteHub = new();
AlligatorHub observer = quoteHub.ToAlligatorHub();

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<AlligatorResult> results = observer.Results;
```
