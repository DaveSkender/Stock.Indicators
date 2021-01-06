# Volume Weighted Average Price (VWAP)

The [Volume Weighted Average Price](https://en.wikipedia.org/wiki/Volume-weighted_average_price) is a Volume weighted average of Close price, typically used on intraday data.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/310 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<VwapResult> results = Indicator.GetVwap(history);

// usage with optional anchored start date
IEnumerable<VwapResult> results = Indicator.GetVwap(history, startDate);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `startDate` | DateTime | Optional.  The anchor date used to start the VWAP accumulation.  The earliest date in `history` is used when not provided.

### Minimum history requirements

You must supply at least one historical quote to calculate; however, more is often needed to be useful.  History is typically provided for a single day using minute-based intraday periods.  Since this is an accumulated weighted average price, different start dates will produce different results.  The accumulation starts at the first period in the provided `history`, unless it is specified in the optional `startDate` parameter.

## Response

```csharp
IEnumerable<VwapResult>
```

The first period or the `startDate` will have a `Vwap = Close` value since it is the initial starting point.  `Vwap` values before `startDate`, if specified, will be `null`.  We always return the same number of elements as there are in the historical quotes.

### VwapResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Vwap` | decimal | Volume Weighted Average Price

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<VwapResult> results = Indicator.GetVwap(history);

// use results as needed
VwapResult result = results.LastOrDefault();
Console.WriteLine("VWAP on {0} was ${1}", result.Date, result.Vwap);
```

```bash
VWAP on 12/15/2020 16:00:00 was $368.18
```
