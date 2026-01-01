---
title: Hilbert Transform Instantaneous Trendline
description: Created by John Ehlers, the Hilbert Transform Instantaneous Trendline is a 5-period trendline of high/low price that that uses classic electrical radio-frequency signal processing algorithms reduce noise.
---

# {{ $frontmatter.title }}

Created by John Ehlers, the Hilbert Transform Instantaneous Trendline is a 5-period trendline of high/low price that that uses classic electrical radio-frequency signal processing algorithms reduce noise.  Dominant Cycle Periods information is also provided.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/363 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/HtTrendline.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax
IReadOnlyList<HtlResult> results =
  quotes.ToHtTrendline();
```

## Historical quotes requirements

You must have at least `100` periods of `quotes` to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

## Response

```csharp
IReadOnlyList<HtlResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first `6` periods will have `null` values for `SmoothPrice` since there's not enough data to calculate.
- The first `7` periods will have `null` values for `DcPeriods` since there is not enough data to calculate; and are generally unreliable for the first ~25 periods.

>&#9886; **Convergence warning**: The first `100` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.

### HtlResult

**`Timestamp`** _`DateTime`_ - date from evaluated `TQuote`

**`DcPeriods`** _`int`_ - Dominant cycle periods (smoothed)

**`Trendline`** _`double`_ - HT Trendline

**`SmoothPrice`** _`double`_ - Weighted moving average of `(H+L)/2` price

### Utilities

- [.Condense()](/utilities#condense)
- [.Find(lookupDate)](/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods()](/utilities#remove-warmup-periods)
- [.RemoveWarmupPeriods(qty)](/utilities#remove-warmup-periods)

See [Utilities and helpers](/utilities#utilities-for-indicator-results) for more information.

## Streaming

### Real-time streaming

Use the streaming hub for real-time incremental calculations:

```csharp
QuoteHub quoteHub = new();
HtTrendlineHub observer = quoteHub.ToHtTrendlineHub();

foreach (IQuote quote in quotes)  // simulating stream  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<HtlResult> results = observer.Results;
```

### Buffer-style streaming

Use the buffer-style `List<T>` when you need incremental calculations:

```csharp
HtlList htlList = new();

foreach (IQuote quote in quotes)  // simulating stream
{
  htlList.Add(quote);
}

// based on `ICollection<HtlResult>`
IReadOnlyList<HtlResult> results = htlList;
```

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HLC3)
    .ToHtTrendline(..);
```

Results can be further processed on `Trendline` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToHtTrendline(..)
    .ToRsi(..);
```
