---
title: Moving Average Envelopes
description:  Moving Average Envelopes is a price band channel overlay that is offset from the moving average of price.
permalink: /indicators/MaEnvelopes/
image: /assets/charts/MaEnvelopes.png
type: price-channel
layout: indicator
---

# {{ page.title }}

[Moving Average Envelopes](https://en.wikipedia.org/wiki/Moving_average_envelope) is a price band channel overlay that is offset from the moving average of price.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/288 "Community discussion about this indicator")

![chart for {{page.title}}]({{page.image}})

```csharp
// C# usage syntax
IReadOnlyList<MaEnvelopeResult> results =
  quotes.GetMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) in the moving average.  Must be greater than 1.

**`percentOffset`** _`double`_ - Percent offset for envelope width.  Example: 3.5% would be entered as 3.5 (not 0.035).  Must be greater than 0.  Typical values range from 2 to 10.  Default is 2.5.

**`movingAverageType`** _`MaType`_ - Type of moving average (e.g. SMA, EMA, HMA).  See [MaType options](#matype-options) below.  Default is `MaType.SMA`.

### Historical quotes requirements

See links in the supported [MaType options](#matype-options) section below for details on the inherited requirements for `quotes` and `lookbackPeriods`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](pages/guide.md#historical-quotes) for more information.

### MaType options

These are the supported moving average types:

**`MaType.ALMA`** - [Arnaud Legoux Moving Average](/indicators/Alma)

**`MaType.DEMA`** - [Double Exponential Moving Average](/indicators/Dema)

**`MaType.EPMA`** - [Endpoint Moving Average](/indicators/Epma)

**`MaType.EMA`** - [Exponential Moving Average](/indicators/Ema)

**`MaType.HMA`** - [Hull Moving Average](/indicators/Hma)

**`MaType.SMA`** - [Simple Moving Average](/indicators/Sma) (default)

**`MaType.SMMA`** - [Smoothed Moving Average](/indicators/Smma)

**`MaType.TEMA`** - [Triple Exponential Moving Average](/indicators/Tema)

**`MaType.WMA`** - [Weighted Moving Average](/indicators/Wma)

> &#128681;  **Warning**: For ALMA, default values are used for `offset` and `sigma`.

## Response

```csharp
IReadOnlyList<MaEnvelopeResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first periods will have `null` values since there's not enough data to calculate; the quantity will vary based on the `movingAverageType` specified.

>&#9886; **Convergence warning**: Some moving average variants have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.  See links in the supported [MaType options](#matype-options) section above for more information.

### MaEnvelopeResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Centerline`** _`double`_ - Moving average

**`UpperEnvelope`** _`double`_ - Upper envelope band

**`LowerEnvelope`** _`double`_ - Lower envelope band

The moving average `Centerline` is based on the `movingAverageType` type specified.

### Utilities

- [.Condense()](pages/utilities.md#condense)
- [.Find(lookupDate)](pages/utilities.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](pages/utilities.md#remove-warmup-periods)

See [Utilities and helpers](pages/utilities.md#utilities-for-indicator-results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HLC3)
    .GetMaEnvelopes(..);
```

Results **cannot** be further chained with additional transforms.
