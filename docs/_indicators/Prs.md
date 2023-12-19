---
title: Price Relative Strength (PRS)
description: Price Relative Strength, also called Comparative Relative Strength, shows the ratio of two quote histories, based on price.  It is often used to compare against a market index or sector ETF.  When using the optional lookback window, this also returns relative percent change over the specified periods.  This is not the same as the more prevalent Relative Strength Index (RSI).
permalink: /indicators/Prs/
image: /assets/charts/Prs.png
type: price-characteristic
layout: indicator
---

# {{ page.title }}

[Price Relative Strength (PRS)](https://en.wikipedia.org/wiki/Relative_strength), also called Comparative Relative Strength, shows the ratio of two quote histories, based on price.  It is often used to compare against a market index or sector ETF.  When using the optional `lookbackPeriods`, this also returns relative percent change over the specified periods.  This is not the same as the more prevalent <a href="{{site.baseurl}}/indicators/Rsi/#content" rel="nofollow">Relative Strength Index (RSI)</a>.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/243 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<PrsResult> results =
  quotesEval.GetPrs(quotesBase);

// usage with optional lookback period and SMA of PRS (shown above)
IEnumerable<PrsResult> results =
  quotesEval.GetPrs(quotesBase, lookbackPeriods, smaPeriods);
```

## Parameters

**`quotesBase`** _`IEnumerable<TQuote>`_ - [Historical quotes]({{site.baseurl}}/guide/#historical-quotes) used as the basis for comparison.  This is usually market index data.  You must have the same number of periods as `quotesEval`.

**`lookbackPeriods`** _`int`_ - Optional.  Number of periods (`N`) to lookback to compute % difference.  Must be greater than 0 if specified or `null`.

**`smaPeriods`** _`int`_ - Optional.  Number of periods (`S`) in the SMA lookback period for `Prs`.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` periods of `quotesEval` to calculate `PrsPercent` if `lookbackPeriods` is specified; otherwise, you must specify at least `S+1` periods.  More than the minimum is typically specified.  For this indicator, the elements must match (e.g. the `n`th elements must be the same date).  An `Exception` will be thrown for mismatch dates.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

`quotesEval` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<PrsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The `N` periods will have `null` values for `PrsPercent` and the first `S-1` periods will have `null` values for `Sma` since there's not enough data to calculate.

### PrResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Prs`** _`double`_ - Price Relative Strength compares `Eval` to `Base` histories

**`PrsSma`** _`double`_ - Moving Average (SMA) of PRS over `S` periods

**`PrsPercent`** _`double`_ - Percent change difference between `Eval` and `Base` over `N` periods

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotesEval
    .Use(CandlePart.HL2)
    .GetPrs(quotesBase, ..);
```

> &#128681; **Warning!** Both `quotesEval` and `quotesBase` arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.

Results can be further processed on `Beta` with additional chain-enabled indicators.

```csharp
// example
var results = quotesEval
    .GetPrs(quotesBase, ..)
    .GetSlope(..);
```
