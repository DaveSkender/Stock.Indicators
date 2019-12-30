# Ulcer Index (UI)

UI is a measure of downside volatility over the lookback period.
[More info ...](https://school.stockcharts.com/doku.php?id=technical_indicators:ulcer_index)

``` C#
// usage
IEnumerable<UlcerIndexResult> results = Indicator.GetUlcerIndex(history, lookbackPeriod);  
```

## Parameters

| name | type | notes
| -- |-- |--
| `history` | IEnumerable\<[Quote](../../README.md#Quote)\> | Historical Quotes data should be at any consistent frequency (day, hour, minute, etc).  You must supply at least `N` periods of `history`.
| `lookbackPeriod` | int | Number of periods (`N`) for review.

## Response

``` C#
IEnumerable<UlcerIndexResult>
```

The first `N-1` slow periods + signal period will have `null` values since there's not enough data to calculate.  We always return the same number of elements as there are in the historical quotes.

### UlcerIndexResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `UI` | decimal | The UlcerIndex value

## Example

``` C#
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate UI(14)
IEnumerable<UlcerIndexResult> results = Indicator.GetUlcerIndex(history,14);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
UlcerIndexResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("UlcerIndex on {0} was ${1}", result.Date, result.UI);
```

``` text
UlcerIndex on 12/31/2018 was 5.73
```
