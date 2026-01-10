---
title: Gator Oscillator
description: Created by Bill Williams, the Gator Oscillator is an expanded oscillator view of Williams Alligator's three moving averages.
---

# Gator Oscillator

Created by Bill Williams, the Gator Oscillator is an expanded oscillator view of [Williams Alligator](/indicators/Alligator)'s three moving averages.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/385 "Community discussion about this indicator")

```csharp
// C# usage syntax
IReadOnlyList<GatorResult> results =
  quotes.ToGator();

// with custom Alligator configuration
IReadOnlyList<GatorResult> results = quotes
  .ToAlligator([see Alligator docs])
  .ToGator();
```

## Historical quotes requirements

If using default settings, you must have at least 121 periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods. Since this uses a smoothing technique, we recommend you use at least 271 data points prior to the intended usage date for better precision.  If using a custom Alligator configuration, see [Alligator documentation](/indicators/Alligator#historical-quotes-requirements) for historical quotes requirements.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<GatorResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first 10-20 periods will have `null` values since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first 150 periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `GatorResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Upper` | double | Absolute value of Alligator `Jaw-Teeth` |
| `Lower` | double | Absolute value of Alligator `Lips-Teeth` |
| `UpperIsExpanding` | bool | Upper value is growing |
| `LowerIsExpanding` | bool | Lower value is growing |

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
    .Use(CandlePart.HLC3)
    .ToGator();
```

Results **cannot** be further chained with additional transforms.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
GatorList gatorList = new();

foreach (IQuote quote in quotes)  // simulating stream
{
  gatorList.Add(quote);
}

// based on `ICollection<GatorResult>`
IReadOnlyList<GatorResult> results = gatorList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
GatorHub observer = quoteHub.ToGatorHub();

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<GatorResult> results = observer.Results;
```
