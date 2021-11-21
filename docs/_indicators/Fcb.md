---
title: Fractal Chaos Bands (FCB)
permalink: /indicators/Fcb/
type: price-channel
layout: indicator
---

# {{ page.title }}

Created by Edward William Dreiss, Fractal Chaos Bands outline high and low price channels to depict broad less-chaotic price movements.  FCB is a channelized depiction of [Williams Fractal](../Fractal#content).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/347 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Fcb.png)

```csharp
// usage
IEnumerable<FcbResult> results =
  quotes.GetFcb(lookbackPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `windowSpan` | int | Fractal evaluation window span width (`S`).  Must be at least 2.  Default is 2.

The total evaluation window size is `2×S+1`, representing `±S` from the evalution date.  See [Williams Fractal](../Fractal#content) for more information about Fractals and `windowSpan`.

### Historical quotes requirements

You must have at least `2×S+1` periods of `quotes`; however, more is typically provided since this is a chartable candlestick pattern.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<FcbResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The periods before the first fractal are `null` since they cannot be calculated.

### FcbResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `UpperBand` | decimal | FCB upper band
| `LowerBand` | decimal | FCB lower band

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate Fcb(14)
IEnumerable<FcbResult> results = quotes.GetFcb(14);
```
