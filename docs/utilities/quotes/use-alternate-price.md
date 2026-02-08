---
title: Use alternate price
description: Specify which price element to analyze instead of the standard Close price.
---

# Use alternate price

`quotes.Use()` can be used before most indicator calls to specify which price element to analyze. It cannot be used for indicators that require the full OHLCV quote profile.

## Syntax

```csharp
IEnumerable<IQuote> quotes = quotes.Use(CandlePart candlePart);
```

## Parameters

**candlePart** - The price element to use for calculations. See [CandlePart options](#candlepart-options) below.

## Usage

```csharp
// example: use HL2 price instead of
// the standard Close price for RSI
var results = quotes
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

## CandlePart options

- `CandlePart.Open` - Open price
- `CandlePart.High` - High price
- `CandlePart.Low` - Low price
- `CandlePart.Close` - Close price (default for most indicators)
- `CandlePart.Volume` - Volume
- `CandlePart.HL2` - (High + Low) / 2
- `CandlePart.HLC3` - (High + Low + Close) / 3
- `CandlePart.OC2` - (Open + Close) / 2
- `CandlePart.OHL3` - (Open + High + Low) / 3
- `CandlePart.OHLC4` - (Open + High + Low + Close) / 4

## Common use cases

### Median price (HL2)

Use the median between high and low for smoother price action analysis:

```csharp
var smaResults = quotes
  .Use(CandlePart.HL2)
  .ToSma(20);
```

### Typical price (HLC3)

Use the typical price for volume-weighted analyses:

```csharp
var emaResults = quotes
  .Use(CandlePart.HLC3)
  .ToEma(50);
```

## Limitations

::: warning Incompatible indicators
Some indicators require the full OHLCV quote profile and cannot be used with `.Use()`. These include:

- Indicators that explicitly use multiple price points (ATR, Stochastic, etc.)
- Volume-based indicators (OBV, CMF, etc.) when not using `CandlePart.Volume`
- Candlestick pattern indicators

Attempting to use `.Use()` with incompatible indicators may produce unexpected results.
:::

## Related utilities

- [Quote utilities overview](/utilities/quotes/)
- [Validate quote history](/utilities/quotes/validate-quote-history) - Ensure data quality
- [Sort quotes](/utilities/quotes/sort-quotes) - Ensure chronological order
