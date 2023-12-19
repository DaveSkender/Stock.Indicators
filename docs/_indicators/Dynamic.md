---
title: McGinley Dynamic
description: Created by John R. McGinley, the McGinley Dynamic is a more responsive variant of exponential moving average.
permalink: /indicators/Dynamic/
image: /assets/charts/Dynamic.png
type: moving-average
layout: indicator
---

# {{ page.title }}

Created by John R. McGinley, the [McGinley Dynamic](https://www.investopedia.com/terms/m/mcginley-dynamic.asp) is a more responsive variant of exponential moving average.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/866 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax (with Close price)
IEnumerable<DynamicResult> results =
  quotes.GetDynamic(lookbackPeriods, kFactor);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 0.

**`kFactor`** _`double`_ - Optional.  Range adjustment factor (`K`).  Must be greater than 0.  Default is 0.6

### Historical quotes requirements

You must have at least `2` periods of `quotes`, to cover the initialization periods.  Since this uses a smoothing technique, we recommend you use at least `4×N` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### Pro tips

> Use a `kFactor` value of `1` if you do not want to adjust the `N` value.
>
> McGinley suggests that using a `K` value of 60% (0.6) allows you to use a `N` equivalent to other moving averages.  For example, DYNAMIC(20,0.6) is comparable to EMA(20); conversely, DYNAMIC(20,1) uses the raw 1:1 `N` value and is not equivalent.

## Response

```csharp
IEnumerable<DynamicResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period will have a `null` value since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `4×N` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### DynamicResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Dynamic`** _`double`_ - McGinley Dynamic

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
    .GetDynamic(..);
```

Results can be further processed on `Dynamic` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetDynamic(..)
    .GetRsi(..);
```
