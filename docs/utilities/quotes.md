---
title: Quote utilities
description: Utilities for preparing and transforming historical price quotes.
---

# Quote utilities

Utilities for preparing and transforming historical price quotes before using them with indicators.

## Use alternate price

`quotes.Use()` can be used before most indicator calls to specify which price element to analyze. It cannot be used for indicators that require the full OHLCV quote profile.

```csharp
// example: use HL2 price instead of
// the standard Close price for RSI
var results = quotes
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

## Sort quotes

`quotes.ToSortedList()` sorts any collection of `TQuote` or `ISeries` and returns it as an `IReadOnlyList` sorted by ascending `Timestamp`. Quotes should be in chronological order before using library indicators; use this utility to sort them if needed.

## Resize quote history

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

### PeriodSize options (for newSize)

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

::: warning
Partially populated period windows at the beginning, end, and market open/close points in `quotes` can be misleading when aggregated. For example, if you are aggregating intraday minute bars into 15 minute bars and there is a single 4:00pm minute bar at the end, the resulting 4:00pm 15-minute bar will only have one minute of data in it whereas the previous 3:45pm bar will have all 15 minutes of bars aggregated (3:45-3:59pm).
:::

## Extended candle properties

`quote.ToCandle()` and `quotes.ToCandles()` convert a quote class into an extended quote format with additional calculated candle properties.

```csharp
// single quote
CandleProperties candle = quote.ToCandle();

// collection of quotes
IReadOnlyList<CandleProperties> candles = quotes.ToCandles();
```

## Validate quote history

`quotes.Validate()` is an advanced check of your `IReadOnlyList<IQuote> quotes`. It will check for duplicate dates and other bad data and will throw an `InvalidQuotesException` if validation fails. This comes at a small performance cost, so we did not automatically add these advanced checks in the indicator methods. Of course, you can and should do your own validation of `quotes` prior to using it in this library. Bad historical quotes can produce unexpected results.

```csharp
// advanced validation
IReadOnlyList<Quote> validatedQuotes = quotes.Validate();

// and can be used inline with chaining
var results = quotes
  .Validate()
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

---
Last updated: January 7, 2026
