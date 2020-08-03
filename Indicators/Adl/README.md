# Accumulation Distribution Line (ADL)

A rolling accumulation of Chaikin Money Flow Volume.  [More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:accumulation_distribution_line)

```csharp
// usage
IEnumerable<AdlResult> results = Indicator.GetAdl(history);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least two historical quotes; however, since this is a trendline, more is recommended.

## Response

```csharp
IEnumerable<AdlResult>
```

We always return the same number of elements as there are in the historical quotes.

### AdlResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `MoneyFlowMultiplier` | decimal | Money Flow Multiplier
| `MoneyFlowVolume` | decimal | Money Flow Volume
| `Adl` | decimal | Accumulation Distribution Line (ADL)

**Warning**: absolute values in ADL and MFV are somewhat meaningless, so use with caution.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<AdlResult> results = Indicator.GetAdl(history);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
AdlResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("ADL on {0} was {1}", result.Date, result.Adl);
```

```bash
ADL on 12/31/2018 was 3439986548
```
