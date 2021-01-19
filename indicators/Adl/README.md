# Accumulation/Distribution Line (ADL)

Created by Marc Chaikin, the [Accumulation/Distribution Line/Index](https://en.wikipedia.org/wiki/Accumulation/distribution_index) is a rolling accumulation of Chaikin Money Flow Volume.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/271 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<AdlResult> results = Indicator.GetAdl(history);  

// usage with optional overlay SMA of ADL (shown above)
IEnumerable<AdlResult> results = Indicator.GetAdl(history, smaPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `smaPeriod` | int | Optional.  Number of periods (`N`) in the moving average of ADL.  Must be greater than 0, if specified.

### Minimum history requirements

You must supply at least two historical quotes; however, since this is a trendline, more is recommended.

## Response

```csharp
IEnumerable<AdlResult>
```

We always return the same number of elements as there are in the historical quotes.

### AdlResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `MoneyFlowMultiplier` | decimal | Money Flow Multiplier
| `MoneyFlowVolume` | decimal | Money Flow Volume
| `Adl` | decimal | Accumulation Distribution Line (ADL)
| `AdlSma` | decimal | Moving average (SMA) of ADL based on `smaPeriod` periods, if specified

:warning: **Warning**: absolute values in ADL and MFV are somewhat meaningless, so use with caution.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate
IEnumerable<AdlResult> results = Indicator.GetAdl(history);

// use results as needed
AdlResult result = results.LastOrDefault();
Console.WriteLine("ADL on {0} was {1}", result.Date, result.Adl);
```

```bash
ADL on 12/31/2018 was 3439986548
```
