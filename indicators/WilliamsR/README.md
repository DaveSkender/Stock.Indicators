# Williams %R

 [Williams %R](https://en.wikipedia.org/wiki/Williams_%25R) is a stochastic oscillator that looks back `N` periods to produce an indicator with scale of -100 to 0.  It is exactly the same as the Fast variant of [Stochastic Oscillator](../Stochastic/README.md), but with a different scaling.

![image](chart.png)

```csharp
// usage
IEnumerable<WilliamsResult> results = Indicator.GetWilliamsR(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 0.  Default is 14.

## Response

```csharp
IEnumerable<WilliamsResult>
```

The first `N-1` periods will have `null` Oscillator values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### WilliamsResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `WilliamsR` | decimal | Oscillator over prior `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate WilliamsR(14)
IEnumerable<WilliamsResult> results = Indicator.GetWilliamsR(history,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
WilliamsResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("Williams %R on {0} was {1}", result.Date, result.WilliamsR);
```

```bash
Williams %R on 12/31/2018 was -52.0
```
