# Kaufman's Adaptive Moving Average (KAMA)

Created by Perry Kaufman, [KAMA](https://school.stockcharts.com/doku.php?id=technical_indicators:kaufman_s_adaptive_moving_average) is an volatility adaptive moving average of Close price over configurable lookback periods.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/210 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<KamaResult> results =
  Indicator.GetKama(history, erPeriod, fastPeriod, slowPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `erPeriod` | int | Number of Efficiency Ratio (volatility) periods (`E`).  Must be greater than 0.  Default is 10.
| `fastPeriod` | int | Number of Fast EMA periods.  Must be greater than 0.  Default is 2.
| `slowPeriod` | int | Number of Slow EMA periods.  Must be greater than `fastPeriod`.  Default is 30.

### Minimum history requirements

You must supply at least `6×E` or `E+100` periods of `history`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `10×E` data points prior to the intended usage date for better precision.

## Response

```csharp
IEnumerable<KamaResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

:warning: **Warning**: The first `10×E` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### KamaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Kama` | decimal | Kaufman's adaptive moving average

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate KAMA(10,2,30)
IEnumerable<KamaResult> results = Indicator.GetKama(history,10,2,30);

// use results as needed
KamaResult result = results.LastOrDefault();
Console.WriteLine("KAMA on {0} was ${1}", result.Date, result.Kama);
```

```bash
KAMA on 12/31/2018 was $251.86
```
