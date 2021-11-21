---
title: Chandelier Exit
permalink: /indicators/Chandelier/
type: stop-and-reverse
layout: indicator
---

# {{ page.title }}

Created by Charles Le Beau, the [Chandelier Exit](https://school.stockcharts.com/doku.php?id=technical_indicators:chandelier_exit) is typically used for stop-loss and can be computed for both long or short types.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/263 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Chandelier.png)

```csharp
// usage
IEnumerable<ChandelierResult> results =
  quotes.GetChandelier(lookbackPeriods, multiplier, type);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `lookbackPeriods` | int | Number of periods (`N`) for the lookback evaluation.  Default is 22.
| `multiplier` | decimal | Multiplier number must be a positive value.  Default is 3.
| `type` | ChandelierType | Direction of exit.  See [ChandelierType options](#chandeliertype-options) below.  Default is `ChandelierType.Long`.

### Historical quotes requirements

You must have at least `N+1` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

### ChandelierType options

| type | description
|-- |--
| `ChandelierType.Long` | Intended as stop loss value for long positions. (default)
| `ChandelierType.Short` | Intended as stop loss value for short positions.

## Response

```csharp
IEnumerable<ChandelierResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N` periods will have `null` Chandelier values since there's not enough data to calculate.

### ChandelierResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `ChandelierExit` | decimal | Exit line

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate Chandelier(22,3,LONG)
IEnumerable<ChandelierResult> results =
  quotes.GetChandelier(22,3,ChandelierType.Long);
```
