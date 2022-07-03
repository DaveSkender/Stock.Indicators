---
title: Utilities and Helpers
permalink: /utilities/
relative_path: utilities.md
layout: page
redirect_from:
 - /docs/UTILITIES.html
---

# {{ page.title }}

- [for Historical Quotes](#utilities-for-historical-quotes)
- [for Indicator Results](#utilities-for-indicator-results)

## Utilities for historical quotes

### Use alternate price

`quotes.Use()` can be used before most indicator calls to specify price element to analyze.  It cannot be used for indicators that require the full OHLCV quote profile.

```csharp
// example: use HL2 price instead of
// the standard Close price for RSI
var results = quotes
  .Use(CandlePart.HL2)
  .GetRsi(14);
```

{% include candlepart-options.md %}

### Resize quote history

`quotes.Aggregate(newSize)` is a tool to convert quotes to larger bar sizes.  For example if you have minute bar sizes in `quotes`, but want to convert it to hourly or daily.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<TQuote> minuteBarQuotes = GetHistoryFromFeed("MSFT");

// aggregate into larger bars
IEnumerable<Quote> dayBarQuotes =
  minuteBarQuotes.Aggregate(PeriodSize.Day);
```

An alternate version of this utility is provided where you can use any native `TimeSpan` value that is greater than `TimeSpan.Zero`.

```csharp
// alternate usage
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

:warning: **Warning**: Partially populated period windows at the beginning, end, and market open/close points in `quotes` can be misleading when aggregated.  For example, if you are aggregating intraday minute bars into 15 minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).

### Validate quote history

`quotes.Validate()` is an advanced check of your `IEnumerable<TQuote> quotes` (historical quotes).  It will check for duplicate dates and other bad data and will throw an `InvalidQuotesException` if validation fails.  This comes at a small performance cost, so we did not automatically add these advanced checks in the indicator methods.  Of course, you can and should do your own validation of `quotes` prior to using it in this library.  Bad historical quotes data can produce unexpected results.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// advanced validation
IEnumerable<Quote> validatedQuotes = quotes.Validate();
```

## Utilities for indicator results

### Condense

`results.Condense()` will remove non-essential results so it only returns meaningful data records.  For example, when used on [Candlestick Patterns]({{site.baseurl}}/indicators/#candlestick-pattern), it will only return records where a signal is generated.

```csharp
// example: only show Marubozu signals
IEnumerable<CandleResult> results
  = quotes.GetMarubozu(..).Condense();
```

:warning: WARNING! In all cases, `.Condense()` will remove non-essential results and will produce fewer records than are in `quotes`.

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

// remove user-specific quantity of periods
IEnumerable<AdxResult> results =
  quotes.GetAdx(14).RemoveWarmupPeriods(50);
```

See [individual indicator pages]({{site.baseurl}}/indicators/#content) for information on recommended pruning quantities.

:warning: Note: `.RemoveWarmupPeriods()` is not available on indicators that do not have any recommended pruning; however, you can still do a custom pruning by using the customizable `.RemoveWarmupPeriods(removePeriods)`.

:warning: WARNING! `.RemoveWarmupPeriods()` will reverse-engineer some parameters in determining the recommended pruning amount.  Consequently, on rare occasions when there are unusual results, there can be an erroneous increase in the amount of pruning.  Using this method on chained indicators without specifying `removePeriods` is strongly discouraged.  If you want more certainty, use the `.RemoveWarmupPeriods(removePeriods)` with a specific number of `removePeriods`.
