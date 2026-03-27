---
title: Getting started
description: Install the Stock Indicators for .NET library and calculate your first indicator in minutes.
---

# Getting started

## Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project.  See more [help for installing packages](https://www.google.com/search?q=install+nuget+package).

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# package manager example
Install-Package Skender.Stock.Indicators
```

## Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You must get historical quotes from your own market data provider.  For clarification, the `GetQuotesFromFeed()` method shown in the example below **is not part of this library**, but rather an example to represent your own acquisition of historical quotes.

Historical price data can be provided as a `List`, `IReadOnlyList`, or `ICollection` of the `Quote` class ([see below](#historical-quotes)); however, it can also be supplied as a generic [custom TQuote type](#using-custom-quote-classes) if you prefer to use your own quote model.

For additional configuration parameters, default values are provided when there is an industry standard.  You can, of course, override these and provide your own values.

## Implementation pattern

The library supports three indicator styles.  Each follows a common pattern:

```csharp
using Skender.Stock.Indicators;

[..]

// step 1: get quote(s) from your source
// step 2: calculate indicator value(s)
```

See [Indicator styles](/guide/) for a comparison of all three styles.

## Example usage

```csharp
using Skender.Stock.Indicators;

[..]

// fetch historical quotes from your feed (your method)
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("MSFT");

// calculate 20-period SMA
IReadOnlyList<SmaResult> results = quotes
  .ToSma(20);

// use results as needed for your use case (example only)
foreach (SmaResult r in results)
{
    Console.WriteLine($"SMA on {r.Timestamp:d} was ${r.Sma:N4}");
}
```

```console
SMA on 4/19/2018 was $255.0590
SMA on 4/20/2018 was $255.2015
SMA on 4/23/2018 was $255.6135
SMA on 4/24/2018 was $255.5105
SMA on 4/25/2018 was $255.6570
SMA on 4/26/2018 was $255.9705
..
```

See [individual indicator pages](/indicators) for specific usage guidance.

More examples available:

- [Example usage code](/examples/) on GitHub
- [Demo site](https://charts.stockindicators.dev) (a stock chart)

::: tip For a deeper guide
See the [Guide](/guide/) for batch, buffer, and stream styles; chaining; custom indicators; and performance guidance.
:::

## Historical quotes

You must provide historical price quotes to the library in the standard OHLCV `IReadOnlyList<Quote>` or a compatible `List` or `ICollection` format.  It should have a consistent period frequency (day, hour, minute, etc).  See [using custom quote classes](#using-custom-quote-classes) if you prefer to use your own quote class.

| name        | type     | notes       |
| ----------- | -------- | ----------- |
| `Timestamp` | DateTime | Close date  |
| `Open`      | decimal  | Open price  |
| `High`      | decimal  | High price  |
| `Low`       | decimal  | Low price   |
| `Close`     | decimal  | Close price |
| `Volume`    | decimal  | Volume      |

### Where can I get historical quote data?

There are many places to get financial market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, see our ongoing [discussion on market data](https://github.com/DaveSkender/Stock.Indicators/discussions/579) for ideas.

### How much historical quote data do I need?

Each indicator will need different amounts of price `quotes` to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, **most use cases will require that you provide more than the minimum**.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).

::: warning 🚩 IMPORTANT
Applying the _minimum_ amount of quote history as possible is NOT a good way to optimize your system. Some indicators use a smoothing technique that converges to better precision over time. While you can calculate these with the minimum amount of quote data, the precision to two decimal points often requires 250 or more preceding historical records.

For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results. Occasionally, even more is required for optimal precision.

See [discussion on warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) for more information.
:::

### Using custom quote classes

If you would like to use your own custom `MyCustomQuote` class, to avoid needing to transpose into the library `Quote` class, you only need to add the `IQuote` interface.

```csharp
using Skender.Stock.Indicators;

[..]

public record MyCustomQuote : IQuote
{
    // required base properties
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    // custom properties
    public int MyOtherProperty { get; set; }
}
```

```csharp
// fetch historical quotes from your favorite feed
IReadOnlyList<MyCustomQuote> myQuotes = GetQuotesFromFeed("MSFT");

// example: get 20-period simple moving average
IReadOnlyList<SmaResult> results = myQuotes.ToSma(20);
```

::: warning Custom quotes must have value based equality
When implementing your custom quote type, it must be either `record` class or implement `IEquatable<T>` to be compatible with streaming hubs
:::

## Chaining: indicator of indicators

If you want to compute an indicator of indicators, such as an SMA of an ADX or an [RSI of an OBV](https://medium.com/@robswc/this-is-what-happens-when-you-combine-the-obv-and-rsi-indicators-6616d991773d), use _**chaining**_ to calculate an indicator from prior results.
Example:

```csharp
// fetch historical quotes from your feed (your method)
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("SPY");

// calculate RSI of OBV
IReadOnlyList<RsiResult> results
  = quotes
    .ToObv()
    .ToRsi(14);
```

See [Chaining indicators](/guide/batch#chaining-indicators) for more.

## Candlestick patterns

[Candlestick Patterns](/indicators/Doji) are a unique form of indicator and have a common output model.

The `Match` enum indicates whether a candlestick pattern is recognized, and provides an optional confirmation and basis signal:

| type                  |  int | description                         |
| --------------------- | ---: | ----------------------------------- |
| `Match.BullConfirmed` |  200 | Confirmation of a prior bull signal |
| `Match.BullSignal`    |  100 | Bullish signal                      |
| `Match.BullBasis`     |   10 | Bars supporting a bullish signal    |
| `Match.Neutral`       |    1 | Signal for non-directional patterns |
| `Match.None`          |    0 | No match                            |
| `Match.BearBasis`     |  -10 | Bars supporting a bearish signal    |
| `Match.BearSignal`    | -100 | Bearish signal                      |
| `Match.BearConfirmed` | -200 | Confirmation of a prior bear signal |

## Utilities

See [Utilities and helper functions](/utilities/) for additional tools.
