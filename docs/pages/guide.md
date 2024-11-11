---
title: Guide and Pro tips
description: Learn how to use the Stock Indicators for .NET Nuget library in your own software tools and platforms.  Whether you're just getting started or an advanced professional, this guide explains how to get setup, example usage code, and instructions on how to use historical price quotes, make custom quote classes, chain indicators of indicators, and create custom technical indicators.
permalink: /guide.md
relative_path: guide.md
---

# {{ page.title }}

<nav role="navigation" aria-label="guide page menu">
<ul class="pipe-list">
  <li><a href="#installation-and-setup">Installation and setup</a></li>
  <li><a href="#prerequisite-data">Prerequisite data</a></li>
  <li><a href="#example-usage">Example usage</a></li>
  <li><a href="#historical-quotes">Historical quotes</a></li>
  <li><a href="#using-custom-quote-classes">Using custom quote classes</a></li>
  <li><a href="#generating-indicator-of-indicators">Generating indicator of indicators</a></li>
  <li><a href="#candlestick-patterns">Candlestick patterns</a></li>
  <li><a href="../examples/CustomIndicators/README.md">Creating custom indicators</a></li>
  <li><a href="utilities.md">Utilities and helper functions</a></li>
  <li><a href="../contributing.md">Contributing guidelines</a></li>
</ul>
</nav>

## Getting started

### Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project.  See more [help for installing packages](https://www.google.com/search?q=install+nuget+package).

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# package manager example
Install-Package Skender.Stock.Indicators
```

### Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You must get historical quotes from your own market data provider.  For clarification, the `GetQuotesFromFeed()` method shown in the example below **is not part of this library**, but rather an example to represent your own acquisition of historical quotes.

Historical price data can be provided as a `List`, `IReadOnlyList`, or `ICollection` of the `Quote` class ([see below](#historical-quotes)); however, it can also be supplied as a generic [custom TQuote type](#using-custom-quote-classes) if you prefer to use your own quote model.

For additional configuration parameters, default values are provided when there is an industry standard.  You can, of course, override these and provide your own values.

### Example usage

All indicator methods will produce all possible results for the provided historical quotes as a time series dataset -- it is not just a single data point returned.  For example, if you provide 3 years worth of historical quotes for the SMA method, you'll get 3 years of SMA result values.

```csharp
using Skender.Stock.Indicators;

[..]

// fetch historical quotes from your feed (your method)
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("MSFT");

// calculate 20-period SMA
IReadOnlyList<SmaResult> results = quotes
  .GetSma(20);

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

See [individual indicator pages](/indicators/) for specific usage guidance.

More examples available:

