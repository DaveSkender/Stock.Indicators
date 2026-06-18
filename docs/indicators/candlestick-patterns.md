---
title: Candlestick patterns
description: Chart patterns derived from candle shapes and price formations.
layout: home
hero:
  name: Candlestick patterns
  tagline: Chart patterns derived from candle shapes and price formations.
  actions:
    - theme: alt
      text: ← All categories
      link: /indicators/
features:
  - title: Doji
    icon:
      src: /assets/thumbs/indicators/doji.png
    link: /indicators/Doji
    details: Indecision candle pattern where open and close price are virtually identical
  - title: Marubozu
    icon:
      src: /assets/thumbs/indicators/marubozu.png
    link: /indicators/Marubozu
    details: Uni-directional solid-body momentum candle pattern with consistent price movement
  - title: Pivots
    icon:
      src: /assets/thumbs/indicators/pivots.png
    link: /indicators/Pivots
    details: Peak and trough chevron patterns with configurable periods wings and higher high, lower low identification
  - title: Fractal (Williams)
    icon:
      src: /assets/thumbs/indicators/fractal.png
    link: /indicators/Fractal
    details: Peak and trough chevron patterns with ±2 fixed period wings for short term pivots
---

## Candlestick signal output

Candlestick pattern indicators return a `Match` enum value indicating whether a pattern is recognized, with optional confirmation and basis signals.

| type | int | description |
| ---- | ---: | ----------- |
| `Match.BullConfirmed` | 200 | Confirmation of a prior bull signal |
| `Match.BullSignal` | 100 | Bullish signal |
| `Match.BullBasis` | 10 | Bars supporting a bullish signal |
| `Match.Neutral` | 1 | Signal for non-directional patterns |
| `Match.None` | 0 | No match |
| `Match.BearBasis` | -10 | Bars supporting a bearish signal |
| `Match.BearSignal` | -100 | Bearish signal |
| `Match.BearConfirmed` | -200 | Confirmation of a prior bear signal |
