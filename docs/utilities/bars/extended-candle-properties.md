---
title: Extended candle properties
description: Convert bars into an extended format with calculated candle properties.
---

# Extended candle properties

`bar.ToCandle()` and `bars.ToCandles()` convert a bar class into an extended bar format with additional calculated candle properties.

## Syntax

```csharp
// single bar
CandleProperties candle = bar.ToCandle();

// collection of bars
IReadOnlyList<CandleProperties> candles = bars.ToCandles();
```

## Returns

**CandleProperties** - An extended bar object with calculated candle properties including:

- All original OHLCV data
- Body size and position
- Upper and lower wick lengths
- Candle range and statistics
- Additional calculated properties

## Usage

### Single bar

```csharp
Bar bar = new() {
  Timestamp = DateTime.Now,
  Open = 100,
  High = 105,
  Low = 98,
  Close = 103,
  Volume = 10000
};

CandleProperties candle = bar.ToCandle();

// access extended properties
decimal? bodySize = candle.Body;
decimal? upperWick = candle.UpperWick;
decimal? lowerWick = candle.LowerWick;
```

### Collection of bars

```csharp
IEnumerable<Bar> bars = GetHistoricalBars();
IReadOnlyList<CandleProperties> candles = bars.ToCandles();

foreach (var candle in candles)
{
  Console.WriteLine($"Date: {candle.Timestamp}, Body: {candle.Body}");
}
```

## CandleProperties members

The `CandleProperties` class extends the basic bar with calculated properties:

| Property | Type | Description |
|----------|------|-------------|
| Size | decimal? | Total candle range (High - Low) |
| Body | decimal? | Absolute difference between Open and Close |
| UpperWick | decimal? | Distance from top of body to High |
| LowerWick | decimal? | Distance from bottom of body to Low |
| BodyPct | double? | Body size as percentage of total size |
| UpperWickPct | double? | Upper wick size as percentage of total size |
| LowerWickPct | double? | Lower wick size as percentage of total size |
| IsBullish | bool | True if Close > Open |
| IsBearish | bool | True if Close < Open |

::: info Property availability
The exact properties available may vary by library version. Refer to the [CandleProperties API documentation](https://github.com/DaveSkender/Stock.Indicators/blob/main/src/_common/Candles/CandleProperties.cs) for the complete list.
:::

## Common use cases

### Candlestick pattern analysis

Analyze candle characteristics for pattern recognition:

```csharp
var candles = bars.ToCandles();

var largeBodies = candles
  .Where(c => c.BodyPct > 0.7)  // Body is >70% of range
  .Where(c => c.IsBullish);     // Bullish candles only
```

### Wick analysis

Identify candles with significant wicks:

```csharp
var candles = bars.ToCandles();

// Find hammer patterns (long lower wick)
var hammers = candles
  .Where(c => c.LowerWick > c.Body * 2)
  .Where(c => c.UpperWick < c.Body * 0.5m);
```

### Volume-weighted candles

Combine extended properties with volume analysis:

```csharp
var candles = bars.ToCandles();

var highVolumeCandles = candles
  .OrderByDescending(c => c.Volume)
  .Take(20)
  .Where(c => c.Body > avgBodySize);
```

## Performance considerations

::: tip Performance
`.ToCandles()` performs calculations on each bar. For large datasets, consider caching the results if you need to access the candle properties multiple times.
:::

## Related utilities

- [Bar utilities overview](/utilities/bars/)
- [Validate bar history](/utilities/bars/validate-bar-history) - Ensure data quality
- [Candlestick patterns](/indicators/Marubozu) - Built-in pattern indicators
