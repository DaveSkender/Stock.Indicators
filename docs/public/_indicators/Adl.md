---
title: Accumulation / Distribution Line (ADL)
description: Created by Marc Chaikin, the Accumulation / Distribution Line is a rolling accumulation of Chaikin Money Flow Volume.  It can be a leading momentum indicator for financial market price movements.
permalink: /indicators/Adl/
image: /assets/charts/Adl.png
type: volume-based
layout: indicator
---

# Accumulation / Distribution Line (ADL)

Created by Marc Chaikin, the [Accumulation/Distribution Line/Index](https://en.wikipedia.org/wiki/Accumulation/distribution_index) is a rolling accumulation of Chaikin Money Flow Volume.
[[Discuss] 💬](https://github.com/DaveSkender/Stock.Indicators/discussions/271 "Community discussion about this indicator")

![chart for Accumulation / Distribution Line (ADL)](/assets/charts/Adl.png)

```csharp
// C# usage syntax
IReadOnlyList<AdlResult> results =
  quotes.GetAdl();
```

## Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, since this is a trendline, more is recommended.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide/#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<AdlResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.

### AdlResult

**`Timestamp`** _`DateTime`_ - Date from evaluated `TQuote`

**`MoneyFlowMultiplier`** _`double`_ - Money Flow Multiplier

**`MoneyFlowVolume`** _`double`_ - Money Flow Volume

**`Adl`** _`double`_ - Accumulation Distribution Line (ADL)

> &#128681; **Warning**: absolute values in ADL and MFV are somewhat meaningless.  Use with caution.

### Utilities

- [.Condense()](/utilities#condense)
- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Adl` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetAdl()
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
