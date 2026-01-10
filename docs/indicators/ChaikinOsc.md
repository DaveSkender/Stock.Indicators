---
title: Chaikin Oscillator
description: Created by Marc Chaikin, the Chaikin Oscillator is the difference between fast and slow Exponential Moving Averages (EMA) of an Accumulation / Distribution Line (ADL).
---

# Chaikin Oscillator

Created by Marc Chaikin, the [Chaikin Oscillator](https://en.wikipedia.org/wiki/Chaikin_Analytics#Chaikin_Oscillator) is the difference between fast and slow Exponential Moving Averages (EMA) of the [Accumulation/Distribution Line](/indicators/Adl) (ADL).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/264 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/ChaikinOsc.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<ChaikinOscResult> results =
  quotes.ToChaikinOsc(fastPeriods, slowPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `fastPeriods` | int | Number of periods (`F`) in the ADL fast EMA.  Must be greater than 0 and smaller than `S`.  Default is 3. |
| `slowPeriods` | int | Number of periods (`S`) in the ADL slow EMA.  Must be greater than `F`.  Default is 10. |

### Historical quotes requirements

You must have at least `2×S` or `S+100` periods of `quotes`, whichever is more,  to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `S+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<ChaikinOscResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` periods will have `null` values for `Oscillator` since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `S+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `ChaikinOscResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `MoneyFlowMultiplier` | double | Money Flow Multiplier |
| `MoneyFlowVolume` | double | Money Flow Volume |
| `Adl` | double | Accumulation Distribution Line (ADL) |
| `Oscillator` | double | Chaikin Oscillator |

::: warning
absolute values in MFV, ADL, and Oscillator are somewhat meaningless.  Use with caution.
:::

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
    .ToChaikinOsc(..)
    .ToSlope(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
ChaikinOscList chaikinOscList = new(fastPeriods, slowPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  chaikinOscList.Add(quote);
}

// based on `ICollection<ChaikinOscResult>`
IReadOnlyList<ChaikinOscResult> results = chaikinOscList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
ChaikinOscHub observer = quoteHub.ToChaikinOscHub(fastPeriods, slowPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<ChaikinOscResult> results = observer.Results;
```
