---
title: Price bar utilities
description: Utilities for preparing and transforming historical price bars.
---

# Price bar utilities

Utilities for preparing and transforming historical price bars before using them with indicators.

## Use alternate price or OHLCV price bar part

Specify which price element to analyze (HL2, HLC3, OC2, etc.) instead of the standard Close price. [More info →](/utilities/bars/use-alternate-price)

```csharp
var results = bars.Use(CandlePart.HL2).ToRsi(14);
```

## Sort price bars

Sort any collection of bars into chronological order before using indicators. [More info →](/utilities/bars/sort-bars)

```csharp
var sortedBars = bars.ToSortedList();
```

## Resize bar history

Aggregate intraday bars into larger timeframes. [More info →](/utilities/bars/resize-bar-history)

```csharp
var hourlyBars = minuteBars.Aggregate(BarInterval.OneHour);
```

## Extended candle properties

Convert price bars into an extended format with calculated candle properties. [More info →](/utilities/bars/extended-candle-properties)

```csharp
var candles = bars.ToCandles();
```

## Validate bar history

Advanced validation to check for duplicate dates and bad data. [More info →](/utilities/bars/validate-bar-history)

```csharp
var validBars = bars.Validate();
```
