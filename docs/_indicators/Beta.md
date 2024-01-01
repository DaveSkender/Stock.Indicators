---
title: Beta Coefficient
description: Beta Coefficient with Beta+/Beta- shows how strongly one asset's price responds to systemic volatility of the entire market.  Upside Beta (Beta+) and Downside Beta (Beta-),  popularized by Harry M. Markowitz, are also included.
permalink: /indicators/Beta/
image: /assets/charts/Beta.png
type: numerical-analysis
layout: indicator
---

# {{ page.title }}

[Beta](https://en.wikipedia.org/wiki/Beta_(finance)) shows how strongly one asset's price responds to systemic volatility of the entire market.  [Upside Beta](https://en.wikipedia.org/wiki/Upside_beta) (Beta+) and [Downside Beta](https://en.wikipedia.org/wiki/Downside_beta) (Beta-), [popularized by Harry M. Markowitz](https://www.jstor.org/stable/j.ctt1bh4c8h), are also included.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/268 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<BetaResult> results = quotesEval
  .GetBeta(quotesMarket, lookbackPeriods, type);
```

## Parameters

**`quotesMarket`** _`IEnumerable<TQuote>`_ - [Historical quotes]({{site.baseurl}}/guide/#historical-quotes) market data should be at any consistent frequency (day, hour, minute, etc).  This `market` quotes will be used to establish the baseline.

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback window.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size and especially when using Beta +/-.

**`type`** _`BetaType`_ -  Type of Beta to calculate.  Default is `BetaType.Standard`. See [BetaType options](#betatype-options) below.

### Historical quotes requirements

You must have at least `N` periods of `quotesEval` to cover the warmup periods.  You must have at least the same matching date elements of `quotesMarket`.  An `InvalidQuotesException` will be thrown if not matched.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

#### BetaType options

**`Standard`** - Standard Beta only.  Uses all historical quotes.

**`Up`** - Upside Beta only.  Uses historical quotes from market up bars only.

**`Down`** - Downside Beta only.  Uses historical quotes from market down bars only.

**`All`** - Returns all of the above.  Use this option if you want `Ratio` and `Convexity` values returned.  Note: 3× slower to calculate.

> &#128161; **Pro tip**
>
> Financial institutions often depict a single number for Beta on their sites.  To get that same long-term Beta value, use 5 years of monthly bars for `quotes` and a value of 60 for `lookbackPeriods`.  If you only have smaller bars, use the [Aggregate()]({{site.baseurl}}/utilities#resize-quote-history) utility to convert it.
>
> [Alpha](https://en.wikipedia.org/wiki/Alpha_(finance)) is calculated as `R – Rf – Beta (Rm - Rf)`, where `Rf` is the risk-free rate.

## Response

```csharp
IEnumerable<BetaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### BetaResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Beta`** _`double`_ - Beta coefficient based

**`BetaUp`** _`double`_ - Beta+ (Up Beta)

**`BetaDown`** _`double`_ - Beta- (Down Beta)

**`Ratio`** _`double`_ - Beta ratio is `BetaUp/BetaDown`

**`Convexity`** _`double`_ - Beta convexity is <code>(BetaUp-BetaDown)<sup>2</sup></code>

**`ReturnsEval`** _`double`_ - Returns of evaluated quotes (`R`)

**`ReturnsMrkt`** _`double`_ - Returns of market quotes (`Rm`)

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
var results = quotesEval
    .Use(CandlePart.HL2)
    .GetBeta(quotesMarket.Use(CandlePart.HL2), ..);
```

> &#128681; **Warning!** Both eval and market arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.

Results can be further processed on `Beta` with additional chain-enabled indicators.

```csharp
// example
var results = quotesEval
    .GetBeta(quotesMarket, ..)
    .GetSlope(..);
```
