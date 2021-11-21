---
title: Parabolic SAR
permalink: /indicators/ParabolicSar/
type: stop-and-reverse
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, [Parabolic SAR](https://en.wikipedia.org/wiki/Parabolic_SAR) (stop and reverse) is a price-time based indicator used to determine trend direction and reversals.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/245 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/ParabolicSar.png)

```csharp
// usage (standard)
IEnumerable<ParabolicSarResult> results =
  quotes.GetParabolicSar(accelerationStep, maxAccelerationFactor);  

// alternate usage with custom initial Factor
IEnumerable<ParabolicSarResult> results =
  quotes.GetParabolicSar(accelerationStep, maxAccelerationFactor, initialFactor);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `accelerationStep` | decimal | Incremental step size for the Acceleration Factor.  Must be greater than 0.  Default is 0.02
| `maxAccelerationFactor` | decimal | Maximimum factor limit.  Must be greater than `accelerationStep`.  Default is 0.2
| `initialFactor` | decimal | Optional.  Initial Acceleration Factor.  Must be greater than 0.  Default is `accelerationStep`.

### Historical quotes requirements

At least two quotes records are required to calculate; however, we recommend at least 100 data points.  Initial Parabolic SAR values prior to the first reversal are not accurate and are excluded from the results.  Therefore, provide sufficient quotes to capture prior trend reversals, before your intended usage period.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<ParabolicSarResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first trend will have `null` values since it is not accurate and based on an initial guess.

### ParabolicSarResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Sar` | decimal | Stop and Reverse value
| `IsReversal` | bool | Indicates a trend reversal

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate ParabolicSar(0.02,0.2)
IEnumerable<ParabolicSarResult> results
  = quotes.GetParabolicSar(0.02,0.2);
```
