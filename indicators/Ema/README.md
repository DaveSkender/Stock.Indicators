# Exponential Moving Average (EMA) and Double EMA (DEMA)

Exponentially weighted moving average of the Close price over `N` periods.  More info on: [EMA](https://school.stockcharts.com/doku.php?id=technical_indicators:moving_averages) and [Double EMA](https://www.investopedia.com/terms/d/double-exponential-moving-average.asp)

```csharp
// usage for EMA (standard)
IEnumerable<EmaResult> results = Indicator.GetEma(history, lookbackPeriod);

// usage for Double EMA
IEnumerable<EmaResult> results = Indicator.GetDoubleEma(history, lookbackPeriod);
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

### Minimum history requirements

**EMA** (standard): You must supply at least 2×`N` or `N`+100 periods of `history`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least `N`+250 data points prior to the intended usage date for maximum precision.

**Double EMA**: You must supply at least 3×`N` or 2×`N`+100 periods of `history`, whichever is more.  Since this uses a smoothing technique, we recommend you use at least 2×`N`+250 data points prior to the intended usage date for maximum precision.

## Response

```csharp
IEnumerable<EmaResult>
```

We always return the same number of elements as there are in the historical quotes.

Standard EMA: The first `N-1` periods will have `null` values since there's not enough data to calculate.

Double EMA: The first `2×N-1` periods will have `null` values since there's not enough data to calculate.

### EmaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Ema` | decimal | Exponential moving average for `N` lookback period

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate 20-period EMA
IEnumerable<EmaResult> results = Indicator.GetEma(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
EmaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("EMA on {0} was ${1}", result.Date, result.Ema);
```

```bash
EMA on 12/31/2018 was $249.3519
```
