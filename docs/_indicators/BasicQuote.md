---
title: Basic quote transforms
description: Basic quote transforms (e.g. HL2, OHL3, etc.) and isolation of individual price quote candle parts from a full OHLCV quote.
permalink: /indicators/BasicQuote/
type: price-transform
layout: indicator
---

# {{ page.title }}

Returns a basic quote transform (e.g. HL2, OHL3, etc.) and isolation of individual price quote candle parts from a full OHLCV quote.

```csharp
// C# usage syntax
IEnumerable<BasicData> results =
  quotes.GetBaseQuote(candlePart);
```

## Parameters

**`candlePart`** _`CandlePart`_ - The [OHLCV]({{site.baseurl}}/guide/#historical-quotes) element or simple price transform.  See [CandlePart options](#candlepart-options) below.

### Historical quotes requirements

You must have at least 1 period of `quotes`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

{% include candlepart-options.md %}

## Response

```csharp
IEnumerable<BasicData>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.

### BasicData

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`Value`** _`double`_ - Price of `CandlePart` option

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Value` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetBaseQuote(CandlePart.OHLC4)
    .GetRsi(..);

// and is equivalent to
var results = quotes
    .Use(CandlePart.OHLC4)
    .GetRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.
