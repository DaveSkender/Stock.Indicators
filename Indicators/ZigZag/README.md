# Zig Zag (ZIGZAG)

Zig Zag is a price chart overlay that simplifies the up and down movements and transitions.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:zigzag)

```csharp
// usage
IEnumerable<ZigZagResult> results = Indicator.GetZigZag(history, percentChange);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least two periods to calculate, but notably more is needed to be useful.
| `percentChange` | decimal | Percent change required to establish a line endpoint.  Example: 3.5% would be entered as 3.5 (not 0.035).  Must be greater than 0.  Typical values range from 3 to 10.

## Response

```csharp
IEnumerable<ZigZagResult>
```

If you do not supply enough points to cover the percent change, there will be no Zig Zag lines.  We always return the same number of elements as there are in the historical quotes.

### ZigZagResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `ZigZag` | decimal | Simple moving average for `N` lookback periods

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 5% change ZIGZAG
IEnumerable<ZigZagResult> results = Indicator.GetZigZag(history,5);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
ZigZagResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("ZIGZAG on {0} was ${1}", result.Date, result.ZigZag);
```

```bash
ZIGZAG on 12/31/2018 was $251.86
```
