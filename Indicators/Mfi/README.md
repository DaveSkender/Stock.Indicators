# Money Flow Index (MFI)

A price-volume oscillator that shows buying and selling momentum.  [More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:money_flow_index_mfi)

```csharp
// usage
IEnumerable<MfiResult> results = Indicator.GetMfi(history,lookbackPeriod);
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N+1` historical quotes.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period.  Must be greater than 1. Default is 14.

## Response

```csharp
IEnumerable<MfiResult>
```

The first `N` periods will have `null` MFI values since they cannot be calculated.  We always return the same number of elements as there are in the historical quotes.

### MfiResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Mfi` | decimal | Money Flow Index

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<MfiResult> results = Indicator.GetMfi(history,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
MfiResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("MFI on {0} was {1}", result.Date, result.Mfi);
```

```bash
MFI on 12/31/2018 was 39.95
```
