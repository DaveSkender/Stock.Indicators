---
title: Resize quote history
description: Aggregate intraday bars into larger timeframes.
---

# Resize quote history

Aggregate intraday bars into larger timeframes using `quotes.Aggregate()`. Convert minute bars to hourly, hourly to daily, or use any custom TimeSpan period.

## Syntax

```csharp
// using PeriodSize enum
IReadOnlyList<Quote> aggregatedQuotes = quotes.Aggregate(PeriodSize newSize);

// using TimeSpan
IReadOnlyList<Quote> aggregatedQuotes = quotes.Aggregate(TimeSpan timeSpan);
```

## Parameters

**newSize** - A `PeriodSize` enum value representing the target period size. See [PeriodSize options](#periodsize-options) below.

**timeSpan** - Any `TimeSpan` value greater than `TimeSpan.Zero` for custom aggregation periods.

## Returns

**IReadOnlyList\<Quote\>** - Aggregated quotes at the specified period size.

## Usage

### Standard periods

```csharp
// aggregate 1-minute bars into 15-minute bars
IReadOnlyList<Quote> fifteenMinQuotes =
  minuteBarQuotes.Aggregate(PeriodSize.FifteenMinutes);

// aggregate hourly bars into daily bars
IReadOnlyList<Quote> dayBarQuotes =
  hourlyBarQuotes.Aggregate(PeriodSize.Day);
```

### Custom periods

```csharp
// aggregate into 2-hour bars using TimeSpan
IReadOnlyList<Quote> twoHourQuotes =
  minuteBarQuotes.Aggregate(TimeSpan.FromHours(2));

// aggregate into 45-minute bars
IReadOnlyList<Quote> customQuotes =
  minuteBarQuotes.Aggregate(TimeSpan.FromMinutes(45));
```

## PeriodSize options

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

## How aggregation works

When aggregating bars, the utility:

- **Open** - Uses the first bar's open price in the period
- **High** - Uses the highest high price in the period
- **Low** - Uses the lowest low price in the period
- **Close** - Uses the last bar's close price in the period
- **Volume** - Sums all volume in the period
- **Timestamp** - Uses the period's starting timestamp

## Important considerations

::: warning Partially populated periods
Partially populated period windows at the beginning, end, and market open/close points in `quotes` can be misleading when aggregated.

**Example**: If you are aggregating intraday minute bars into 15-minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).
:::

::: tip Best practices

- Filter out partial periods at market open/close if they could skew your analysis
- Be aware of time zone handling when aggregating across day boundaries
- Consider using only complete periods for backtesting
:::

## Common use cases

### Multi-timeframe analysis

Analyze the same security on different timeframes:

```csharp
// 15-minute trend
var shortTermTrend = minuteQuotes
  .Aggregate(PeriodSize.FifteenMinutes)
  .ToEma(20);

// 4-hour trend
var longTermTrend = minuteQuotes
  .Aggregate(PeriodSize.FourHours)
  .ToEma(20);
```

### Reducing data noise

Aggregate to larger timeframes to smooth out intraday volatility:

```csharp
// reduce 1-minute noise by using 5-minute bars
var smoothedQuotes = tickData
  .Aggregate(PeriodSize.FiveMinutes);
```

## Related utilities

- [Quote utilities overview](/utilities/quotes/)
- [Validate quote history](/utilities/quotes/validate-quote-history) - Ensure data quality
- [Sort quotes](/utilities/quotes/sort-quotes) - Ensure chronological order
