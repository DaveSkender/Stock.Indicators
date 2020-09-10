# Price Momentum Oscillator (PMO)

DecisionPoint Price Momentum Oscillator is double-smoothed ROC based momentum indicator.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:dppmo)

```csharp
// usage
IEnumerable<PmoResult> results = Indicator.GetPmo(history, timePeriod, smoothingPeriod, signalPeriod);
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `T+S` periods of `history`.
| `timePeriod` | int | Number of periods (`T`) for ROC EMA smoothing.  Must be greater than 1.  Default is 35.
| `smoothingPeriod` | int | Number of periods (`S`) for PMO EMA smoothing.  Must be greater than 0.  Default is 20.
| `signalPeriod` | int | Number of periods (`G`) for Signal line EMA.  Must be greater than 0.  Default is 10.

## Response

```csharp
IEnumerable<PmoResult>
```

The first `T+S-1` periods will have `null` values for PMO since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.  Since this uses multiple smoothing operations, we recommend you use at least `T+S+G+250` data points prior to the intended usage date for maximum precision.

### PmoResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Pmo` | decimal | Price Momentum Oscillator
| `Signal` | decimal | Signal line is EMA of PMO

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate 20-period PMO
IEnumerable<PmoResult> results = Indicator.GetPmo(history,35,20,10);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
PmoResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("PMO on {0} was {1}", result.Date, result.Pmo);
```

```bash
PMO on 12/31/2018 was -2.70
```
