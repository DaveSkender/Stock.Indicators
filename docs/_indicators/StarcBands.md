---
title: STARC Bands
permalink: /indicators/StarcBands/
type: price-channel
layout: indicator
---

# {{ page.title }}

Created by Manning Stoller, [Stoller Average Range Channel (STARC) Bands](https://www.investopedia.com/terms/s/starc.asp), are based on an SMA centerline and ATR band widths.  See also [Keltner Channels]({{site.baseurl}}/indicators/Keltner/#content) for an EMA centerline equivalent.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/292 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/StarcBands.png)

```csharp
// usage
IEnumerable<StarcBandsResult> results =
  quotes.GetStarcBands(smaPeriods, multiplier, atrPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `smaPeriods` | int | Number of lookback periods (`S`) for the center line moving average.  Must be greater than 1 to calculate and is typically between 5 and 10.
| `multiplier` | double | ATR Multiplier. Must be greater than 0.  Default is 2.
| `atrPeriods` | int | Number of lookback periods (`A`) for the Average True Range.  Must be greater than 1 to calculate and is typically the same value as `smaPeriods`.  Default is 10.

### Historical quotes requirements

You must have at least `S` or `A+100` periods of `quotes`, whichever is more, to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `A+150` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StarcBandsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate, where `N` is the greater of `S` or `A`.

:hourglass: **Convergence Warning**: The first `A+150` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### StarcBandsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `UpperBand` | decimal | Upper STARC band
| `Centerline` | decimal | SMA of price
| `LowerBand` | decimal | Lower STARC band

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
