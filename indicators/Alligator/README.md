# Williams' Alligator

[Williams' Alligator](https://www.investopedia.com/articles/trading/072115/exploring-williams-alligator-indicator.asp) is an indicator that transposes multiple moving averages, showing chart patterns that Bill Williams compared to an alligator's feeding habits when describing market movement. 
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/385 "Community discussion about this indicator")

```csharp
// usage
IEnumerable<AlligatorResult> results = Indicator.GetAlligator(history);  // Traditional Alligator; Jaw (13, 8), Teeth (8, 5), and Lips (5, 3)

IEnumerable<AlligatorResult> results = Indicator.GetAlligator(history, 
    lookbackJaw, smoothingJaw, 
    lookbackTeeth, smoothingTeeth, 
    lookbackLips, smoothingLips);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `lookbackJaw` | int | Number of periods (`N`) in the moving average for the Alligator Jaw.  Must be greater than 0.
| `smoothingJaw` | int | Number of periods to smooth the moving average for the Alligator Jaw.  Must be less than `lookbackJaw`.
| `lookbackTeeth` | int | Number of periods (`N`) in the moving average for the Alligator Teeth.  Must be greater than 0.
| `smoothingTeeth` | int | Number of periods to smooth the moving average for the Alligator Teeth.  Must be less than `lookbackTeeth`.
| `lookbackLips` | int | Number of periods (`N`) in the moving average for the Alligator Lips.  Must be greater than 0.
| `smoothingLips` | int | Number of periods to smooth the moving average for the Alligator Lips.  Must be less than `lookbackLips`.


### Minimum history requirements

You must supply at least `2×N` or `N+100` periods of `history` for the largest lookback period (traditionally `lookbackJaw`), whichever is more.  Since this uses a smoothing technique, we recommend you use at least `N+250` data points prior to the intended usage date for better precision.

## Response

```csharp
IEnumerable<AlligatorResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

:warning: **Warning**: The first `N+100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### AlligatorResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Jaw` | decimal | The smoothed moving average representing the Alligator's Jaw
| `Teeth` | decimal | The smoothed moving average representing the Alligator's Teeth
| `Lips` | decimal | The smoothed moving average representing the Alligator's Lips

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate the Williams' Alligator
IEnumerable<AlligatorResult> results = Indicator.GetAlligator(history);

// use results as needed
AlligatorResult result = results.LastOrDefault();
Console.WriteLine("Jaw on {0} was ${1}", result.Date, result.Jaw);
Console.WriteLine("Teeth on {0} was ${1}", result.Date, result.Teeth);
Console.WriteLine("Lips on {0} was ${1}", result.Date, result.Lips);
```

```bash
Jaw on 12/31/2018 was $247.03
Teeth on 12/31/2018 was $243.99
Lips on 12/31/2018 was $242.94
```
