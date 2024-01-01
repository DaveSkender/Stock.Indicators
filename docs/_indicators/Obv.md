---
title: On-Balance Volume (OBV)
description: Popularized by Joseph Granville, On-balance Volume is a rolling accumulation of volume based on Close price direction.
permalink: /indicators/Obv/
image: /assets/charts/Obv.png
type: volume-based
layout: indicator
---

# {{ page.title }}

Popularized by Joseph Granville, [On-balance Volume](https://en.wikipedia.org/wiki/On-balance_volume) is a rolling accumulation of volume based on Close price direction.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/246 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<ObvResult> results =
  quotes.GetObv();

// usage with optional overlay SMA of OBV (shown above)
IEnumerable<ObvResult> results =
  quotes.GetObv(smaPeriods);
```

## Parameters

**`smaPeriods`** _`int`_ - Optional.  Number of periods (`N`) in the moving average of OBV.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, since this is a trendline, more is recommended.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ObvResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period OBV will have `0` value since there's not enough data to calculate.

### ObvResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Obv`** _`double`_ - On-balance Volume

**`ObvSma`** _`double`_ - Moving average (SMA) of OBV based on `smaPeriods` periods, if specified

> &#128681; **Warning**: absolute values in OBV are somewhat meaningless. Use with caution.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Obv` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetObv(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
