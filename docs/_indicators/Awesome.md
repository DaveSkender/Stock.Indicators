---
title: Awesome Oscillator (AO)
description: Created by Bill Williams, the Awesome Oscillator (AO), also known as Super AO, is a measure of the gap between a fast and slow period modified moving average.
permalink: /indicators/Awesome/
image: /assets/charts/Awesome.png
type: oscillator
layout: indicator
---

# {{ page.title }}

Created by Bill Williams, the Awesome Oscillator (aka Super AO) is a measure of the gap between a fast and slow period modified moving average.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/282 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<AwesomeResult> results =
  quotes.GetAwesome(fastPeriods, slowPeriods);
```

## Parameters

**`fastPeriods`** _`int`_ - Number of periods (`F`) for the faster moving average.  Must be greater than 0.  Default is 5.

**`slowPeriods`** _`int`_ - Number of periods (`S`) for the slower moving average.  Must be greater than `fastPeriods`.  Default is 34.

### Historical quotes requirements

You must have at least `S` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AwesomeResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period `S-1` periods will have `null` values since there's not enough data to calculate.

### AwesomeResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Oscillator`** _`double`_ - Awesome Oscillator

**`Normalized`** _`double`_ - `100 × Oscillator ÷ (median price)`

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
    .GetAwesome(..);
```

Results can be further processed on `Oscillator` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAwesome(..)
    .GetRsi(..);
```
