---
title: Rate of Change (ROC)
description: Rate of Change, also known as Momentum Oscillator, is the percent change of price over a lookback window.  Momentum is the raw price change equivalent.
permalink: /indicators/Roc/
image: /assets/charts/Roc.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

[Rate of Change](https://en.wikipedia.org/wiki/Momentum_(technical_analysis)), also known as Momentum Oscillator, is the percent change of price over a lookback window.  Momentum is the raw price change equivalent.  A [Rate of Change with Bands]({{site.baseurl}}/indicators/RocWb/#content) variant, created by Vitali Apirine, is also available.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/242 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// usage
IEnumerable<RocResult> results =
  quotes.GetRoc(lookbackPeriods);

// usage with optional SMA of ROC (shown above)
IEnumerable<RocResult> results =
  quotes.GetRoc(lookbackPeriods, smaPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to go back.  Must be greater than 0.

**`smaPeriods`** _`int`_ - Optional.  Number of periods in the moving average of ROC.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<RocResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values for ROC since there's not enough data to calculate.

### RocResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Momentum`** _`double`_ - Raw change in price over `N` periods

**`Roc`** _`double`_ - Percent change in price (%, not decimal)

**`RocSma`** _`double`_ - Moving average (SMA) of ROC based on `smaPeriods` periods, if specified

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
    .GetRoc(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetRoc(..)
    .GetEma(..);
```
