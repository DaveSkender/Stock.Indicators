# MESA Adaptive Moving Average (MAMA) [:speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/211 "Community discussion about this indicator")

Created by John Ehlers, the [MAMA](http://mesasoftware.com/papers/MAMA.pdf) indicator is a 5-period adaptive moving average of Close price.

![image](chart.png)

```csharp
// usage
IEnumerable<MamaResult> results = Indicator.GetMama(history,fastLimit,slowLimit);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least 6 periods of `history`.
| `fastLimit` | double | Fast limit threshold.  Must be greater than `slowLimit` and less than 1.  Default is 0.5.
| `slowLimit` | double | Slow limit threshold.  Must be greater than 0.  Default is 0.05.

## Response

```csharp
IEnumerable<MamaResult>
```

The first 4 periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### MamaResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Mama` | decimal | MESA adaptive moving average
| `Fama` | decimal | Following adaptive moving average (FAMA)

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate Mama(0.5,0.05)
IEnumerable<MamaResult> results = Indicator.GetMama(history,0.5,0.05);

// use results as needed
MamaResult result = results.LastOrDefault();
Console.WriteLine("MAMA on {0} was ${1}", result.Date, result.Mama);
```

```bash
MAMA on 12/31/2018 was $251.86
```
