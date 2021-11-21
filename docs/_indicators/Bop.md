---
title: Balance of Power (BOP)
description: Balance of Power (BOP) / Balance of Market Power
permalink: /indicators/Bop/
type: price-characteristic
layout: indicator
---

# {{ page.title }}

Created by Igor Levshin, the [Balance of Power](https://school.stockcharts.com/doku.php?id=technical_indicators:balance_of_power) (aka Balance of Market Power) is a momentum oscillator that depicts the strength of buying and selling pressure.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/302 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Bop.png)

```csharp
// usage
IEnumerable<BopResult> results =
  quotes.GetBop(smoothPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `smoothPeriods` | int | Number of periods (`N`) for smoothing.  Must be greater than 0.  Default is 14.

### Historical quotes requirements

You must have at least `N` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<BopResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `N-1` periods will have `null` values since there's not enough data to calculate.

### BopResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Bop` | decimal | Balance of Power

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()]({{site.baseurl}}/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 14-period BOP
IEnumerable<BopResult> results = quotes.GetBop(14);
```
