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
IReadOnlyList<PrsResult> results =
  quotesEval.ToPrs(quotesBase);
```

## Parameters

**`quotesBase`** _`IReadOnlyList<TQuote>`_ - [Historical quotes]({{site.baseurl}}/guide/#historical-quotes) used as the basis for comparison.  This is usually market index data.  You must have the same number of periods as `quotesEval`.

**`lookbackPeriods`** _`int`_ - Optional.  Number of periods (`N`) to lookback to compute % difference.  Must be greater than 0 if specified or `null`.

### Historical quotes requirements

You must have at least `N` periods of `quotesEval` to calculate `PrsPercent` if `lookbackPeriods` is specified; otherwise, you must specify at least `S+1` periods.  More than the minimum is typically specified.  For this indicator, the elements must match (e.g. the `n`th elements must be the same date).  An `Exception` will be thrown for mismatch dates.  Historical price quotes should have a consistent frequency (day, hour, minute, etc).

`quotesEval` is an `IReadOnlyList<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<PrsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The `N` periods will have `null` values for `PrsPercent` since there's not enough data to calculate.

### PrsResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Prs`** _`double`_ - Price Relative Strength compares `Eval` to `Base` histories

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
    .ToPrs(quotesBase, ..);
```

> &#128681; **Warning!** Both `quotesEval` and `quotesBase` arguments must contain the same number of elements and be the results of a chainable indicator or `.Use()` method.

Results can be further processed on `Beta` with additional chain-enabled indicators.

```csharp
// example
var results = quotesEval
    .ToPrs(quotesBase, ..)
    .ToSlope(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations for Price Relative Strength:

```csharp
PrsList prsList = new(lookbackPeriods);

// simulating dual-stream inputs
for (int i = 0; i < quotesEval.Count; i++)
{
  prsList.Add(quotesEval[i], quotesBase[i]);
}

// based on `ICollection<PrsResult>`
IReadOnlyList<PrsResult> results = prsList;
```

For reusable values from chainable indicators:

```csharp
IReadOnlyList<IReusable> evalValues = quotesEval.Use(CandlePart.Close);
IReadOnlyList<IReusable> baseValues = quotesBase.Use(CandlePart.Close);

PrsList prsList = new(lookbackPeriods);

// incremental addition
for (int i = 0; i < evalValues.Count; i++)
{
  prsList.Add(evalValues[i], baseValues[i]);
}

IReadOnlyList<PrsResult> results = prsList;
```

Subscribe to dual `QuoteHub` providers for advanced streaming scenarios:

```csharp
QuoteHub<Quote> quoteHubEval = new();
QuoteHub<Quote> quoteHubBase = new();

// Add quotes to both hubs
quoteHubEval.Add(quotesEval);
quoteHubBase.Add(quotesBase);

// Create PRS hub from two providers
PrsHub prsHub = quoteHubEval.ToPrsHub(quoteHubBase, lookbackPeriods);

IReadOnlyList<PrsResult> results = prsHub.Results;
```

> &#9432; **Note**: Price Relative Strength requires synchronized dual inputs (evaluated asset and base/market). Both input series must have matching timestamps and element counts.
