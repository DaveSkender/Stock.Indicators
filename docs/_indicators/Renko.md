---
title: Renko Chart
description: The Renko Chart is a Japanese price transformed candlestick pattern that uses "bricks" to show a defined increment of change over a non-linear time series.  Transitions can use either Close or High/Low price values.  An Average True Range (ATR) variant is also provided where brick size is determined by current Average True Range values.
permalink: /indicators/Renko/
image: /assets/charts/Renko.png
type: price-transform
layout: indicator
---

# {{ page.title }}

The [Renko Chart](https://en.m.wikipedia.org/wiki/Renko_chart) is a Japanese price transformed candlestick pattern that uses "bricks" to show a defined increment of change over a non-linear time series.  Transitions can use either `Close` or `High/Low` price values.  An [ATR variant](#atr-variant) is also provided where brick size is determined by current Average True Range values.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/478 "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// usage
IEnumerable<RenkoResult> results =
  quotes.GetRenko(brickSize, endType);
```

## Parameters

**`brickSize`** _`decimal`_ - Brick size.  Must be greater than 0.

**`endType`** _`EndType`_ - See options below.  Default is `EndType.Close`

### Historical quotes requirements

You must have at least two periods of `quotes` to cover the warmup periods; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### EndType options

**`EndType.Close`** - Brick change threshold measured from `Close` price (default)

**`EndType.HighLow`** - Brick change threshold measured from `High` and `Low` price

## Chaining

Results are based in `IQuote` and can be further used in any indicator.

```csharp
// example
var results = quotes
    .GetRenko(..)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Response

```csharp
IEnumerable<RenkoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It does not return a single incremental indicator value.
- `RenkoResult` is based on `IQuote`, so it can be used as a direct replacement for `quotes`.

> :warning: **Warning**: Unlike most indicators in this library, this indicator DOES NOT return the same number of elements as there are in the historical quotes.  Renko bricks are added to the results once the `brickSize` change is achieved.  For example, if it takes 3 days for a $2.50 price change to occur an entry is made on the third day while the first two are skipped.  If a period change occurs at multiples of `brickSize`, multiple bricks are drawn with the same `Date`.  See [online documentation](https://www.investopedia.com/terms/r/renkochart.asp) for more information.

### RenkoResult

Each result record represents one Renko brick.

**`Date`** _`DateTime`_ - Formation date of brick(s)

**`Open`** _`decimal`_ - Brick open price

**`High`** _`decimal`_ - Highest high during elapsed quotes periods

**`Low`** _`decimal`_ - Lowest low during elapsed quotes periods

**`Close`** _`decimal`_ - Brick close price

**`Volume`** _`decimal`_ - Sum of Volume over elapsed quotes periods

**`IsUp`** _`bool`_ - Direction of brick (true=up,false=down)

> :warning: **Warning**: When multiple bricks are drawn from a single `quote` period, the extra information about `High` and `Low` wicks and `Volume` is potentially confusing to interpret.  `High` and `Low` wicks will be the same across the multiple bricks; and `Volume` is portioning evenly across the number of bricks.  For example, if within one `quote` period 3 bricks are drawn, the `Volume` for each brick will be `(sum of quotes Volume since last brick) / 3`.

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## ATR Variant

```csharp
// usage
IEnumerable<RenkoResult> results =
  quotes.GetRenkoAtr(atrPeriods, endType);
```

### Parameters for ATR

**`atrPeriod`** _`int`_ - Number of lookback periods (`A`) for ATR evaluation.  Must be greater than 0.

**`endType`** _`EndType`_ - See options below.  Default is `EndType.Close`

#### Historical quotes requirements for ATR

You must have at least `A+100` periods of `quotes`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response for ATR

```csharp
IEnumerable<RenkoResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.

> :paintbrush: **Repaint warning**: When using the `GetRenkoAtr()` variant, the last [Average True Range (ATR)]({{site.baseurl}}/indicators/Atr/#content) value is used to set `brickSize`.  Since the ATR changes over time, historical bricks will be repainted as new periods are added or updated in `quotes`.
