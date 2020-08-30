# Heikin-Ashi

A modified candlestick pattern that uses prior day for smoothing.  [More info ...](https://school.stockcharts.com/doku.php?id=chart_analysis:heikin_ashi)

```csharp
// usage
IEnumerable<HeikinAshiResult> results = Indicator.GetHeikinAshi(history);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least two historical quotes.

## Response

```csharp
IEnumerable<HeikinAshiResult>
```

The first period will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### HeikinAshiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Modified open price
| `High` | decimal | Modified high price
| `Low` | decimal | Modified low price
| `Close` | decimal | Modified close price
| `IsBullish` | bool | Indication of bar direction.  Persists for no change.
| `Weakness` | decimal | Size of directional shadow, weakness of signal (no shadow is strong)

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate
IEnumerable<HeikinAshiResult> results = Indicator.GetHeikinAshi(history);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
HeikinAshiResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("Heikin-Ashi open price on {0} was ${1}", result.Date, result.Open);
```

```bash
Heikin-Ashi open price on 12/31/2018 was $241.3
```
