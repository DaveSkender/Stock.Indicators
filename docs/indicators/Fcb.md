---
title: Fractal Chaos Bands (FCB)
description: Created by Edward William Dreiss, Fractal Chaos Bands outline high and low price channels to depict broad less-chaotic price movements.  FCB is a channelized depiction of Williams Fractal.
---

# Fractal Chaos Bands (FCB)

Created by Edward William Dreiss, Fractal Chaos Bands outline high and low price channels to depict broad less-chaotic price movements.  FCB is a channelized depiction of <a href="/indicators/Fractal/" rel="nofollow">Williams Fractal</a>.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/347 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Fcb.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<FcbResult> results =
  quotes.ToFcb(windowSpan);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `windowSpan` | int | Fractal evaluation window span width (`S`).  Must be at least 2.  Default is 2. |

The total evaluation window size is `2×S+1`, representing `±S` from the evaluation date.  See [Williams Fractal](/indicators/Fractal) for more information about Fractals and `windowSpan`.

### Historical quotes requirements

You must have at least `2×S+1` periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<FcbResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The periods before the first fractal are `null` since they cannot be calculated.

::: warning 🖌️ Repaint warning
Fractal Chaos Bands are based on [Williams Fractal](/indicators/Fractal), which uses future bars.  This indicator will never identify bands in the last `S` periods of `quotes` since fractals are retroactively identified.
:::

### `FcbResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `UpperBand` | decimal | FCB upper band |
| `LowerBand` | decimal | FCB lower band |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
FcbList fcbList = new(windowSpan);

foreach (IQuote quote in quotes)  // simulating stream
{
  fcbList.Add(quote);
}

// based on `ICollection<FcbResult>`
IReadOnlyList<FcbResult> results = fcbList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
FcbHub observer = quoteHub.ToFcbHub(windowSpan);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<FcbResult> results = observer.Results;
```
