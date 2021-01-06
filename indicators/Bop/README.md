# Balance of Power (BOP)

Created by Igor Levshin, the [Balance of Power](https://school.stockcharts.com/doku.php?id=technical_indicators:balance_of_power) (aka Balance of Market Power) is a momentum oscillator that depicts the strength of buying and selling pressure.
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/302 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<BopResult> results = Indicator.GetBop(history, smoothPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `smoothPeriod` | int | Number of periods (`N`) for smoothing.  Must be greater than 0.  Default is 14.

### Minimum history requirements

You must supply at least `N` periods of `history`.

## Response

```csharp
IEnumerable<BopResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### BopResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Bop` | decimal | Balance of Power

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 14-period BOP
IEnumerable<BopResult> results = Indicator.GetBop(history,14);

// use results as needed
BopResult result = results.LastOrDefault();
Console.WriteLine("BOP on {0} was {1}", result.Date, result.Bop);
```

```bash
BOP on 12/31/2018 was 0.29
```
