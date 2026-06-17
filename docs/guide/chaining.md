---
title: Chaining indicators
description: Combine indicators to build indicators of indicators — feed one indicator's results into another across any indicator style.
---

# Chaining indicators

**Chaining** lets you calculate an _indicator of indicators_ — for example an SMA of an [ADX](/indicators/Adx), or an [RSI of OBV](https://medium.com/@robswc/this-is-what-happens-when-you-combine-the-obv-and-rsi-indicators-6616d991773d). Instead of feeding raw bars into a single indicator, you feed the **results** of one indicator into the next.

```csharp
// RSI of On-Balance Volume
IReadOnlyList<RsiResult> results = bars
    .ToObv()
    .ToRsi(14);
```

You can also start a chain from a chosen price field with [`Use(CandlePart)`](/utilities/bars/use-alternate-price):

```csharp
// EMA of the HL2 price (average of high and low)
IReadOnlyList<EmaResult> results = bars
    .Use(CandlePart.HL2)
    .ToEma(20);
```

## How it works

Chaining works through the `IReusable` interface. Any result that implements `IReusable` exposes a single representative `Value`, which the next indicator consumes as its input series:

- **Chainable inputs** — most indicators accept a reusable series, so they can take either raw `bars` or another indicator's results.
- **Chainable outputs** — a result type is chainable only when it implements `IReusable` (it exposes a `Value`). Indicators that produce multiple primary outputs (e.g. bands or channels) may not be chainable as a source.

See [Creating custom indicators](/guide/customization) to make your own indicators chainable.

## Chaining in each style

Chaining is available in all three [indicator styles](/guide/styles/); the concept is the same, but the mechanics differ:

- **[Batch (Series)](/guide/styles/batch#chaining-indicators)** — chain extension methods fluently (`bars.ToObv().ToRsi(14)`). The standard, default approach.
- **[Buffer lists](/guide/styles/buffer#chaining-indicators)** — feed one buffer list's results into another as values arrive. Unlike the other styles, this chaining is orchestrated by you, not the library: you decide when to pass each result onward.
- **[Stream hubs](/guide/styles/stream#chaining-indicators)** — subscribe one hub to another so chained indicators update automatically as new bars stream in.

## See also

- [Indicator styles](/guide/styles/) — compare batch, buffer, and stream
- [Use alternate price](/utilities/bars/use-alternate-price) — start a chain from a chosen price field
- [Creating custom indicators](/guide/customization) — make your own indicators chainable
