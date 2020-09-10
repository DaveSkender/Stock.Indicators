# William %R

William %R is a stochastic oscillator that looks back `N` periods to produce an indicator with scale of -100 to 0.  It is exactly the same as the Fast variant of [Stochastic Oscillator](../Stochastic/README.md), but with a different scaling.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:williams_r)

```csharp
// usage
IEnumerable<WilliamResult> results = Indicator.GetWilliamR(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0.  Default is 14.

## Response

```csharp
IEnumerable<WilliamResult>
```

The first `N-1` periods will have `null` Oscillator values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### WilliamResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `WilliamR` | decimal | Oscillator over prior `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate WilliamR(14)
IEnumerable<WilliamResult> results = Indicator.GetWilliamR(history,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
WilliamResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("William %R on {0} was {1}", result.Date, result.WilliamR);
```

```bash
William %R on 12/31/2018 was -52.0
```
