---
title: Williams Alligator
permalink: /indicators/Alligator/
type: price-trend
layout: indicator
---

# {{ page.title }}

Created by Bill Williams, Alligator is a depiction of three smoothed moving averages of median price, showing chart patterns that compared to an alligator's feeding habits when describing market movement. The moving averages are known as the Jaw, Teeth, and Lips, which are calculated using specific lookback and offset periods.  See also the [Gator Oscillator](../Gator#content).
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/385 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Alligator.png)

```csharp
// usage
IEnumerable<AlligatorResult> results =
  quotes.GetAlligator();
```

## Historical quotes requirements

You must have at least 115 periods of `quotes`. Since this uses a smoothing technique, we recommend you use at least 265 data points prior to the intended usage date for better precision.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### Internal parameters

This indicator uses fixed interal parameters for the three moving averages of median price `(H+L)/2`.

| SMMA | Lookback | Offset
| -- |-- |--
| Jaw | 13 | 8
| Teeth | 8 | 5
| Lips | 5 | 3

## Response

```csharp
IEnumerable<AlligatorResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first 10-20 periods will have `null` values since there's not enough data to calculate.

:hourglass: **Convergence Warning**: The first 150 periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AlligatorResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Jaw` | decimal | Alligator's Jaw
| `Teeth` | decimal | Alligator's Teeth
| `Lips` | decimal | Alligator's Lips

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate the Williams Alligator
IEnumerable<AlligatorResult> results = quotes.GetAlligator();
```
