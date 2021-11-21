---
title: Triple Exponential Moving Average (TEMA)
permalink: /indicators/TripleEma/
type: moving-average
layout: indicator
---

# {{ page.title }}

[Triple exponential moving average](https://en.wikipedia.org/wiki/Triple_exponential_moving_average) of the Close price over a lookback window.
Note: TEMA is often confused with the alternative [TRIX](../Trix#content) oscillator.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/256 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/TripleEma.png)

TEMA is shown as the dotted line above.  [EMA](../Ema#content) (solid line) and [Double EMA](../DoubleEma#content) (dashed line) are also shown here for comparison.

```csharp
// usage
IEnumerable<TemaResult> results =
  quotes.GetTripleEma(lookbackPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `4×N` or `3×N+100` periods of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `3×N+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<TemaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `3×N-2` periods will have `null` values since there's not enough data to calculate.  Also note that we are using the proper [weighted variant](https://en.wikipedia.org/wiki/Triple_exponential_moving_average) for TEMA.  If you prefer the unweighted raw 3 EMAs value, please use the `Ema3` output from the [TRIX](../Trix#content) oscillator instead.

:hourglass: **Convergence Warning**: The first `3×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### TemaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Tema` | decimal | Triple exponential moving average

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate 20-period TEMA
IEnumerable<TemaResult> results = quotes.GetTripleEma(20);
```
