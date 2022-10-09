---
title: Moving Average Convergence / Divergence (MACD)
description: Created by Gerald Appel, MACD is a simple oscillator view of two converging / diverging exponential moving averages and their differences.
permalink: /indicators/Macd/
image: /assets/charts/Macd.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Gerald Appel, [MACD](https://en.wikipedia.org/wiki/MACD) is a simple oscillator view of two converging / diverging exponential moving averages and their differences.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/248 "Community discussion about this indicator")

![image]({{site.baseurl}}{{page.image}})

```csharp
// usage (with Close price)
IEnumerable<MacdResult> results =
  quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods);
```

## Parameters

| name | type | notes
| -- |-- |--
| `fastPeriods` | int | Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 12.
| `slowPeriods` | int | Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 26.
| `signalPeriods` | int | Number of periods (`P`) for the moving average of MACD.  Must be greater than or equal to 0.  Default is 9.

### Historical quotes requirements

You must have at least `2×(S+P)` or `S+P+100` worth of `quotes`, whichever is more, to cover the convergence periods.  Since this uses a smoothing technique, we recommend you use at least `S+P+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<MacdResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `S-1` slow periods will have `null` values since there's not enough data to calculate.

> :hourglass: **Convergence warning**: The first `S+P+250` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### MacdResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Macd` | double | The MACD line is the difference between slow and fast moving averages (`MACD = FastEma - SlowEma`)
| `Signal` | double | Moving average of the `MACD` line
| `Histogram` | double | Gap between of the `MACD` and `Signal` line
| `FastEma` | double | Fast Exponential Moving Average
| `SlowEma` | double | Slow Exponential Moving Average

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetMacd(..);
```

Results can be further processed on `Macd` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetMacd(..)
    .GetSlope(..);
```
