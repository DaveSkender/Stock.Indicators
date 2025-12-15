---
title: Average Directional Index (ADX)
description: Created by J. Welles Wilder, the Directional Movement Index (DMI) and Average Directional Movement Index (ADX) is a measure of price directional movement.  It includes upward and downward indicators, and is often used to measure strength of trend.
permalink: /indicators/Adx/
image: /assets/charts/AdIndex.png
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by J. Welles Wilder, the Directional Movement Index (DMI) and [Average Directional Movement Index](https://en.wikipedia.org/wiki/Average_directional_movement_index) (ADX) is a measure of price directional movement.  It includes upward and downward indicators, and is often used to measure strength of trend.
[[Discuss] &#128172;]({{site.github.repository_url}}/discussions/270 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// C# usage syntax
IEnumerable<AdxResult> results =
  quotes.GetAdx(lookbackPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to consider.  Must be greater than 1.  Default is 14.

### Historical quotes requirements

You must have at least `2×N+100` periods of `quotes` to cover the [warmup and convergence]({{site.github.repository_url}}/discussions/688) periods.  We generally recommend you use at least `2×N+250` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AdxResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `2×N-1` periods will have `null` values for `Adx` since there's not enough data to calculate.

>&#9886; **Convergence warning**: The first `2×N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AdxResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Pdi`** _`double`_ - Plus Directional Index (+DI) - Measures upward price movement

**`Mdi`** _`double`_ - Minus Directional Index (-DI) - Measures downward price movement

**`Dmi`** _`double`_ - Directional Movement Index (DMI) - Also known as the Directional Index (DX), this is the normalized difference between +DI and -DI, calculated as `100 × |+DI - -DI| / (+DI + -DI)`

**`Adx`** _`double`_ - Average Directional Index (ADX) - A smoothed moving average of the DMI that measures the strength of the trend (regardless of direction)

**`Adxr`** _`double`_ - Average Directional Index Rating (ADXR) - A smoothed version of ADX

>&#128161; **About DMI and ADX**: The relationship between these indicators:
>
>- **+DI and -DI** indicate the _direction_ of price movement (upward vs. downward)
>- **DMI (DX)** measures the _separation_ between +DI and -DI, indicating how directional the movement is
>- **ADX** is a smoothed average of DMI that measures the _strength_ of the trend, regardless of whether it's up or down
>
>Traders often use +DI/-DI crossovers to identify potential entry points, while ADX acts as a filter to confirm trend strength. An ADX above 25 typically indicates a strong trend, while below 20 suggests a weak or range-bound market.

### Utilities

- [.Condense()]({{site.baseurl}}/utilities#condense)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Adx` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAdx(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
