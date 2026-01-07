---
title: Donchian Channels
description: Created by Richard Donchian, Donchian Channels, also called Price Channels, are price ranges derived from highest High and lowest Low values.
---

# Donchian Channels

Created by Richard Donchian, [Donchian Channels](https://en.wikipedia.org/wiki/Donchian_channel), also called Price Channels, are price ranges derived from highest High and lowest Low values.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/257 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Donchian.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<DonchianResult> results =
  quotes.ToDonchian(lookbackPeriods);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) for lookback period.  Must be greater than 0 to calculate; however we suggest a larger value for an appropriate sample size.  Default is 20. |

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<DonchianResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### `DonchianResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `UpperBand` | decimal | Upper line is the highest High over `N` periods |
| `Centerline` | decimal | Simple average of Upper and Lower bands |
| `LowerBand` | decimal | Lower line is the lowest Low over `N` periods |
| `Width` | decimal | Width as percent of Centerline price.  `(UpperBand-LowerBand)/Centerline` |

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
DonchianList donchianList = new(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  donchianList.Add(quote);
}

// based on `ICollection<DonchianResult>`
IReadOnlyList<DonchianResult> results = donchianList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
DonchianHub observer = quoteHub.ToDonchianHub(lookbackPeriods);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<DonchianResult> results = observer.Results;
```
