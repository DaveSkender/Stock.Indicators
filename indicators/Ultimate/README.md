﻿# Ultimate Oscillator

Created by Larry Williams, the [Ultimate Oscillator](https://en.wikipedia.org/wiki/Ultimate_oscillator) uses several lookback periods to weigh buying power against true range price to produce on oversold / overbought oscillator.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/231 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<UltimateResult> results =
  quotes.GetUltimate(shortPeriods, middlePeriods, longPeriods);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `shortPeriods` | int | Number of periods (`S`) in the short lookback.  Must be greater than 0.  Default is 7.
| `middlePeriods` | int | Number of periods (`M`) in the middle lookback.  Must be greater than `S`.  Default is 14.
| `longPeriods` | int | Number of periods (`L`) in the long lookback.  Must be greater than `M`.  Default is 28.

### Historical quotes requirements

You must have at least `L+1` periods of `quotes`.

`quotes` is an `IEnumerable<TQuote>` collection of historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](../../docs/GUIDE.md#historical-quotes) for more information.

## Response

```csharp
IEnumerable<UltimateResult>
```

The first `L-1` periods will have `null` Ultimate values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### UltimateResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Ultimate` | decimal | Simple moving average for `N` lookback periods

### Utilities

- [.Find(lookupDate)](../../docs/UTILITIES.md#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](../../docs/UTILITIES.md#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](../../docs/UTILITIES.md#remove-warmup-periods)

See [Utilities and Helpers](../../docs/UTILITIES.md#content) for more information.

## Example

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 20-period Ultimate
IEnumerable<UltimateResult> results = quotes.GetUltimate(7,14,28);

// use results as needed
UltimateResult result = results.LastOrDefault();
Console.WriteLine("ULT on {0} was {1}", result.Date, result.Ultimate);
```

```bash
ULT on 12/31/2018 was 49.53
```
