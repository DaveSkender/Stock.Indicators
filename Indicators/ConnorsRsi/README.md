# ConnorsRSI

A composite oscillator that incorporates RSI, winning/losing streaks, and percentile gain metrics on scale of 0 to 100.
[More info](https://alvarezquanttrading.com/wp-content/uploads/2016/05/ConnorsRSIGuidebook.pdf) and [analysis](https://alvarezquanttrading.com/blog/connorsrsi-analysis).

```csharp
// usage
IEnumerable<ConnorsRsiResult> results = Indicator.GetConnorsRsi(history, rsiPeriod, streakPeriod, rankPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).
| `rsiPeriod` | int | Lookback period (`R`) for the close price RSI.  Must be greater than 1.  Default is 3.
| `streakPeriod` | int | Lookback period (`S`) for the streak RSI.  Must be greater than 1.  Default is 2.
| `rankPeriod` | int | Lookback period (`P`) for the Percentile Rank.  Must be greater than 1.  Default is 100.

### History requirements

`N` is the greater of `R` and `S`, and `P`.  You must supply at least `N+2` periods of `history`.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for maximum precision.

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
| `RsiClose` | decimal | RSI(`R`) of the Close price.
| `RsiStreak` | decimal | RSI(`S`) of the Streak.
| `PercentRank` | decimal | Percentile rank of the period gain value.
| `ConnorsRsi` | decimal | ConnorsRSI

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate ConnorsRsi(3,2.100)
IEnumerable<ConnorsRsiResult> results = Indicator.GetConnorsRsi(history,3,2,100);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
ConnorsRsiResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("ConnorsRSI on {0} was {1}", result.Date, result.ConnorsRsi);
```

```bash
ConnorsRSI on 12/31/2018 was 74.77
```
