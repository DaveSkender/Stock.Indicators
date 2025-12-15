---
title: ROC with Bands
description: Rate of Change with Bands, created by Vitali Apirine, is a volatility banded variant of the basic Rate of Change (ROC) indicator.
---

# {{ $frontmatter.title }}

Rate of Change (ROC) with Bands, created by Vitali Apirine, is a volatility banded variant of [Rate of Change (ROC)](/indicators/Roc).
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/242 "Community discussion about this indicator")

<img src="/assets/charts/RocWb.png" alt="chart for ROC with Bands" />

```csharp
// C# usage syntax
IReadOnlyList<RocWbResult> results =
  quotes.ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
```

## Parameters

**`lookbackPeriods`** _`int`_ - Number of periods (`N`) to go back.  Must be greater than 0.  Typical values range from 10-20.

**`emaPeriods`** _`int`_ - Number of periods for the ROC EMA line.  Must be greater than 0.  Standard is 3.

**`stdDevPeriods`** _`int`_ - Number of periods the standard deviation for upper/lower band lines.  Must be greater than 0 and not more than `lookbackPeriods`.  Standard is to use same value as `lookbackPeriods`.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes` to cover the warmup periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<RocWbResult>
```

### RocWbResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Roc`** _`double`_ - Rate of Change over `N` lookback periods (%, not decimal)

**`RocEma`** _`double`_ - Exponential moving average (EMA) of `Roc`

**`UpperBand`** _`double`_ - Upper band of ROC (overbought indicator)

**`LowerBand`** _`double`_ - Lower band of ROC (oversold indicator)

### Utilities

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Streaming

This indicator can be used with the buffer style for incremental streaming scenarios.  See [Streaming guide](/guide) for more information.

```csharp
// buffer-style streaming
RocWbList buffer = new(lookbackPeriods, emaPeriods, stdDevPeriods);

foreach (Quote quote in quotes)
{
    buffer.Add(quote);
    RocWbResult result = buffer[^1];
}

// or initialize with historical quotes
RocWbList buffer = quotes.ToRocWbList(lookbackPeriods, emaPeriods, stdDevPeriods);
```

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToRocWb(..);
```

Results can be further processed on `Roc` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToRocWb(..)
    .ToEma(..);
```
