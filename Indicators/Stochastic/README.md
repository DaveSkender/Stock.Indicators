# Stochastic Oscillator (STOCH)

Stochastic Oscillator is a momentum indicator that looks back `N` periods to produce a scale of 0 to 100.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:stochastic_oscillator_fast_slow_and_full)

```csharp
// usage
IEnumerable<StochResult> results = Indicator.GetStoch(history, lookbackPeriod, signalPeriod, smoothingPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](/GUIDE.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) in the lookback period to calculate the Oscillator (%K)
| `signalPeriod` | int | Lookback period for the signal (%D)
| `smoothingPeriod` | int | Smoothes the Oscillator (%K).  "Slow" stochastic uses 3, "Fast" stochastic uses 1.  You can specify as needed here.

## Response

```csharp
IEnumerable<StochResult>
```

The first `N-1` periods will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### StochResult

| name | type | notes
| -- |-- |--
| `Index` | int | Sequence of dates
| `Date` | DateTime | Date
| `Oscillator` | float | Oscillator over prior `N` lookback periods
| `Signal` | float | Simple moving average of Oscillator
| `IsIncreasing` | bool | Direction since last period (e.g. up or down).  Is `null` for no change.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate STOCH %K(14),%D(3) (slow)
IEnumerable<StochResult> results = Indicator.GetStoch(history,14,3,3);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
StochResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("STOCH on {0} was {1}", result.Date, result.Oscillator);
```

```bash
STOCH on 12/31/2018 was 43.1
```
