---
title: Extended candle properties
description: Convert quotes into an extended format with calculated candle properties.
---

# Extended candle properties

`quote.ToCandle()` and `quotes.ToCandles()` convert a quote class into an extended quote format with additional calculated candle properties.

## Syntax

```csharp
// single quote
CandleProperties candle = quote.ToCandle();

// collection of quotes
IReadOnlyList<CandleProperties> candles = quotes.ToCandles();
```

## Returns

**CandleProperties** - An extended quote object with calculated candle properties including:

- All original OHLCV data
- Body size and position
- Upper and lower wick lengths
- Candle range and statistics
- Additional calculated properties

## Usage

### Single quote

```csharp
Quote quote = new() {
  Timestamp = DateTime.Now,
  Open = 100,
  High = 105,
  Low = 98,
  Close = 103,
  Volume = 10000
};

CandleProperties candle = quote.ToCandle();

// access extended properties
decimal bodySize = candle.BodySize;
decimal upperWick = candle.UpperWick;
decimal lowerWick = candle.LowerWick;
```

### Collection of quotes

```csharp
IEnumerable<Quote> quotes = GetHistoricalQuotes();
IReadOnlyList<CandleProperties> candles = quotes.ToCandles();

foreach (var candle in candles)
{
  Console.WriteLine($"Date: {candle.Timestamp}, Body: {candle.BodySize}");
}
```

## CandleProperties members

The `CandleProperties` class extends the basic quote with calculated properties:

| Property | Type | Description |
|----------|------|-------------|
| BodySize | decimal | Absolute difference between Open and Close |
| BodyPct | decimal | Body size as percentage of total range |
| UpperWick | decimal | Distance from top of body to High |
| LowerWick | decimal | Distance from bottom of body to Low |
| IsBullish | bool | True if Close > Open |
| IsBearish | bool | True if Open > Close |
| Range | decimal | High - Low |

::: info Property availability
The exact properties available may vary by library version. Refer to the [CandleProperties API documentation](https://github.com/DaveSkender/Stock.Indicators/blob/main/src/_common/Candles/CandleProperties.cs) for the complete list.
:::

## Common use cases

### Candlestick pattern analysis

Analyze candle characteristics for pattern recognition:

```csharp
var candles = quotes.ToCandles();

var largeBodies = candles
  .Where(c => c.BodyPct > 0.7)  // Body is >70% of range
  .Where(c => c.IsBullish);     // Bullish candles only
```

### Wick analysis

Identify candles with significant wicks:

```csharp
var candles = quotes.ToCandles();

// Find hammer patterns (long lower wick)
var hammers = candles
  .Where(c => c.LowerWick > c.BodySize * 2)
  .Where(c => c.UpperWick < c.BodySize * 0.5);
```

### Volume-weighted candles

Combine extended properties with volume analysis:

```csharp
var candles = quotes.ToCandles();

var highVolumeCandles = candles
  .OrderByDescending(c => c.Volume)
  .Take(20)
  .Where(c => c.BodySize > avgBodySize);
```

## Performance considerations

::: tip Performance
`.ToCandles()` performs calculations on each quote. For large datasets, consider caching the results if you need to access the candle properties multiple times.
:::

## Related utilities

- [Quote utilities overview](/utilities/quotes/)
- [Validate quote history](/utilities/quotes/validate-quote-history) - Ensure data quality
- [Candlestick patterns](/indicators/Marubozu) - Built-in pattern indicators
