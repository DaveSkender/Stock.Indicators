---
title: Simple Moving Average (SMA)
permalink: /indicators/Sma/
layout: default
---

# {{ page.title }}

[Simple Moving Average](https://en.wikipedia.org/wiki/Moving_average#Simple_moving_average) is the average of Close price over a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/240 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Sma.png)

```csharp
// usage
IEnumerable<SmaResult> results =
  quotes.GetSma(lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback window.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide#historical-quotes) for more information.

## Response

```csharp
IEnumerable<SmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### SmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sma` | decimal | Simple moving average

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA
IEnumerable<SmaResult> results = quotes.GetSma(20);
```

## Extended analysis

An extended variant of this indicator includes additional analysis.

```csharp
// usage
IEnumerable<SmaExtendedResult> results =
  quotes.GetSmaExtended(lookbackPeriods);  
```

### SmaExtendedResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sma` | decimal | Simple moving average
| `Mad` | decimal | Mean absolute deviation
| `Mse` | decimal | Mean square error
| `Mape` | decimal | Mean absolute percentage error
