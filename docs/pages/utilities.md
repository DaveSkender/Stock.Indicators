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
- [indicator metadata catalog](#indicator-catalog-metadata)

## Utilities for historical quotes

### Use alternate price

`quotes.Use()` can be used before most indicator calls to specify which price element to analyze.  It cannot be used for indicators that require the full OHLCV quote profile.

```csharp
// example: use HL2 price instead of
// the standard Close price for RSI
var results = quotes
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

{% include candlepart-options.md %}

### Sort quotes

`quotes.ToSortedList()` sorts any collection of `TQuote` or `ISeries` and returns it as a `IReadOnlyList` sorted by ascending `Timestamp`.  You **do need to sort quotes** before using library indicators.

### Resize quote history

`quotes.Aggregate(newSize)` is a tool to convert quotes to larger bar sizes.  For example if you have minute bar sizes in `quotes`, but want to convert it to hourly or daily.

```csharp
// aggregate into larger bars
IReadOnlyList<Quote> dayBarQuotes =
  minuteBarQuotes.Aggregate(PeriodSize.Day);
```

An alternate version of this utility is provided where you can use any native `TimeSpan` value that is greater than `TimeSpan.Zero`.

```csharp
// alternate usage with TimeSpan
IReadOnlyList<Quote> dayBarQuotes =
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
IReadOnlyList<CandleProperties> candles = quotes.ToCandles();
```

{% include candle-properties.md %}

### Validate quote history

`quotes.Validate()` is an advanced check of your `IReadOnlyList<IQuote> quotes`.  It will check for duplicate dates and other bad data and will throw an `InvalidQuotesException` if validation fails.  This comes at a small performance cost, so we did not automatically add these advanced checks in the indicator methods.  Of course, you can and should do your own validation of `quotes` prior to using it in this library.  Bad historical quotes can produce unexpected results.

```csharp
// advanced validation
IReadOnlyList<Quote> validatedQuotes = quotes.Validate();

// and can be used inline with chaining
var results = quotes
  .Validate()
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

## Utilities for indicator results

### Condense

`results.Condense()` will remove non-essential results so it only returns meaningful data records.  For example, when used on [Candlestick Patterns]({{site.baseurl}}/indicators/#candlestick-pattern), it will only return records where a signal is generated.

```csharp
// example: only show Marubozu signals
IReadOnlyList<CandleResult> results
  = quotes.ToMarubozu(..).Condense();
```

> &#128681; **Warning**: In all cases, `.Condense()` will remove non-essential results and will produce fewer records than are in `quotes`.

### Find indicator result by date

`results.Find(lookupDate)` is a simple lookup for your indicator results collection.  Just specify the date you want returned.

```csharp
// calculate indicator series
IReadOnlyList<SmaResult> results = quotes.ToSma(20);

// find result on a specific date
DateTime lookupDate = [..] // the date you want to find
SmaResult result = results.Find(lookupDate);

// throws 'InvalidOperationException' when not found
```

### Remove warmup periods

`results.RemoveWarmupPeriods()` will remove the recommended initial warmup periods from indicator results.  An alternative `.RemoveWarmupPeriods(removePeriods)` is also provided if you want to customize the pruning amount.

```csharp
// auto remove recommended warmup periods
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(14).RemoveWarmupPeriods();

// remove a specific quantity of periods
int n = 14;
IReadOnlyList<AdxResult> results =
  quotes.ToAdx(n).RemoveWarmupPeriods(n+100);
```

See [individual indicator pages]({{site.baseurl}}/indicators/#content) for information on recommended pruning quantities.

> &#128161; **Note**: `.RemoveWarmupPeriods()` is not available on some indicators; however, you can still do a custom pruning by using the customizable `.RemoveWarmupPeriods(removePeriods)`.
>
> &#128681; **Warning**: without a specified `removePeriods` value, this utility will reverse-engineer the pruning amount.  When there are unusual results or when chaining multiple indicators, there will be an erroneous increase in the amount of pruning.  If you want more certainty, use a specific number for `removePeriods`.  Using this method on chained indicators without `removePeriods` is strongly discouraged.

### Sort results

`results.ToSortedList()` sorts any collection of indicator results and returns it as a `IReadOnlyList` sorted by ascending `Timestamp`.  Results from the library indicators are already sorted, so you'd only potentially need this if you're creating [custom indicators]({{site.baseurl}}/custom-indicators/#content).

## Utilities for numerical analysis

This library also includes several tools that we use internally to calculate indicator algorithms.  These can be useful if you are creating your own [custom indicators]({{site.baseurl}}/custom-indicators/).

### Numerical methods

<!-- markdownlint-disable MD060 -->
| method             | example usage |
|--------------------|---------------|
| Slope              | `double[] xValues = { 1, 2, 5, 4 };`<br>`double[] yValues = { 4, 7, 8, 1 };`<br>`double slope = Numerix.Slope(xValues, yValues);` |
| Standard deviation | `double[] values = { 1, 2, 3, 4 };`<br>`double sd = values.StdDev();` |
<!-- markdownlint-enable MD060 -->

### NullMath

Most `NullMath` methods work exactly like the `System.Math` library in C#, except these will return `null` if a `null` is provided.  The `System.Math` library infamously does not allow `null` values, so you'd always need to apply defensive code.  This class does that defensive `null` handling for you.

<!-- markdownlint-disable MD060 -->
| method   | example usage |
|----------|---------------|
| Abs      | `var abs = NullMath.Abs(-25)` » `25`<br>`var abs = NullMath.Abs(null)` » `null` |
| Round    | `var rnd = NullMath.Round(1.234, 1)` » `1.2`<br>`var rnd = NullMath.Round(null, 1)` » `null` |
| Null2NaN | `var val = null;`<br>`var n2n = val.Null2NaN()` » `[NaN]` |
| NaN2Null | `var val = double.NaN;`<br>`var n2n = val.NaN2Null()` » `null` |
<!-- markdownlint-enable MD060 -->

## Indicator catalog (metadata)

Use the indicator catalog to discover indicators, build simple pickers, or export metadata for a UI.

- Discover indicators and parameters at runtime
- Build configuration UIs or export to JSON
- Optionally execute an indicator by ID (no compile-time generics required)

> [!IMPORTANT]
> _The Catalog_ provides a programatic way to interact with indicators and options; however, it is not the idiomatic .NET way to use this library.  See the examples in [the Guide](guide.md) for normal sytax examples.

### Browse or export the catalog

```csharp
using Skender.Stock.Indicators;
using System.Text.Json;

// all listings (name, id, style, category, parameters, results)
IReadOnlyCollection<IndicatorListing> indicatorListings
    = CatalogRegistry.Get();

// optional: filter helpers
IndicatorListing? emaSeriesListing 
    = CatalogRegistry.Get("EMA", Style.Series);

IReadOnlyCollection<IndicatorListing> seriesListings
    = CatalogRegistry.Get(Style.Series);

// convert to JSON
string catalogJson = myListings.ToJson();
```

### Execute by ID (dynamic)

```csharp
// run an indicator using just ID + Style
IReadOnlyList<EmaResult> byId = quotes.ExecuteById<EmaResult>(
    id: "EMA",
    style: Style.Series,
    parameters: new() {
        { "lookbackPeriods", lookback }
    });
```

### Execute by config (JSON)

```csharp
string string json = """
    {
        "id" : "EMA",
        "style" : "Series",
        "parameters" : { "lookbackPeriods" : 20 }
    }
    """;

IReadOnlyList<EmaResult> emaResultsFromJson
    = quotes.ExecuteFromJson<EmaResult>(json);
```

### Execute with strong typing

```csharp
// prefer typed results when you know the indicator
IndicatorListing indicatorListing = IndicatorRegistry
  .GetByIdAndStyle("EMA", Style.Series)
  ?? throw new InvalidOperationException("Indicator 'EMA' (Series) not found.");

// Call the quotes overload directly
IReadOnlyList<EmaResult> emaResultsWithDefaults = indicatorListing
  .Execute<EmaResult>(quotes);

// Or with specified parameters
IReadOnlyList<EmaResult> emaResultsWithParams = indicatorListing
  .FromSource(quotes)
  .WithParamValue("lookbackPeriods", 10)
  .Execute<EmaResult>();
```
