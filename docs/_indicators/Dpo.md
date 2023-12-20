---
title: Detrended Price Oscillator (DPO)
description: Detrended Price Oscillator depicts the difference between price and an offset simple moving average.  It is used to identify trend cycles and duration.
permalink: /indicators/Dpo/
image: /assets/charts/Dpo.png
type: oscillator
layout: indicator
---

# {{ page.title }}

[Detrended Price Oscillator](https://en.wikipedia.org/wiki/Detrended_price_oscillator) depicts the difference between price and an offset simple moving average.  It is used to identify trend cycles and duration.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/551 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<DpoResult> results =
  quotes.GetDpo(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

### Historical quotes requirements

You must have at least `N` historical quotes to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<DpoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N/2-2` and last `N/2+1` periods will be `null` since they cannot be calculated.

### DpoResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Sma`** _`double`_ - Simple moving average offset by `N/2+1` periods

**`Dpo`** _`double`_ - Detrended Price Oscillator (DPO)

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .GetDpo(..);
```

Results can be further processed on `Dpo` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetDpo(..)
    .GetRsi(..);
```
