---
title: Marubozu (Preview)
permalink: /indicators/Marubozu/
layout: indicator
type: candlestick-pattern
---

# {{ page.title }}

[Marubozu](https://en.wikipedia.org/wiki/Marubozu) is a candlestick pattern that has no wicks, representing consistent directional movement.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/512 "Community discussion about this indicator")

  <img src="{{site.baseurl}}/assets/charts/Marubozu.png" alt="drawing" height="150" />

```csharp
// usage
IEnumerable<MarubozuResult> results =
  quotes.GetMarubozu(minBodyPercent);
```

## Parameters

| name | type | notes
| -- |-- |--
| `minBodyPercent` | double | Optional.  Minimum body size as a decimalized percent of total candle size.  Must be between 0.8 and 1, if specified.  Default is 0.95 (95%).

### Historical quotes requirements

You must have at least one historical quote.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<MarubozuResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The candlestick pattern is indicated on dates when `Marubozu` has a non-null value.

### MarubozuResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Marubozu` | decimal | Indicates a Marubozu candle `Close` price; otherwise `null`
| `IsBullish` | bool | Direction of the candle

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<MarubozuResult> results = quotes.GetMarubozu();
```
