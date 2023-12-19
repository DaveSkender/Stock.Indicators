---
title: Money Flow Index (MFI)
description: Created by Quong and Soudack, the Money Flow Index is a price-volume oscillator that shows buying and selling momentum.
permalink: /indicators/Mfi/
image: /assets/charts/Mfi.png
type: volume-based
layout: indicator
---

# {{ page.title }}

Created by Quong and Soudack, the [Money Flow Index](https://en.wikipedia.org/wiki/Money_flow_index) is a price-volume oscillator that shows buying and selling momentum.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/247 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<MfiResult> results =
  quotes.GetMfi(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the lookback period.  Must be greater than 1. Default is 14.

### Historical quotes requirements

You must have at least `N+1` historical quotes to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<MfiResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` MFI values since they cannot be calculated.

### MfiResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Mfi`** _`decimal`_ - Money Flow Index

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Mfi` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetMfi(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
