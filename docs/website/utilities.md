# Utilities and helpers

The Stock Indicators for .NET library includes utilities to help you use and transform historical prices quotes and indicator results, and to create custom indicators.

- [for historical quotes](#utilities-for-historical-quotes)
- [for indicator results](#utilities-for-indicator-results)
- [for numerical analysis](#utilities-for-numerical-analysis)

## Utilities for historical quotes

### Use alternate price

`quotes.Use()` can be used before most indicator calls to specify which price element to analyze. It cannot be used for indicators that require the full OHLCV quote profile.

```csharp
// example: use HL2 price instead of
// the standard Close price for RSI
var results = quotes
  .Use(CandlePart.HL2)
  .GetRsi(14);
```

**CandlePart options**

| Enumeration | Name | Description |
|:------------|:-----|:------------|
| `O` | Open | Opening price |
| `H` | High | High price |
| `L` | Low | Low price |
| `C` | Close | Closing price |
| `V` | Volume | Share volume |
| `HL2` | High-Low midpoint | Average of High and Low prices |
| `HLC3` | Typical price | Average of High, Low, and Close prices |
| `OC2` | Open-Close midpoint | Average of Open and Close prices |
| `OHL3` | Mean price | Average of Open, High, and Low prices |
| `OHLC4` | Weighted price | Average of Open, High, Low, and Close prices |

### Using tuple quotes

`quotes.ToTupleCollection()` is a method for converting any `TQuote` collection to a simple [tuple](https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-tuples) `(DateTime, double)` formatted `Collection`. Most indicators in our library will accept this tuple format. With that said, there are many indicators that also require the full OHLCV quote format, so it cannot be used universally.

### Sort quotes {#condense}

`quotes.ToSortedCollection()` sorts any collection of `TQuote` or tuple `(DateTime, double)` and returns it as a `Collection` sorted by ascending `Date`. You do not need to sort quotes before using library indicators; however, if you are creating custom indicators it's important to analyze `quotes` in a proper sequence.

### Resize quote history

`quotes.Aggregate(newSize)` is a tool to convert quotes to larger bar sizes. For example if you have minute bar sizes in `quotes`, but want to convert it to hourly or daily.

```csharp
// aggregate into larger bars
IEnumerable<Quote> dayBarQuotes =
  minuteBarQuotes.Aggregate(PeriodSize.Day);
```

An alternate version of this utility is provided where you can use any native `TimeSpan` value that is greater than `TimeSpan.Zero`.

```csharp
// aggregate to a custom size using TimeSpan
IEnumerable<Quote> customBarQuotes =
  minuteBarQuotes.Aggregate(TimeSpan.FromMinutes(45));
```

## Utilities for indicator results

### Get or exclude nulls {#remove-warmup-periods}

`results.RemoveWarmupPeriods()` will remove all indicator result records that have `NaN` values.  This is useful to remove warm-up and convergence periods that can contain unusable `NaN` values.  Conversely, you can use `HasWarmupPeriods()` to detect if a result set contains `NaN` values.

**Note on behavior change**: this method will remove any record that has at least one `NaN` value; however, the definition of a warm-up period is subjective based on indicator type and usage.  Previously, we removed records based on warm-up estimates, but these did not catch all the `NaN` values.  If you need to truncate by a specific quantity, see the `Skip` example below.

### Find indicator result {#find-indicator-result-by-date}

`results.Find(lookupDate)` finds the result record on or immediately before the lookup date.

```csharp
// get the result based on a lookup date
var record = results.Find(DateTime.Parse("2019-01-15"));

// or alternatively get the 42nd record
var record42 = results.Skip(42).FirstOrDefault();
```

## Utilities for numerical analysis

### Verify that a number is finite

`myNumber.IsNaN()` or `double.IsNaN(myNumber)` can be used to determine if a result is a valid numeric value.  It will return `true` when the value is either `NaN` or `+/-infinity`.
