---
title: Heikin-Ashi
description: Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern that transforms prices based on prior period prices for smoothing.
permalink: /indicators/HeikinAshi/
image: /assets/charts/HeikinAshi.png
type: price-transform
layout: indicator
---

# {{ page.title }}

Created by Munehisa Homma, [Heikin-Ashi](https://en.wikipedia.org/wiki/Candlestick_chart#Heikin-Ashi_candlesticks) is a modified candlestick pattern based on prior period prices for smoothing.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/254 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<HeikinAshiResult> results =
  quotes.GetHeikinAshi();
```

## Historical quotes requirements

You must have at least two periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<HeikinAshiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period will have `null` values since there's not enough data to calculate.
- `HeikinAshiResult` is based on `IQuote`, so it can be used as a direct replacement for `quotes`.

### HeikinAshiResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Open`** _`decimal`_ - Modified open price

**`High`** _`decimal`_ - Modified high price

**`Low`** _`decimal`_ - Modified low price

**`Close`** _`decimal`_ - Modified close price

**`Volume`** _`decimal`_ - Volume (same as `quotes`)

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)
- .ToQuotes() to convert to a `Quote` collection.  Example:

  ```csharp
  IEnumerable<Quote> results = quotes
    .GetHeikinAshi()
    .ToQuotes();
  ```

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results are based in `IQuote` and can be further used in any indicator.

```csharp
// example
var results = quotes
    .GetHeikinAshi(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
