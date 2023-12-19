---
title: Donchian Channels
description: Created by Richard Donchian, Donchian Channels, also called Price Channels, are price ranges derived from highest High and lowest Low values.

permalink: /indicators/Donchian/
image: /assets/charts/Donchian.png
type: price-channel
layout: indicator
---

# {{ page.title }}

Created by Richard Donchian, [Donchian Channels](https://en.wikipedia.org/wiki/Donchian_channel), also called Price Channels, are price ranges derived from highest High and lowest Low values.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/257 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<DonchianResult> results =
  quotes.GetDonchian(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) for lookback period.  Must be greater than 0 to calculate; however we suggest a larger value for an appropriate sample size.  Default is 20.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<DonchianResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` values since there's not enough data to calculate.

### DonchianResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`UpperBand`** _`decimal`_ - Upper line is the highest High over `N` periods

**`Centerline`** _`decimal`_ - Simple average of Upper and Lower bands

**`LowerBand`** _`decimal`_ - Lower line is the lowest Low over `N` periods

**`Width`** _`decimal`_ - Width as percent of Centerline price.  `(UpperBand-LowerBand)/Centerline`

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

This indicator is not chain-enabled and must be generated from `quotes`.  It **cannot** be used for further processing by other chain-enabled indicators.
