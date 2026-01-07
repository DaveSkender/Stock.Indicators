---
title: Beta Coefficient
description: Beta Coefficient with Beta+/Beta- shows how strongly one asset's price responds to systemic volatility of the entire market.  Upside Beta (Beta+) and Downside Beta (Beta-),  popularized by Harry M. Markowitz, are also included.
---

# Beta Coefficient

[Beta](https://en.wikipedia.org/wiki/Beta_(finance)) shows how strongly one asset's price responds to systemic volatility of the entire market.  [Upside Beta](https://en.wikipedia.org/wiki/Upside_beta) (Beta+) and [Downside Beta](https://en.wikipedia.org/wiki/Downside_beta) (Beta-), [popularized by Harry M. Markowitz](https://www.jstor.org/stable/j.ctt1bh4c8h), are also included.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/268 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Beta.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<BetaResult> results = quotesEval
  .ToBeta(quotesMarket, lookbackPeriods, type);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `quotesMarket` | IReadOnlyList\<TQuote\> | [Historical quotes](/guide#historical-quotes) market data should be at any consistent frequency (day, hour, minute, etc).  This `market` quotes will be used to establish the baseline. |
| `lookbackPeriods` | int | Number of periods (`N`) in the lookback window.  Must be greater than 0 to calculate; however we suggest a larger period for statistically appropriate sample size and especially when using Beta +/-. |
| `type` | BetaType | Type of Beta to calculate.  Default is `BetaType.Standard`. See [BetaType options](#betatype-options) below. |

### Historical quotes requirements

You must have at least `N` periods of `quotesEval` to cover the warmup periods.  You must have at least the same matching date elements of `quotesMarket`.  An `InvalidQuotesException` will be thrown if not matched.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

#### BetaType options

**`Standard`** - Standard Beta only.  Uses all historical quotes.

**`Up`** - Upside Beta only.  Uses historical quotes from market up bars only.

**`Down`** - Downside Beta only.  Uses historical quotes from market down bars only.

**`All`** - Returns all of the above.  Use this option if you want `Ratio` and `Convexity` values returned.  Note: 3× slower to calculate.

::: tip 💡 Pro tip
Financial institutions often depict a single number for Beta on their sites.  To get that same long-term Beta value, use 5 years of monthly bars for `quotes` and a value of 60 for `lookbackPeriods`.  If you only have smaller bars, use the [Aggregate()](/utilities/quotes#resize-quote-history) utility to convert it.

[Alpha](https://en.wikipedia.org/wiki/Alpha_(finance)) is calculated as `R – Rf – Beta (Rm - Rf)`, where `Rf` is the risk-free rate.
:::

## Response

```csharp
IReadOnlyList<BetaResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### `BetaResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Beta` | double | Beta coefficient based |
| `BetaUp` | double | Beta+ (Up Beta) |
| `BetaDown` | double | Beta- (Down Beta) |
| `Ratio` | double | Beta ratio is `BetaUp/BetaDown` |
| `Convexity` | double | Beta convexity is <code>(BetaUp-BetaDown)<sup>2</sup></code> |
| `ReturnsEval` | double | Returns of evaluated quotes (`R`) |
| `ReturnsMrkt` | double | Returns of market quotes (`Rm`) |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities/results#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotesEval
    .Use(CandlePart.HL2)
    .ToBeta(quotesMarket.Use(CandlePart.HL2), ..);
```

::: warning
Both eval and market arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.
:::

Results can be further processed on `Beta` with additional chain-enabled indicators.

```csharp
// example
var results = quotesEval
    .ToBeta(quotesMarket, ..)
    .ToSlope(..);
```
