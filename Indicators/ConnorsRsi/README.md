# Connors ConnorsRsi

Relative Strength Index measures strength of winning/losing streak over `N` lookback periods on a scale of 0 to 100.  ConnorsRSI values over 95 are considered overbought, while values under 5 are considered oversold.
[More info ...](https://www.qmatix.com/ConnorsRSI-Pullbacks-Guidebook.pdf)

```csharp
// usage
IEnumerable<ConnorsRsiResult> results = Indicator.GetConnorsRsi(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.  Since this uses a smoothing technique, we recommend you use at least 250 data points prior to the intended usage date for maximum precision.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1.

## Response

```csharp
IEnumerable<ConnorsRsiResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### ConnorsRsiResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `ConnorsRsi` | float | ConnorsRsi over prior `N` lookback periods
| `IsIncreasing` | bool | Direction since last period (e.g. up or down).  Persists for no change.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate ConnorsRsi(14)
IEnumerable<ConnorsRsiResult> results = Indicator.GetConnorsRsi(history,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
ConnorsRsiResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("ConnorsRsi on {0} was {1}", result.Date, result.ConnorsRsi);
```

```bash
ConnorsRsi on 12/31/2018 was 42.08
```
