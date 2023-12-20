---
title: Gator Oscillator
description: Created by Bill Williams, the Gator Oscillator is an expanded oscillator view of Williams Alligator's three moving averages.
permalink: /indicators/Gator/
image: /assets/charts/Gator.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Bill Williams, the Gator Oscillator is an expanded oscillator view of [Williams Alligator]({{site.baseurl}}/indicators/Alligator/#content)'s three moving averages.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/385 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<GatorResult> results =
  quotes.GetGator();

// with custom Alligator configuration
IEnumerable<GatorResult> results = quotes
  .GetAlligator([see Alligator docs])
  .GetGator();
```

## Historical quotes requirements

If using default settings, you must have at least 121 periods of `quotes`. Since this uses a smoothing technique, we recommend you use at least 271 data points prior to the intended usage date for better precision.  If using a custom Alligator configuration, see [Alligator documentation]({{site.baseurl}}/indicators/Alligator/#historical-quotes-requirements) for historical quotes requirements.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<GatorResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first 10-20 periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first 150 periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### GatorResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Upper`** _`double`_ - Absolute value of Alligator `Jaw-Teeth`

**`Lower`** _`double`_ - Absolute value of Alligator `Lips-Teeth`

**`UpperIsExpanding`** _`bool`_ - Upper value is growing

**`LowerIsExpanding`** _`bool`_ - Lower value is growing

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
var results = quotes
    .Use(CandlePart.HLC3)
    .GetGator();
```

Results **cannot** be further chained with additional transforms.
