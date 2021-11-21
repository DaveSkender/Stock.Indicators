---
title: Keltner Channels
permalink: /indicators/Keltner/
type: price-channel
layout: indicator
---

# {{ page.title }}

Created by Chester W. Keltner, [Keltner Channels](https://en.wikipedia.org/wiki/Keltner_channel) are based on an EMA centerline and ATR band widths.  See also [STARC Bands](../StarcBands#content) for an SMA centerline equivalent.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/249 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Keltner.png)

```csharp
// usage
IEnumerable<KeltnerResult> results =
  quotes.GetKeltner(emaPeriods, multiplier, atrPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `emaPeriods` | int | Number of lookback periods (`E`) for the center line moving average.  Must be greater than 1 to calculate.  Default is 20.
| `multiplier` | decimal | ATR Multiplier. Must be greater than 0.  Default is 2.
| `atrPeriods` | int | Number of lookback periods (`A`) for the Average True Range.  Must be greater than 1 to calculate.  Default is 10.

### Historical quotes requirements

You must have at least `2×N` or `N+100` periods of `quotes`, whichever is more, where `N` is the greater of `E` or `A` periods.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<KeltnerResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### KeltnerResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `UpperBand` | decimal | Upper band of Keltner Channel
| `Centerline` | decimal | EMA of Close price
| `LowerBand` | decimal | Lower band of Keltner Channel
| `Width` | decimal | Width as percent of Centerline price.  `(UpperBand-LowerBand)/Centerline`

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate Keltner(20)
IEnumerable<KeltnerResult> results
  = quotes.GetKeltner(20,2.0,10);
```
