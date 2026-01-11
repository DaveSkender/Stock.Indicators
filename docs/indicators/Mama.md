---
title: MESA Adaptive Moving Average (MAMA)
description: Created by John Ehlers, the MAMA indicator is a 5-period adaptive moving average of high/low price that uses classic electrical radio-frequency signal processing algorithms to reduce noise.
---

# MESA Adaptive Moving Average (MAMA)

Created by John Ehlers, the [MAMA](https://mesasoftware.com/papers/MAMA.pdf) indicator is a 5-period adaptive moving average of high/low price that uses classic electrical radio-frequency signal processing algorithms to reduce noise.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/211 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Mama.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<MamaResult> results =
  quotes.ToMama(fastLimit, slowLimit);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `fastLimit` | double | Fast limit threshold.  Must be greater than `slowLimit` and less than 1.  Default is 0.5. |
| `slowLimit` | double | Slow limit threshold.  Must be greater than 0.  Default is 0.05. |

### Historical quotes requirements

You must have at least `50` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<MamaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `5` periods will have `null` values for `Mama` since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `50` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `MamaResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Mama` | double | MESA adaptive moving average (MAMA) |
| `Fama` | double | Following adaptive moving average (FAMA) |

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
    .ToMama(..);
```

Results can be further processed on `Mama` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToMama(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
MamaList mamaList = new(fastLimit, slowLimit);

foreach (IQuote quote in quotes)  // simulating stream
{
  mamaList.Add(quote);
}

// based on `ICollection<MamaResult>`
IReadOnlyList<MamaResult> results = mamaList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
MamaHub observer = quoteHub.ToMamaHub(fastLimit, slowLimit);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<MamaResult> results = observer.Results;
```
