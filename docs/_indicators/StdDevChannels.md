---
title: Standard Deviation Channels
permalink: /indicators/StdDevChannels/
type: price-channel
layout: indicator
---

# {{ page.title }}

Standard Deviation Channels are based on an linear regression centerline and standard deviations band widths.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/368 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/StdDevChannels.png)

```csharp
// usage
IEnumerable<StdDevChannelsResult> results =
  quotes.GetStdDevChannels(lookbackPeriods, standardDeviations);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Size (`N`) of the evaluation window.  Must be `null` or greater than 1 to calculate.  A `null` value will produce a full `quotes` evaluation window ([see below](#alternative-depiction-for-full-quotes-variant)).  Default is 20.
| `standardDeviations` | int | Width of bands.  Standard deviations (`D`) from the regression line.  Must be greater than 0.  Default is 2.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StdDevChannelsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- Up to `N-1` periods will have `null` values since there's not enough data to calculate.

:paintbrush: **Repaint Warning**: Historical results are a function of the current period window position and will fluctuate over time.  Recommended for visualization; not recommended for backtesting.

### StdDevChannelsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Centerline` | decimal | Linear regression line (center line)
| `UpperChannel` | decimal | Upper line is `D` standard deviations above the center line
| `LowerChannel` | decimal | Lower line is `D` standard deviations below the center line
| `BreakPoint` | bool | Helper information.  Indicates first point in new window.

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate StdDevChannels(20,2)
IEnumerable<StdDevChannelsResult> results
  = quotes.GetStdDevChannels(20,2);
```

## Alternative depiction for full quotes variant

If you specify `null` for the `lookbackPeriods`, you will get a regression line over the entire provided `quotes`.

![image]({{site.baseurl}}/assets/charts/StdDevChannelsFull.png)
