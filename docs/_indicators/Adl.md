---
title: Accumulation/Distribution Line (ADL)
permalink: /indicators/Adl/
type: volume-based
layout: indicator
---

# {{ page.title }}

Created by Marc Chaikin, the [Accumulation/Distribution Line/Index](https://en.wikipedia.org/wiki/Accumulation/distribution_index) is a rolling accumulation of Chaikin Money Flow Volume.
[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/271 "Community discussion about this indicator")

![image]({{site.baseurl}}/assets/charts/Adl.png)

```csharp
// usage
IEnumerable<AdlResult> results =
  quotes.GetAdl();  

// usage with optional overlay SMA of ADL (shown above)
IEnumerable<AdlResult> results =
  quotes.GetAdl(smaPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `smaPeriods` | int | Optional.  Number of periods (`N`) in the moving average of ADL.  Must be greater than 0, if specified.

### Historical quotes requirements

You must have at least two historical quotes; however, since this is a trendline, more is recommended.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<AdlResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.

### AdlResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `MoneyFlowMultiplier` | decimal | Money Flow Multiplier
| `MoneyFlowVolume` | decimal | Money Flow Volume
| `Adl` | decimal | Accumulation Distribution Line (ADL)
| `AdlSma` | decimal | Moving average (SMA) of ADL based on `smaPeriods` periods, if specified

:warning: **Warning**: absolute values in ADL and MFV are somewhat meaningless, so use with caution.

### Utilities

- [.ConvertToQuotes()]({{site.baseurl}}/utilities#convert-to-quotes)
- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and Helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<AdlResult> results = quotes.GetAdl();
```
