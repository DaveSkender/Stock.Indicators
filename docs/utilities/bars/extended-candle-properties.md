---
title: Extended candle properties
description: Convert bars into an extended format with calculated candle properties.
---

# Extended candle properties

`bar.ToCandle()` and `bars.ToCandles()` convert a bar class into an extended bar format with additional calculated candle properties.

```csharp
// single bar
CandleProperties candle = bar.ToCandle();

// collection of bars
IReadOnlyList<CandleProperties> candles = bars.ToCandles();
```

## Response

<!--@include: ../../shared/candle-properties.md-->

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
- [Candlestick patterns](/indicators/marubozu) - Built-in pattern indicators
