# Commodity Channel Index (CCI)

[Commodity Channel Index](https://en.wikipedia.org/wiki/Commodity_channel_index) is an oscillator depicting deviation from typical price range, often used to identify cyclical trends.

![image](chart.png)

```csharp
// usage
IEnumerable<CciResult> results = Indicator.GetCci(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N+1` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the moving average.  Must be greater than 0.  Default is 20.

## Response

```csharp
IEnumerable<CciResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### CciResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Cci` | decimal | CCI value for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate 20-period CCI
IEnumerable<CciResult> results = Indicator.GetCci(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
CciResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("CCI on {0} was ${1}", result.Date, result.Cci);
```

```bash
CCI on 12/31/2018 was -52.99
```
