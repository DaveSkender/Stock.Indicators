# PivotPoints

[Pivot Points](https://en.wikipedia.org/wiki/Pivot_point_(technical_analysis)) depict support and resistance levels of Close price, based on the prior lookback periods.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/274 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<PivotPointResult> results = Indicator.GetPivotPoints(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#quote)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `windowSize` | PeriodSize | Size of the lookback window.
| `pointType` | PivotPointType | Type of Pivot Point.  Default is `Standard`.

### Minimum history requirements

You must supply at least `2` windows of `history`.  For example, if you specify a `Weekly` window size, you need at least 14 days of `history`.  However, to ensure you have a full lookback period, we recommend you always use _more than_ `2` windows of history.

### PeriodSize options (for windowSize)

| type | description
|-- |--
| `PeriodSize.Month` | Use the prior month's data to calculate current month's Pivot Points
| `PeriodSize.Week` | [..] weekly
| `PeriodSize.Day` | [..] daily.  Commonly used for intraday data.
| `PeriodSize.Hour` | [..] hourly

### PivotPointType options

| type | description
|-- |--
| `PivotPointType.Standard` | Standard "floor trading" Pivot Points
| `PivotPointType.Camarilla` | Camarilla
| `PivotPointType.Demark` | Demark
| `PivotPointType.Fibonacci` | Fibonacci
| `PivotPointType.Woodie` | Woodie

## Response

```csharp
IEnumerable<PivotPointsResult>
```

The first `N` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### PivotPointResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `R3` | decimal | Resistance level 3
| `R2` | decimal | Resistance level 2
| `R1` | decimal | Resistance level 1
| `PP` | decimal | Pivot Point
| `S1` | decimal | Support level 1
| `S2` | decimal | Support level 2
| `S3` | decimal | Support level 3

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate standard 20-period Pivot Points
IEnumerable<PivotPointResult> results = Indicator.GetPivotPoints(history,20,PivotPointType.Woodie);

// use results as needed
PivotPointsResult result = results.LastOrDefault();
Console.WriteLine("P on {0} was ${1}", result.Date, result.P);
```

```bash
P on 12/31/2018 was $251.86
```
