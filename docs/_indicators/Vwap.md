---
title: Volume Weighted Average Price (VWAP)
permalink: /indicators/Vwap/
type: moving-average
layout: indicator
---

# {{ page.title }}

The [Volume Weighted Average Price](https://en.wikipedia.org/wiki/Volume-weighted_average_price) is a Volume weighted average of price, typically used on intraday data.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/310 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Vwap.png)

```csharp
// usage
IEnumerable<VwapResult> results =
  quotes.GetVwap();

// usage with optional anchored start date
IEnumerable<VwapResult> results =
  quotes.GetVwap(startDate);
```

## Parameters

| name | type | notes
| -- |-- |--
| `startDate` | DateTime | Optional.  The anchor date used to start the VWAP accumulation.  The earliest date in `quotes` is used when not provided.

### Historical quotes requirements

You must have at least one historical quote to calculate; however, more is often needed to be useful.  Historical quotes are typically provided for a single day using minute-based intraday periods.  Since this is an accumulated weighted average price, different start dates will produce different results.  The accumulation starts at the first period in the provided `quotes`, unless it is specified in the optional `startDate` parameter.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<VwapResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period or the `startDate` will have a `Vwap = Close` value since it is the initial starting point.
- `Vwap` values before `startDate`, if specified, will be `null`.

### VwapResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Vwap` | double | Volume Weighted Average Price

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Vwap` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetVwap(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
