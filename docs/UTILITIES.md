# Utilities and Helpers

- [for Historical Quotes](#utilities-for-historical-quotes)
- [for Indicator Results](#utilities-for-indicator-results)

## Utilities for historical quotes

### Resize quote history

`history.Aggregate(newSize)` is a tool to convert history to larger bar sizes.  For example if you have minute bar sizes in `history`, but want to convert it to hourly or daily.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<TQuote> minuteBarHistory = GetHistoryFromFeed("MSFT");

// aggregate into larger bars
IEnumerable<Quote> dayBarHistory = 
  minuteBarHistory.Aggregate(PeriodSize.Day);
```

:warning: **Warning**: Partially populated period windows at the beginning, end, and market open/close points in `history` can be misleading when aggregated.  For example, if you are aggregating intraday minute bars into 15 minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).

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

### Prune warmup periods

`results.PruneWarmupPeriods()` will remove the recommended initial periods from indicator results.
An alternative `.PruneWarmupPeriods(prunePeriods)` is also provided if you want to customize the pruning amount.

```csharp
// prune recommended periods
IEnumerable<AdxResult> results = 
  history.GetAdx(14)
    .PruneWarmupPeriods();

// prune user-specific quantity of periods
IEnumerable<AdxResult> results = 
  history.GetAdx(14)
    .PruneWarmupPeriods(50);
```

See [individual indicator pages](INDICATORS.md) for information on recommended pruning quantities.

:warning: IMPORTANT! `.PruneWarmupPeriods()` is not available on indicators that do not have any recommended pruning; however, you can still do a custom pruning by using the customizable `.PruneWarmupPeriods(prunePeriods)`.

### Find indicator result by date

`results.Find()` is a simple lookup for your indicator results collection.  Just specify the date you want returned.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate indicator series
IEnumerable<SmaResult> results = history.GetSma(20);

// find result on a specific date
DateTime lookupDate = [..] // the date you want to find
SmaResult result = results.Find(lookupDate);
```
