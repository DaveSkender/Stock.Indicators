# Fractal

[Fractal](https://www.investopedia.com/terms/f/fractal.asp) is a price pattern that identifies a high or low point over 5 periods.

![image](chart.png)

```csharp
// usage
IEnumerable<FractalResult> results = Indicator.GetFractal(history);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[TQuote](../../docs/GUIDE.md#quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least 5 periods of `history`.

## Response

```csharp
IEnumerable<FractalResult>
```

The first and last two periods in `history` are unable to be calculated since there's not enough prior/following data.
We always return the same number of elements as there are in the historical quotes.

WARNING: this price pattern looks forward and backward in the historical quotes so it will never identify a `fractal` in the last two periods of `history`.

### FractalResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `FractalBear` | decimal | Value indicates a **high** point; otherwise `null` is returned.
| `FractalBull` | decimal | Value indicates a **low** point; otherwise `null` is returned.

## Example

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate Fractal
IEnumerable<FractalResult> results = Indicator.GetFractal(history);

// use results as needed
FractalResult r = results.Where(x=>x.FractalBear!=null).LastOrDefault();
Console.WriteLine("FractalBear on {0} was ${1}", r.Date, r.FractalBear);
```

```bash
FractalBear on 12/12/2018 was $262.47
```
