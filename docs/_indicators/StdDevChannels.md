---
title: Standard Deviation Channels
description: Standard Deviation Channels are price ranges based on an linear regression centerline and standard deviations band widths.
permalink: /indicators/StdDevChannels/
image: /assets/charts/StdDevChannels.png
type: price-channel
layout: indicator
---

# {{ page.title }}

Standard Deviation Channels are prices ranges based on an linear regression centerline and standard deviations band widths.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/368 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<StdDevChannelsResult> results =
  quotes.GetStdDevChannels(lookbackPeriods, stdDeviations);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Size (`N`) of the evaluation window.  Must be `null` or greater than 1 to calculate.  A `null` value will produce a full `quotes` evaluation window ([see below](#alternative-depiction-for-full-quotes-variant)).  Default is 20.

**`stdDeviations`** _`double`_ - Width of bands.  Standard deviations (`D`) from the regression line.  Must be greater than 0.  Default is 2.

### Historical quotes requirements

You must have at least `N` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<StdDevChannelsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- Up to `N-1` periods will have `null` values since there's not enough data to calculate.

> &#128073; **Repaint warning**: Historical results are a function of the current period window position and will fluctuate over time.  Recommended for visualization; not recommended for backtesting.

### StdDevChannelsResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Centerline`** _`double`_ - Linear regression line (center line)

**`UpperChannel`** _`double`_ - Upper line is `D` standard deviations above the center line

**`LowerChannel`** _`double`_ - Lower line is `D` standard deviations below the center line

**`BreakPoint`** _`bool`_ - Helper information.  Indicates first point in new window.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Alternative depiction for full quotes variant

If you specify `null` for the `lookbackPeriods`, you will get a regression line over the entire provided `quotes`.

![image]({{site.baseurl}}/assets/charts/StdDevChannelsFull.png)

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotesEval
    .Use(CandlePart.HL2)
    .GetStdDevChannels(..);
```

Results **cannot** be further chained with additional transforms.
