# On-Balance Volume

A rolling accumulation of volume based on Close price direction.  [More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:on_balance_volume_obv)

```csharp
// usage
IEnumerable<ObvResult> results = Indicator.GetObv(history);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least two historical quotes; however, since this is a trendline, more is recommended.

## Response

```csharp
IEnumerable<ObvResult>
```

The first period OBV will have `0` value since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### ObvResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `Obv` | long | On-balance Volume

**Warning**: absolute values in OBV are somewhat meaningless, so use with caution.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<ObvResult> results = Indicator.GetObv(history);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
ObvResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("OBV on {0} was {1}", result.Date, result.Obv);
```

```bash
OBV on 12/31/2018 was 539843504
```
