---
title: Quote parts and basic transforms
description: Basic quote transforms (e.g. HL2, OHL3, etc.) and isolation of individual price quote candle parts from a full OHLCV quote.
permalink: /indicators/quotepart/
redirect-from:
  - /indicators/Use/
  - /indicators/BasicQuote/
type: price-transform
layout: indicator
---

# {{ page.title }}

Returns a reusable (chainable) basic quote transform (e.g. HL2, OHL3, etc.) by isolating a single component part value or calculated value from the full OHLCV quote candle parts.

```csharp
// C# usage syntax
IReadOnlyList<QuotePart> results =
  quotes.Use(candlePart);

// alternate syntax
IReadOnlyList<QuotePart> results =
  quotes.ToQuotePart(candlePart);
```

## Parameters

**`candlePart`** _`CandlePart`_ - The [OHLCV]({{site.baseurl}}/guide/#historical-quotes) element or simple price transform.

### Historical quotes requirements

You must have at least 1 period of `quotes`.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

{% include candlepart-options.md %}

## Response

```csharp
IReadOnlyList<QuotePart>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.

### `QuotePart` type

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`Value`** _`double`_ - Price of `CandlePart` option

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

Results can be further processed on `Value` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .Use(CandlePart.OHLC4)
    .ToRsi(..);
```

This indicator must be generated from `quotes` and **cannot** be generated from results of another chain-enabled indicator or method.

## Streaming

Subscribe to a `QuoteHub` for streaming scenarios:

```csharp
QuoteHub quoteHub = new();
QuotePartHub observer = quoteHub.ToQuotePartHub(CandlePart.HL2);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<QuotePart> results = observer.Results;
```

## Buffering

Use a `QuotePartList` for incremental buffering scenarios:

```csharp
QuotePartList buffer = new(CandlePart.Close);

foreach (IQuote quote in quotes)  // simulating stream
{
  buffer.Add(quote);
}

IReadOnlyList<QuotePart> results = buffer;
```
