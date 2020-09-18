# Volume Simple Moving Average

The Volume Simple Moving Average is the average volume over `N` lookback periods.  This is helpful when you are trying to assess whether volume is above or below average.

```csharp
// usage
IEnumerable<VolSmaResult> results = Indicator.GetVolSma(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.

## Response

```csharp
IEnumerable<VolSmaResult>
```

The first `N-1` periods will have `null` values for `VolSma` since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### VolSmaResult

The result set is a modified version of the `Quote` class.

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume
| `VolSma` | decimal | Simple moving average of `Volume` for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA of Volume
IEnumerable<VolSmaResult> results = Indicator.GetVolSma(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
VolSmaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("Average Volume on {0} was ${1}", result.Date, result.VolSma);
```

```bash
Average Volume on 12/31/2018 was 163695200
```
