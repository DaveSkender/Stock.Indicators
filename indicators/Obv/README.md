# On-Balance Volume (OBV)

Popularized by Joseph Granville, [On-balance Volume](https://en.wikipedia.org/wiki/On-balance_volume) is a rolling accumulation of volume based on Close price direction.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/246 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<ObvResult> results = Indicator.GetObv(history);

// usage with optional overlay SMA of OBV (shown above)
IEnumerable<ObvResult> results = Indicator.GetObv(history, smaPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `smaPeriod` | int | Optional.  Number of periods (`N`) in the moving average of OBV.  Must be greater than 0, if specified.

### Minimum history requirements

You must supply at least two historical quotes; however, since this is a trendline, more is recommended.

## Response

```csharp
IEnumerable<ObvResult>
```

The first period OBV will have `0` value since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### ObvResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Obv` | decimal | On-balance Volume
| `ObvSma` | decimal | Moving average (SMA) of OBV based on `smaPeriod` periods, if specified

:warning: **Warning**: absolute values in OBV are somewhat meaningless, so use with caution.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<ObvResult> results = Indicator.GetObv(history);

// use results as needed
ObvResult result = results.LastOrDefault();
Console.WriteLine("OBV on {0} was {1}", result.Date, result.Obv);
```

```bash
OBV on 12/31/2018 was 539843504
```
