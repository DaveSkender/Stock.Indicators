---
title: Standard Deviation (volatility)
description: Standard Deviation represents the volatility of historical financial market prices.  It is also known as Historical Volatility (HV). Z-Score is also returned.
permalink: /indicators/StdDev/
image: /assets/charts/StdDev.png
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Standard Deviation](https://en.wikipedia.org/wiki/Standard_deviation) of price over a rolling lookback window.  Also known as Historical Volatility (HV).  Z-Score is also returned.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/239 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<StdDevResult> results =
  quotes.GetStdDev(lookbackPeriods);

// usage with optional SMA of SD (shown above)
IEnumerable<StdDevResult> results =
  quotes.GetStdDev(lookbackPeriods, smaPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback period.  Must be greater than 1 to calculate; however we suggest a larger period for statistically appropriate sample size.

**`smaPeriods`** _`int`_ - Optional.  Number of periods in the moving average of `StdDev`.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StdDevResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### StdDevResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`StdDev`** _`double`_ - Standard Deviation of price

**`Mean`** _`double`_ - Mean value of price

**`ZScore`** _`double`_ - Z-Score of current price (number of standard deviations from mean)

**`StdDevSma`** _`double`_ - Moving average (SMA) of `StdDev` based on `smaPeriods` periods, if specified

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetStdDev(..);
```

Results can be further processed on `StdDev` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetStdDev(..)
    .GetSlope(..);
```
