---
title: Exponential Moving Average (EMA)
permalink: /indicators/Ema/
type: moving-average
layout: indicator
---

# {{ page.title }}

[Exponentially weighted moving average](https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average) price over a lookback window.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/256 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Ema.png)

EMA is shown as the solid line above.  [Double EMA](../DoubleEma#content) (dashed line) and [Triple EMA](../TripleEma#content) (dotted line) are also shown here for comparison.

```csharp
// usage (with Close price)
IEnumerable<EmaResult> results =
  quotes.GetEma(lookbackPeriods);

// alternate
IEnumerable<EmaResult> results =
  quotes.GetEma(lookbackPeriods, candlePart);
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.
| `candlePart` | CandlePart | Optional.  Specify the [OHLCV]({{site.baseurl}}/guide/#historical-quotes) candle part to evaluate.  See [CandlePart options](#candlepart-options) below.  Default is `CandlePart.Close`

### Historical quotes requirements

You must have at least `2×N` or `N+100` periods of `quotes`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### CandlePart options

| type | description
|-- |--
| `CandlePart.Open` | Use `Open` price
| `CandlePart.High` | Use `High` price
| `CandlePart.Low` | Use `Low` price
| `CandlePart.Close` | Use `Close` price (default)
| `CandlePart.Volume` | Use `Volume`

## Response

```csharp
IEnumerable<EmaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### EmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Ema` | decimal | Exponential moving average

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate 20-period EMA
IEnumerable<EmaResult> results = quotes.GetEma(20);
```
