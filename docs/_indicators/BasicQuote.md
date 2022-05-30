---
title: Basic Quote Transforms
permalink: /indicators/BasicQuote/
type: price-transform
layout: indicator
---

# {{ page.title }}

Returns a basic quote transform.

```csharp
// usage
IEnumerable<BasicData> results =
  quotes.GetBaseQuote(candlePart);
```

## Parameters

| name | type | notes
| -- |-- |--
| `candlePart` | CandlePart | The [OHLCV]({{site.baseurl}}/guide/#historical-quotes) element or simple price transform.  See [CandlePart options](#candlepart-options) below.

### Historical quotes requirements

You must have at least 1 period of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

{% include candlepart-options.md %}

## Response

```csharp
IEnumerable<BasicData>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes when not chained from another indicator.
- It does not return a single incremental indicator value.

### BasicData

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Value` | double | Price of `CandlePart` option

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate HL2 price (average of high and low price)
IEnumerable<BasicData> results = quotes.GetBaseQuote(CandlePart.HL2);
```
