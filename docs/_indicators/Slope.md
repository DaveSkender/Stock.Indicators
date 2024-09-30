---
title: Slope and Linear Regression
description: Slope of the best fit line is determined by an ordinary least-squares simple linear regression on price.  It can be used to help identify trend strength and direction.  This indicator can be used to produce both a rolling slope value and a straight line through a specified lookback window.
permalink: /indicators/Slope/
image: /assets/charts/Linear.png
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Slope of the best fit line](https://school.stockcharts.com/doku.php?id=technical_indicators:slope) is determined by an [ordinary least-squares simple linear regression](https://en.wikipedia.org/wiki/Simple_linear_regression) on price.  It can be used to help identify trend strength and direction.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/241 "Community discussion about this indicator")

![image](/assets/charts/Linear.png)
![image](/assets/charts/Slope.png)

```csharp
// C# usage syntax
IEnumerable<SlopeResult> results =
  quotes.GetSlope(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for the linear regression.  Must be greater than 1.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](pages/guide.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<SlopeResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values for `Slope` since there's not enough data to calculate.
- `Line` values are only provided for the last `N` periods of your quote history

> &#128073; **Repaint warning**: the `Line` will be continuously repainted since it is based on the last quote and lookback period.

### SlopeResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Slope`** _`double`_ - Slope `m` of the best-fit line of price

**`Intercept`** _`double`_ - Y-Intercept `b` of the best-fit line

**`StdDev`** _`double`_ - Standard Deviation of price over `N` lookback periods

**`RSquared`** _`double`_ - R-Squared (R&sup2;), aka Coefficient of Determination

**`Line`** _`decimal`_ - Best-fit line `y` over the last `N` periods (i.e. `y=mx+b` using last period values)

### Utilities

- [.Condense()](pages/utilities.md#condense)
- [.Find(lookupDate)](pages/utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](pages/utilities.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](pages/utilities.md#remove-warmup-periods)

See [Utilities and helpers](pages/utilities.md#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .GetEma(..)
    .GetSlope(..);
```

Results can be further processed on `Slope` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetSlope(..)
    .GetRsi(..);
```
