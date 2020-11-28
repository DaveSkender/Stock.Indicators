# Average Directional Index (ADX)

[Average Directional Movement Index](https://en.wikipedia.org/wiki/Average_directional_movement_index) is a measure of price directional movement.  It includes upward and downward indicators, and is often used to measure strength of trend.

![image](chart.png)

```csharp
// usage
IEnumerable<AdxResult> results = Indicator.GetAdx(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `2×N+100` periods of `history` to allow for smoothing convergence.  We generally recommend you use at least 250 data points prior to the intended usage date for maximum precision.
| `lookbackPeriod` | int | Number of periods (`N`) to consider.  Must be greater than 1.  Default is 14.

## Response

```csharp
IEnumerable<AdxResult>
```

The first `2×N-1` periods will have `null` values for ADX since there's not enough data to calculate.  The first `2×N+100` values will be less precise due to smoothing convergence.  We always return the same number of elements as there are in the historical quotes.

### AdxResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Pdi` | decimal | Plus Directional Index (+DI) for `N` lookback periods
| `Mdi` | decimal | Minus Directional Index (-DI) for `N` lookback periods
| `Adx` | decimal | Average Directional Index (ADX) for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate 14-period ADX
IEnumerable<AdxResult> results = Indicator.GetAdx(history,14);

// use results as needed
AdxResult result = results.LastOrDefault();
Console.WriteLine("ADX on {0} was {1}", result.Date, result.Adx);
```

```bash
ADX on 12/31/2018 was 34.30
```
