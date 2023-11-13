---
title: Utilities and helpers
description: The Stock Indicators for .NET library includes utilities to help you use and transform historical prices quotes and indicator results, and to create custom indicators.
permalink: /utilities/
relative_path: pages/utilities.md
layout: page
---

# {{ page.title }}

- [for historical quotes](#utilities-for-historical-quotes)
- [for indicator results](#utilities-for-indicator-results)
- [for numerical analysis](#utilities-for-numerical-analysis)

## Utilities for historical quotes

### Use alternate price

`quotes.Use()` can be used before most indicator calls to specify which price element to analyze.  It cannot be used for indicators that require the full OHLCV quote profile.

```csharp
// example: use HL2 price instead of
// the standard Close price for RSI
var results = quotes
  .Use(CandlePart.HL2)
  .GetRsi(14);
```

{% include candlepart-options.md %}

### Using tuple quotes

`quotes.ToTupleCollection()` is a method for converting any `TQuote` collection to a simple [tuple](https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-tuples) `(DateTime, double)` formatted `Collection`.  Most indicators in our library will accept this tuple format.  With that said, there are many indicators that also require the full OHLCV quote format, so it cannot be used universally.

### Sort quotes

`quotes.ToSortedCollection()` sorts any collection of `TQuote` or tuple `(DateTime, double)` and returns it as a `Collection` sorted by ascending `Date`.  You do not need to sort quotes before using library indicators; however, if you are creating [custom indicators]({{site.baseurl}}/custom-indicators/#content) it's important to analyze `quotes` in a proper sequence.

### Resize quote history

`quotes.Aggregate(newSize)` is a tool to convert quotes to larger bar sizes.  For example if you have minute bar sizes in `quotes`, but want to convert it to hourly or daily.

```csharp
// aggregate into larger bars
IEnumerable<Quote> dayBarQuotes =
  minuteBarQuotes.Aggregate(PeriodSize.Day);
```

An alternate version of this utility is provided where you can use any native `TimeSpan` value that is greater than `TimeSpan.Zero`.

```csharp
// alternate usage with TimeSpan
IEnumerable<Quote> dayBarQuotes =
  minuteBarQuotes.Aggregate(TimeSpan timeSpan);
```

#### PeriodSize options (for newSize)

- `PeriodSize.Month`
- `PeriodSize.Week`
- `PeriodSize.Day`
- `PeriodSize.FourHours`
- `PeriodSize.TwoHours`
- `PeriodSize.OneHour`
- `PeriodSize.ThirtyMinutes`
- `PeriodSize.FifteenMinutes`
- `PeriodSize.FiveMinutes`
- `PeriodSize.ThreeMinutes`
- `PeriodSize.TwoMinutes`
- `PeriodSize.OneMinute`

> &#128681; **Warning**: Partially populated period windows at the beginning, end, and market open/close points in `quotes` can be misleading when aggregated.  For example, if you are aggregating intraday minute bars into 15 minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).

### Extended candle properties

`quote.ToCandle()` and `quotes.ToCandles()` converts a quote class into an extended quote format with additional calculated candle properties.

``` csharp
// single quote
CandleProperties candle = quote.ToCandle();

// collection of quotes
IEnumerable<CandleProperties> candles = quotes.ToCandles();
```

{% include candle-properties.md %}

### Validate quote history

`quotes.Validate()` is an advanced check of your `IEnumerable<TQuote> quotes`.  It will check for duplicate dates and other bad data and will throw an `InvalidQuotesException` if validation fails.  This comes at a small performance cost, so we did not automatically add these advanced checks in the indicator methods.  Of course, you can and should do your own validation of `quotes` prior to using it in this library.  Bad historical quotes can produce unexpected results.

```csharp
// advanced validation
IEnumerable<Quote> validatedQuotes = quotes.Validate();

// and can be used inline with chaining
var results = quotes
  .Validate()
  .Use(CandlePart.HL2)
  .GetRsi(14);
```

## Utilities for indicator results

### Condense

`results.Condense()` will remove non-essential results so it only returns meaningful data records.  For example, when used on [Candlestick Patterns]({{site.baseurl}}/indicators/#candlestick-pattern), it will only return records where a signal is generated.

```csharp
// example: only show Marubozu signals
IEnumerable<CandleResult> results
  = quotes.GetMarubozu(..).Condense();
```

> &#128681; **Warning**: In all cases, `.Condense()` will remove non-essential results and will produce fewer records than are in `quotes`.

### Find indicator result by date

`results.Find(lookupDate)` is a simple lookup for your indicator results collection.  Just specify the date you want returned.

```csharp
// calculate indicator series
IEnumerable<SmaResult> results = quotes.GetSma(20);

// find result on a specific date
DateTime lookupDate = [..] // the date you want to find
SmaResult result = results.Find(lookupDate);
```

### Remove warmup periods

`results.RemoveWarmupPeriods()` will remove the recommended initial warmup periods from indicator results.  An alternative `.RemoveWarmupPeriods(removePeriods)` is also provided if you want to customize the pruning amount.

```csharp
// auto remove recommended warmup periods
IEnumerable<AdxResult> results =
  quotes.GetAdx(14).RemoveWarmupPeriods();

// remove a specific quantity of periods
int n = 14;
IEnumerable<AdxResult> results =
  quotes.GetAdx(n).RemoveWarmupPeriods(n+100);
```

See [individual indicator pages]({{site.baseurl}}/indicators/#content) for information on recommended pruning quantities.

> &#128161; **Note**: `.RemoveWarmupPeriods()` is not available on some indicators; however, you can still do a custom pruning by using the customizable `.RemoveWarmupPeriods(removePeriods)`.
>
> &#128681; **Warning**: without a specified `removePeriods` value, this utility will reverse-engineer the pruning amount.  When there are unusual results or when chaining multiple indicators, there will be an erroneous increase in the amount of pruning.  If you want more certainty, use a specific number for `removePeriods`.  Using this method on chained indicators without `removePeriods` is strongly discouraged.

### Using tuple results

`results.ToTupleCollection()` converts results to a simpler `(DateTime Date, double? Value)` [tuple](https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-tuples) `Collection`.

`results.ToTupleNaN()` converts results to simpler `(DateTime Date, double Value)` tuple `Collection` with `null` values converted to `double.NaN`.

`results.ToTupleChainable()` is a specialty converter used to prepare [custom indicators]({{site.baseurl}}/custom-indicators/#content) for chaining by removing `null` warmup periods and converting all remaining `null` values to `double.NaN`.

> &#128681; **Warning**: warmup periods are pruned when using `.ToTupleChainable()`, resulting in fewer records.

### Sort results

`results.ToSortedCollection()` sorts any collection of indicator results and returns it as a `Collection` sorted by ascending `Date`.  Results from the library indicators are already sorted, so you'd only potentially need this if you're creating [custom indicators]({{site.baseurl}}/custom-indicators/#content).

## Utilities for numerical analysis

This library also includes several tools that we use internally to calculate indicator algorithms.  These can be useful if you are creating your own [custom indicators]({{site.baseurl}}/custom-indicators/).

### Numerical methods

| method | example usage
| -- |--
| Slope | `double[] xValues = { 1, 2, 5, 4 };`<br>`double[] yValues = { 4, 7, 8, 1 };`<br>`double slope = Numerix.Slope(xValues, yValues);`
| Standard deviation | `double[] values = { 1, 2, 3, 4 };`<br>`double sd = values.StdDev();`

### NullMath

Most `NullMath` methods work exactly like the `System.Math` library in C#, except these will return `null` if a `null` is provided.  The `System.Math` library infamously does not allow `null` values, so you'd always need to apply defensive code.  This class does that defensive `null` handling for you.

| method | example usage
| -- |--
| Abs | `var abs = NullMath.Abs(-25)` » `25`<br>`var abs = NullMath.Abs(null)` » `null`
| Round | `var rnd = NullMath.Round(1.234, 1)` » `1.2`<br>`var rnd = NullMath.Round(null, 1)` » `null`
| Null2NaN | `var val = null;`<br>`var n2n = val.Null2NaN()` » `[NaN]`
| NaN2Null | `var val = double.NaN;`<br>`var n2n = val.NaN2Null()` » `null`
