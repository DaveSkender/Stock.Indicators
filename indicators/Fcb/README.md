# Fractal Chaos Bands (FCB)

Created by Edward William Dreiss, Fractal Chaos Bands outline high and low price channels to depict broad less-chaotic price movements.  FCB is a channelized depiction of [Williams Fractal](../Fractal/README.md#content).
[[Discuss] :speech_balloon:](https://github.com/DaveSkender/Stock.Indicators/discussions/347 "Community discussion about this indicator")

![image](chart.png)

```csharp
// usage
IEnumerable<FcbResult> results = Indicator.GetFcb(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#historical-quotes)\> | Historical price quotes should have a consistent frequency (day, hour, minute, etc).
| `windowSpan` | int | Fractal evaluation window span width (`S`).  Must be at least 2.  Default is 2.

The total evaluation window size is `2×S+1`, representing `±S` from the evalution date.  See [Williams Fractal](../Fractal/README.md#content) for more information about Fractals and `windowSpan`.

### Minimum history requirements

You must supply at least `2×S+1` periods of `history`; however, more is typically provided since this is a chartable candlestick pattern.

## Response

```csharp
IEnumerable<FcbResult>
```

The periods before the first fractal are `null` since they cannot be calculated.
We always return the same number of elements as there are in the historical quotes.

### FcbResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `UpperBand` | decimal | FCB upper band
| `LowerBand` | decimal | FCB lower band

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate Fcb(14)
IEnumerable<FcbResult> results = Indicator.GetFcb(history,14);

// use results as needed
FcbResult result = results.LastOrDefault();
Console.WriteLine("FCB Upper Band on {0} was ${1}",
  result.Date, result.UpperBand);
```

```bash
FCB Upper Band on 12/31/2018 was $273.7
```
