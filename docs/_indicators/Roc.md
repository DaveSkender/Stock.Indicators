---
title: Rate of Change (ROC)
description: Rate of Change (ROC), Momentum Oscillator, and ROC with Bands
permalink: /indicators/Roc/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

[Rate of Change](https://en.wikipedia.org/wiki/Momentum_(technical_analysis)), also known as Momentum Oscillator, is the percent change of Close price over a lookback window.  A [Rate of Change with Bands](#roc-with-bands) variant, created by Vitali Apirine, is also included.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/242 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Roc.png)

```csharp
// usage
IEnumerable<RocResult> results =
  quotes.GetRoc(lookbackPeriods);

// usage with optional SMA of ROC (shown above)
IEnumerable<RocResult> results =
  quotes.GetRoc(lookbackPeriods, smaPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) to go back.  Must be greater than 0.
| `smaPeriods` | int | Optional.  Number of periods in the moving average of ROC.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<RocResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ROC since there's not enough data to calculate.

### RocResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Roc` | decimal | Rate of Change over `N` lookback periods (%, not decimal)
| `RocSma` | decimal | Moving average (SMA) of ROC based on `smaPeriods` periods, if specified

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate 20-period ROC
IEnumerable<RocResult> results = quotes.GetRoc(20);
```

## ROC with Bands

![image]({{site.baseurl}}/assets/charts/RocWb.png)

```csharp
// usage
IEnumerable<RocWbResult> results =
  quotes.GetRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
```

### Parameters with Bands

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) to go back.  Must be greater than 0.  Typical values range from 10-20.
| `emaPeriods` | int | Number of periods for the ROC EMA line.  Must be greater than 0.  Standard is 3.
| `stdDevPeriods` | int | Number of periods the standard deviation for upper/lower band lines.  Must be greater than 0 and not more than `lookbackPeriods`.  Standard is to use same value as `lookbackPeriods`.

### RocWbResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Roc` | decimal | Rate of Change over `N` lookback periods (%, not decimal)
| `RocEma` | decimal | Exponential moving average (EMA) of `Roc`
| `UpperBand` | decimal | Upper band of ROC (overbought indicator)
| `LowerBand` | decimal | Lower band of ROC (oversold indicator)
