---
title: McGinley Dynamic
description: Created by John R. McGinley, the McGinley Dynamic is a more responsive variant of exponential moving average.
---

# McGinley Dynamic

Created by John R. McGinley, the [McGinley Dynamic](https://www.investopedia.com/terms/m/mcginley-dynamic.asp) is a more responsive variant of exponential moving average.
[[Discuss] &#128172;](https://github.com/DaveSkender/Stock.Indicators/discussions/866 "Community discussion about this indicator")

<ClientOnly>
  <IndicatorChart src="/data/Dynamic.json" :height="360" />
</ClientOnly>

```csharp
// C# usage syntax (with Close price)
IReadOnlyList<DynamicResult> results =
  quotes.ToDynamic(lookbackPeriods, kFactor);
```

## Parameters

| param | type | description |
| ----- | ---- | ----------- |
| `lookbackPeriods` | int | Number of periods (`N`) in the moving average.  Must be greater than 0. |
| `kFactor` | double | Optional.  Range adjustment factor (`K`).  Must be greater than 0.  Default is 0.6 |

### Historical quotes requirements

You must have at least `2` periods of `quotes`, to cover the [warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) periods.  Since this uses a smoothing technique, we recommend you use at least `4×N` data points prior to the intended usage date for better precision.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide](/guide#historical-quotes) for more information.

### Pro tips

> Use a `kFactor` value of `1` if you do not want to adjust the `N` value.
>
> McGinley suggests that using a `K` value of 60% (0.6) allows you to use a `N` equivalent to other moving averages.  For example, DYNAMIC(20,0.6) is comparable to EMA(20); conversely, DYNAMIC(20,1) uses the raw 1:1 `N` value and is not equivalent.

## Response

```csharp
IReadOnlyList<DynamicResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period will have a `null` value since there's not enough data to calculate.

::: warning ⚞ Convergence warning
The first `4×N` periods will have decreasing magnitude, convergence-related precision errors that can be as high as ~5% deviation in indicator values for earlier periods.
:::

### `DynamicResult`

| property | type | description |
| -------- | ---- | ----------- |
| `Timestamp` | DateTime | Date from evaluated `TQuote` |
| `Dynamic` | double | McGinley Dynamic |

### Utilities

- [.Condense()](/utilities/results#condense)
- [.Find(lookupDate)](/utilities/results#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)](/utilities/results#remove-warmup-periods)

See [Utilities and helpers](/utilities/results) for more information.

## Chaining

This indicator may be generated from any chain-enabled indicator or method.

```csharp
// example
var results = quotes
    .Use(CandlePart.HL2)
    .ToDynamic(..);
```

Results can be further processed on `Dynamic` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .ToDynamic(..)
    .ToRsi(..);
```

## Streaming

Use the buffer-style `List<T>` when you need incremental calculations without a hub:

```csharp
DynamicList dynamicList = new(lookbackPeriods, kFactor);

foreach (IQuote quote in quotes)  // simulating stream
{
  dynamicList.Add(quote);
}

// based on `ICollection<DynamicResult>`
IReadOnlyList<DynamicResult> results = dynamicList;
```

Subscribe to a `QuoteHub` for advanced streaming scenarios:

```csharp
QuoteHub quoteHub = new();
DynamicHub observer = quoteHub.ToDynamicHub(lookbackPeriods, kFactor);

foreach (IQuote quote in quotes)  // simulating stream
{
  quoteHub.Add(quote);
}

IReadOnlyList<DynamicResult> results = observer.Results;
```
