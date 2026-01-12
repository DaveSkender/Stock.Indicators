---
title: Volume Weighted Average Price (VWAP)
description: The Volume Weighted Average Price is a volume weighted average of price, typically used on intraday data. Trading above or below the VWAP line can assist in finding favorable short-term trading windows.
---

# Volume Weighted Average Price (VWAP)

The [Volume Weighted Average Price](https://en.wikipedia.org/wiki/Volume-weighted_average_price) is a Volume weighted average of price, typically used on intraday data.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/310 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Vwap.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<VwapResult> results =
  quotes.ToVwap();

// usage with optional anchored start date
IReadOnlyList<VwapResult> results =
  quotes.ToVwap(startDate);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `startDate` | DateTime | Optional.  The anchor date used to start the VWAP accumulation.  The earliest date in `quotes` is used when not provided. |

### Historical quotes requirements

You must have at least one historical quote to calculate; however, more is often needed to be useful.  Historical quotes are typically provided for a single day using minute-based intraday periods.  Since this is an accumulated weighted average price, different start dates will produce different results.  The accumulation starts at the first period in the provided `quotes`, unless it is specified in the optional `startDate` parameter.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<VwapResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period or the `startDate` will have a `Vwap = Close` value since it is the initial starting point.
- `Vwap` values before `startDate`, if specified, will be `null`.

### `VwapResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Vwap` | double | Volume Weighted Average Price |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

Results can be further processed on `Vwap` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToVwap(..)
    .ToRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
VwapList vwapList = new(startDate);

foreach (IQuote quote in quotes)  // simulating stream
{
  vwapList.Add(quote);
}

// based on `ICollection<VwapResult>`
IReadOnlyList<VwapResult> results = vwapList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
VwapHub observer = quoteHub.ToVwapHub(startDate);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<VwapResult> results = observer.Results;
```