- [Example usage code](../examples/README.md) in a simple working console application
- [Demo site](https://charts.stockindicators.dev) (a stock chart)

## Historical quotes

You must provide historical price quotes to the library in the standard OHLCV `IReadOnlyList<Quote>` or a compatible `List` or `ICollection` format.  It should have a consistent period frequency (day, hour, minute, etc).  See [using custom quote classes](#using-custom-quote-classes) if you prefer to use your own quote class.

| name | type | notes
| -- |-- |--
| `Timestamp` | DateTime | Close date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume

### Where can I get historical quote data?

There are many places to get financial market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, see our ongoing [discussion on market data](https://github.com/DaveSkender/Stock.Indicators/discussions/579) for ideas.

### How much historical quote data do I need?

Each indicator will need different amounts of price `quotes` to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, **most use cases will require that you provide more than the minimum**.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).

> &#128681; **IMPORTANT! Applying the _minimum_ amount of quote history as possible is NOT a good way to optimize your system.**  Some indicators use a smoothing technique that converges to better precision over time.  While you can calculate these with the minimum amount of quote data, the precision to two decimal points often requires 250 or more preceding historical records.
>
> For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results.  Occasionally, even more is required for optimal precision.
>
> See [discussion on warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) for more information.

### Using custom quote classes

If you would like to use your own custom `MyCustomQuote` class, to avoid needing to transpose into the built-in library `Quote` class, you only need to add the `IQuote` interface and ensure that you've implemented a correct and compatible quote `record` or class.

> &#128681; **IMPORTANT!**
> Your custom quote class needs to be [equatable using property values](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type).  Since this can be complicated to setup, we've provided the shown `EquatableQuote<TQuote>` base class.  You can exclude this base and write your own `IEquatable<IQuote>` interface by only implementing the `IQuote` interface; however, if you do not define it fully with `==` and `!=` operator overrides correctly, it may cause problems with streaming overflow handling.  **We recommend using the equatable `record` class** type for your custom quote class.

```csharp
using Skender.Stock.Indicators;

/// EASY METHOD (use record class)
public record class MyCustomQuote : IQuote
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

    // required mapping method for equality
    public bool Equals(IQuote? other)
        => base.Equals(other);
}
```

```csharp
/// EASY METHOD (use our base equatable class)
public class MyCustomQuote : EquatableQuote<MyCustomQuote>, IQuote
{
    // required inherited base properties do not need to be redefined,
    // however, if you prefer to explicitly define for clarity,
    // use the override keyword (optional)

    public override DateTime Timestamp { get; set; }
    public override decimal Open { get; set; }
    public override decimal High { get; set; }
    public override decimal Low { get; set; }
    public override decimal Close { get; set; }
    public override decimal Volume { get; set; }

    // custom properties
    public int MyOtherProperty { get; set; }
}
```

```csharp
/// HARD METHOD (define your own equatable overrides)
public class MyCustomQuote : IQuote
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

    // equatable overrides
    public override bool Equals(object? obj) => this.Equals(obj);
    public bool Equals(IQuote? other) => this.Equals(other);
    public bool Equals(MyCustomQuote? other) { ... }
    public static bool operator ==( ... ) { ... }
    public static bool operator !=( ... ) { ... }
    public override int GetHashCode()
      => HashCode.Combine(
         Timestamp, Open, High, Low, Close, Volume);
}
```

```csharp
// USAGE
// fetch historical quotes from your favorite feed
IReadOnlyList<MyCustomQuote> myQuotes = GetQuotesFromFeed("MSFT");

// example: get 20-period simple moving average
IReadOnlyList<SmaResult> results = myQuotes.GetSma(20);
```

#### Using custom quote property names

If you have a model that has different properties names, but the same meaning, you only need to map them.  For example, if your class has a property called `CloseDate` instead of `Timestamp`, it could be represented like this:

```csharp
// if using record type
public record class MyCustomQuote : IQuote
{
    // redirect required base properties
    // with your custom properties
    public DateTime Timestamp => CloseDate;
    public decimal Volume => Vol;

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal Vol { get; set; }
}
```

```csharp
// if using inherited equatable class type
public class MyCustomQuote : EquatableQuote<MyCustomQuote>, IQuote
{
    // override inherited, required base properties
    // with your custom properties
    public override DateTime Timestamp => CloseDate;
    public override decimal Volume => Vol;

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal Vol { get; set; }
}
```

Note the use of explicit interface (property declaration is `ISeries.Timestamp`), this is because having two properties that expose the same information can be confusing, this way `Timestamp` property is only accessible when working with the included `Quote` type, while if you are working with a `MyCustomQuote` the `Timestamp` property will be hidden, avoiding confusion.

For more information on explicit interfaces, refer to the [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation).

## Generating indicator of indicators

If you want to compute an indicator of indicators, such as an SMA of an ADX or an [RSI of an OBV](https://medium.com/@robswc/this-is-what-happens-when-you-combine-the-obv-and-rsi-indicators-6616d991773d), use _**chaining**_ to calculate an indicator from prior results.
Example:

```csharp
// fetch historical quotes from your feed (your method)
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("SPY");

// calculate RSI of OBV
IReadOnlyList<RsiResult> results
  = quotes
    .GetObv()
    .GetRsi(14);

// or with two separate operations
IReadOnlyList<ObvResult> obvResults = quotes.GetObv();
IReadOnlyList<RsiResult> rsiOfObv = obvResults.GetRsi(14);
```

## Candlestick patterns

[Candlestick Patterns](/indicators/#candlestick-pattern) are a unique form of indicator and have a common output model.

{% include candle-result.md %}

### Match

When a candlestick pattern is recognized, it produces a matching signal.  In some cases, an intrinsic confirmation is also available after the signal.  In cases where previous bars were used to identify a pattern, they are indicated as the basis for the signal.  This `enum` can also be referenced as an `int` value.  [Documentation for each candlestick pattern](/indicators/#candlestick-pattern) will indicate whether confirmation and/or basis information is produced.

| type | int | description
|-- |--: |--
| `Match.BullConfirmed` | 200 | Confirmation of a prior bull signal
| `Match.BullSignal` | 100 | Bullish signal
| `Match.BullBasis` | 10 | Bars supporting a bullish signal
| `Match.Neutral` | 1 | Signal for non-directional patterns
| `Match.None` | 0 | No match
| `Match.BearBasis` | -10 | Bars supporting a bearish signal
| `Match.BearSignal` | -100 | Bearish signal
| `Match.BearConfirmed` | -200 | Confirmation of a prior bear signal

### Candle

The `CandleProperties` class is an extended version of `Quote`, and contains additional calculated properties.  `TQuote` classes can be converted to `CandleProperties` with the `.ToCandle()` [utility](utilities.md#extended-candle-properties), and further used as the basis for calculating indicators.

{% include candle-properties.md %}

## Utilities

See [Utilities and helper functions](utilities.md) for additional tools.
