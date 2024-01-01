---
title: Parabolic SAR
description: Created by J. Welles Wilder, Parabolic SAR (stop and reverse) is a price-time based indicator used to determine trend direction and reversals.  It can be used to identify trend direction, reversals, and stop-loss signals.
permalink: /indicators/ParabolicSar/
image: /assets/charts/ParabolicSar.png
type: stop-and-reverse
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, [Parabolic SAR](https://en.wikipedia.org/wiki/Parabolic_SAR) (stop and reverse) is a price-time based indicator used to determine trend direction and reversals.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/245 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax (standard)
IEnumerable<ParabolicSarResult> results =
  quotes.GetParabolicSar(accelerationStep, maxAccelerationFactor);

// alternate usage with custom initial Factor
IEnumerable<ParabolicSarResult> results =
  quotes.GetParabolicSar(accelerationStep, maxAccelerationFactor, initialFactor);
```

## Parameters

**`accelerationStep`** _`double`_ - Incremental step size for the Acceleration Factor.  Must be greater than 0.  Default is 0.02

**`maxAccelerationFactor`** _`double`_ - Maximum factor limit.  Must be greater than `accelerationStep`.  Default is 0.2

**`initialFactor`** _`double`_ - Optional.  Initial Acceleration Factor.  Must be greater than 0 and not larger than `maxAccelerationFactor`.  Default is `accelerationStep`.

### Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, we recommend at least 100 data points.  Initial Parabolic SAR values prior to the first reversal are not accurate and are excluded from the results.  Therefore, provide sufficient quotes to capture prior trend reversals, before your intended usage period.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ParabolicSarResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first trend will have `null` values since it is not accurate and based on an initial guess.

### ParabolicSarResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Sar`** _`double`_ - Stop and Reverse value

**`IsReversal`** _`bool`_ - Indicates a trend reversal

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Sar` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetParabolicSar(..)
    .GetEma(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
