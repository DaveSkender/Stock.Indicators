---
title: Williams Alligator
description: Created by Bill Williams, Alligator is a depiction of three smoothed moving averages of median price, showing chart patterns that compared to an alligator's feeding habits when describing market movement. The moving averages are known as the Jaw, Teeth, and Lips, which are calculated using lookback and offset periods.  The related Gator Oscillator depicts periods of eating and resting.
permalink: /indicators/Alligator/
image: /assets/charts/Alligator.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Bill Williams, Alligator is a depiction of three smoothed moving averages of median price, showing chart patterns that compared to an alligator's feeding habits when describing market movement. The moving averages are known as the Jaw, Teeth, and Lips, which are calculated using lookback and offset periods.  See also the [Gator Oscillator]({{site.baseurl}}/indicators/Gator/#content).
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/385 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// usage
IEnumerable<AlligatorResult> results =
  quotes.GetAlligator(jawPeriods,jawOffset,teethPeriods,teethOffset,lipsPeriods,lipsOffset);
```

## Parameters

**`jawPeriods`** _`int`_ - Number of periods (`JP`) for the Jaw moving average.  Must be greater than `teethPeriods`.  Default is 13.

**`jawOffset`** _`int`_ - Number of periods (`JO`) for the Jaw offset.  Must be greater than 0.  Default is 8.

**`teethPeriods`** _`int`_ - Number of periods (`TP`) for the Teeth moving average.  Must be greater than `lipsPeriods`.  Default is 8.

**`teethOffset`** _`int`_ - Number of periods (`TO`) for the Teeth offset.  Must be greater than 0.  Default is 5.

**`lipsPeriods`** _`int`_ - Number of periods (`LP`) for the Lips moving average.  Must be greater than 0.  Default is 5.

**`lipsOffset`** _`int`_ - Number of periods (`LO`) for the Lips offset.  Must be greater than 0.  Default is 3.

### Historical quotes requirements

You must have at least `JP+JO+100` periods of `quotes` to cover the convergence periods. Since this uses a smoothing technique, we recommend you use at least `JP+JO+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AlligatorResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `JP+JO` periods will have `null` values since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `JP+JO+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AlligatorResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Jaw`** _`double`_ - Alligator's Jaw

**`Teeth`** _`double`_ - Alligator's Teeth

**`Lips`** _`double`_ - Alligator's Lips

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
    .Use(CandlePart.HL2)
    .GetAlligator();
```

Results **cannot** be further chained with additional transforms.
