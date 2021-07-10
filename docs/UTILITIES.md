# Utilities and Helpers

- [for Historical Quotes](#utilities-for-historical-quotes)
- [for Indicator Results](#utilities-for-indicator-results)

## Utilities for historical quotes

### Resize quote history

`quotes.Aggregate(newSize)` is a tool to convert quotes to larger bar sizes.  For example if you have minute bar sizes in `quotes`, but want to convert it to hourly or daily.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<TQuote> minuteBarQuotes = GetHistoryFromFeed("MSFT");

// aggregate into larger bars
IEnumerable<Quote> dayBarQuotes = 
  minuteBarQuotes.Aggregate(PeriodSize.Day);
```

:warning: **Warning**: Partially populated period windows at the beginning, end, and market open/close points in `quotes` can be misleading when aggregated.  For example, if you are aggregating intraday minute bars into 15 minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).

#### PeriodSize options (for newSize)

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

## Utilities for indicator results

### Remove warmup periods

`results.RemoveWarmupPeriods()` will remove the recommended initial warmup periods from indicator results.
An alternative `.RemoveWarmupPeriods(removePeriods)` is also provided if you want to customize the pruning amount.

```csharp
// auto remove recommended warmup periods
IEnumerable<AdxResult> results = 
  quotes.GetAdx(14)
    .RemoveWarmupPeriods();

// remove user-specific quantity of periods
IEnumerable<AdxResult> results = 
  quotes.GetAdx(14)
    .RemoveWarmupPeriods(50);
```

See [individual indicator pages](INDICATORS.md) for information on recommended pruning quantities.

:warning: Note: `.RemoveWarmupPeriods()` is not available on indicators that do not have any recommended pruning; however, you can still do a custom pruning by using the customizable `.RemoveWarmupPeriods(removePeriods)`.

:warning: WARNING! `.RemoveWarmupPeriods()` will reverse-engineer some parameters in determing the recommended pruning amount.  Consequently, on rare occassions when there are unusual results, there can be an erroneous increase in the amount of pruning.  If you want more certainty, use the `.RemoveWarmupPeriods(removePeriods)` with a specific number of `removePeriods`.

### Find indicator result by date

`results.Find(lookupDate)` is a simple lookup for your indicator results collection.  Just specify the date you want returned.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate indicator series
IEnumerable<SmaResult> results = quotes.GetSma(20);

// find result on a specific date
DateTime lookupDate = [..] // the date you want to find
SmaResult result = results.Find(lookupDate);
```
