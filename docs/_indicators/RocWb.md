---
title: ROC with Bands
description: Rate of Change with Bands, created by Vitali Apirine, is a volatility banded variant of the basic Rate of Change (ROC) indicator.

permalink: /indicators/RocWb/
image: /assets/charts/RocWb.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Rate of Change (ROC) with Bands, created by Vitali Apirine, is a volatility banded variant of [Rate of Change (ROC)]({{site.baseurl}}/indicators/Roc/#content).
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/242 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<RocWbResult> results =
  quotes.GetRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to go back.  Must be greater than 0.  Typical values range from 10-20.

**`emaPeriods`** _`int`_ - Number of periods for the ROC EMA line.  Must be greater than 0.  Standard is 3.

**`stdDevPeriods`** _`int`_ - Number of periods the standard deviation for upper/lower band lines.  Must be greater than 0 and not more than `lookbackPeriods`.  Standard is to use same value as `lookbackPeriods`.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<RocWbResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ROC since there's not enough data to calculate.

### RocWbResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Roc`** _`double`_ - Rate of Change over `N` lookback periods (%, not decimal)

**`RocEma`** _`double`_ - Exponential moving average (EMA) of `Roc`

**`UpperBand`** _`double`_ - Upper band of ROC (overbought indicator)

**`LowerBand`** _`double`_ - Lower band of ROC (oversold indicator)

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
    .GetRocWb(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetRocWb(..)
    .GetEma(..);
```
