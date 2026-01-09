---
title: Guide and Pro tips
description: Learn how to use the Stock Indicators for .NET Nuget library in your own software tools and platforms.  Whether you're just getting started or an advanced professional, this guide explains how to get setup, example usage code, and instructions on how to use historical price quotes, make custom quote classes, chain indicators of indicators, and create custom technical indicators.
permalink: /guide/
relative_path: pages/guide.md
layout: page
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
  <li><a href="{{site.baseurl}}/custom-indicators/#content">Creating custom indicators</a></li>
  <li><a href="{{site.baseurl}}/utilities/#content">Utilities and helper functions</a></li>
  <li><a href="{{site.baseurl}}/contributing/#content">Contributing guidelines</a></li>
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

### Implementation pattern

Each [indicator style](#indicator-styles-and-features) available (series, buffer list, and stream hub) will have a slightly different [implementation syntax](#example-usage); however, all will follow a common overall pattern.

```csharp
using Skender.Stock.Indicators;

[..]

// step 1: get quote(s) from your source
// step 2: calculate indicator value(s)
```

See [usage examples](#example-usage) for additional details.

## Indicator styles and features

This library has three indicator styles available to support different uses cases.

| style        | use case                                     | best for                       |
| ------------ | -------------------------------------------- | ------------------------------ |
| Series batch | convert full quote collections to indicators | once-and-done bulk conversions |
| Buffer lists | standalone incrementing `ICollection` lists  | self-managed incrementing data |
| Stream hub   | subscription based hub-observer pattern      | streaming or live data sources |

### Feature comparison

| feature        | Series batch    | Buffer lists  | Stream hub   |
| -------------- | --------------- | ------------- | ------------ |
| incrementing   | no              | yes           | yes          |
| batch speed    | fastest         | faster        | fast         |
| scaling        | low             | moderate      | high         |
| class type     | static          | instance      | instance     |
| base interface | `IReadOnlyList` | `ICollection` | `IStreamHub` |
| complexity     | lowest          | moderate      | highest      |
| chainable      | yes             | yes           | yes          |
| pruning        | with utility    | auto-preset   | auto-preset  |

### Example usage

#### Series (batch) style usage example

All series-style indicators will produce all possible results for the provided historical quotes as a time series dataset -- it is not just a single data point returned.  For example, if you provide 3 years worth of historical quotes for the SMA method, you'll get 3 years of SMA result values.

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

#### Buffer list style usage example

Buffer list style indicators maintain incremental state as you add new data points. This is ideal for scenarios where you're building up historical data over time or processing data incrementally without needing a full hub infrastructure.

```csharp
using Skender.Stock.Indicators;

[..]

// create buffer list with lookback period
SmaList smaList = new(20);

// add quotes incrementally (from your data source)
foreach (IQuote quote in quotes)  // simulating stream
{
    smaList.Add(quote);
}

// access results as ICollection
IReadOnlyList<SmaResult> results = smaList;

// or get the latest result
SmaResult latest = smaList.LastOrDefault();
```

**Key features:**

- Implements `ICollection<TResult>` for standard collection operations
- Automatically manages internal buffers for efficient calculations
- Supports `.Add()` for individual quotes or `.Add(IReadOnlyList)` for batches
- Auto-prunes results when exceeding `MaxListSize` (default ~1.9B elements)
- Can be cleared and reused with `.Clear()`

#### Stream hub style usage example

Stream hub style uses the observer pattern where multiple indicators can subscribe to a central `QuoteHub`. This provides coordinated real-time updates for live data feeds and WebSocket integration.

```csharp
using Skender.Stock.Indicators;

[..]

// create quote hub and subscribe indicators
QuoteHub<Quote> quoteHub = new();
SmaHub<Quote> smaHub = quoteHub.ToSma(20);
RsiHub<Quote> rsiHub = quoteHub.ToRsi(14);
MacdHub<Quote> macdHub = quoteHub.ToMacd();

// stream quotes as they arrive
foreach (Quote quote in liveQuotes)
{
    // single update propagates to all observers
    quoteHub.Add(quote);
    
    // access latest results from each indicator
    SmaResult sma = smaHub.Results.LastOrDefault();
    RsiResult rsi = rsiHub.Results.LastOrDefault();
    MacdResult macd = macdHub.Results.LastOrDefault();
    
    // use results for trading logic, alerts, etc.
}
```

**Key features:**

- Observable pattern with hub-observer architecture
- Single quote update propagates to all subscribed indicators
- Supports state management and rollback for late-arriving data
- Indicators can be chained: `quoteHub.ToEma(20).ToRsi(14)`
- Optimized for low-latency real-time scenarios
- Results accessible via `.Results` property

See [individual indicator pages]({{site.baseurl}}/indicators/) for specific usage guidance.

More examples available:

- [Example usage code]({{site.baseurl}}/examples/#content) in a simple working console application
- [Demo site](https://charts.stockindicators.dev) (a stock chart)

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

There are many places to get financial market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, see our ongoing [discussion on market data]({{site.github.repository_url}}/discussions/579) for ideas.

### How much historical quote data do I need?

Each indicator will need different amounts of price `quotes` to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, **most use cases will require that you provide more than the minimum**.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).

> &#128681; **IMPORTANT! Applying the _minimum_ amount of quote history as possible is NOT a good way to optimize your system.**  Some indicators use a smoothing technique that converges to better precision over time.  While you can calculate these with the minimum amount of quote data, the precision to two decimal points often requires 250 or more preceding historical records.
>
> For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results.  Occasionally, even more is required for optimal precision.
>
> See [discussion on warmup and convergence]({{site.github.repository_url}}/discussions/688) for more information.

### Using custom quote classes

If you would like to use your own custom `MyCustomQuote` class, to avoid needing to transpose into the library `Quote` class, you only need to add the `IQuote` interface.

```csharp
using Skender.Stock.Indicators;

[..]

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
}
```

```csharp
// fetch historical quotes from your favorite feed
IReadOnlyList<MyCustomQuote> myQuotes = GetQuotesFromFeed("MSFT");

// example: get 20-period simple moving average
IReadOnlyList<SmaResult> results = myQuotes.ToSma(20);
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
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    decimal IQuote.Volume => Vol;

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
    .ToObv()
    .ToRsi(14);

// or with two separate operations
IReadOnlyList<ObvResult> obvResults = quotes.ToObv();
IReadOnlyList<RsiResult> rsiOfObv = obvResults.ToRsi(14);
```

## Candlestick patterns

[Candlestick Patterns]({{site.baseurl}}/indicators/#candlestick-pattern) are a unique form of indicator and have a common output model.

{% include candle-result.md %}

### Match

When a candlestick pattern is recognized, it produces a matching signal.  In some cases, an intrinsic confirmation is also available after the signal.  In cases where previous bars were used to identify a pattern, they are indicated as the basis for the signal.  This `enum` can also be referenced as an `int` value.  [Documentation for each candlestick pattern]({{site.baseurl}}/indicators/#candlestick-pattern) will indicate whether confirmation and/or basis information is produced.

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

### Candle

The `CandleProperties` class is an extended version of `Quote`, and contains additional calculated properties.  `TQuote` classes can be converted to `CandleProperties` with the `.ToCandle()` [utility]({{site.baseurl}}/utilities/#extended-candle-properties), and further used as the basis for calculating indicators.

{% include candle-properties.md %}

## Incremental buffer style indicators

Buffer list style indicators provide efficient incremental processing for growing datasets. Use this style when you need to add data points one at a time without the overhead of a full hub infrastructure.

### When to use Buffer lists

**Ideal for:**

- Building up historical data incrementally
- Processing data feeds where quotes arrive sequentially
- Self-managed incremental calculations
- Scenarios where you don't need multi-indicator coordination
- Memory-efficient processing with auto-pruning

**Not ideal for:**

- Complete historical datasets (use Series style instead)
- Multiple indicators needing coordinated updates (use StreamHub instead)
- One-time batch calculations (use Series style instead)

### Buffer list implementation pattern

```csharp
// Create buffer list with parameters
{IndicatorName}List indicatorList = new(lookbackPeriods);

// Add quotes incrementally
foreach (IQuote quote in quotes)
{
    indicatorList.Add(quote);
}

// Access results as ICollection
IReadOnlyList<{IndicatorName}Result> results = indicatorList;

// Or get latest value
{IndicatorName}Result latest = indicatorList.LastOrDefault();

// Clear and reuse if needed
indicatorList.Clear();
```

### Memory management

Buffer lists automatically manage memory with the `MaxListSize` property (default ~1.9B elements). When the list exceeds this size, older results are automatically pruned. You can customize this behavior:

```csharp
SmaList smaList = new(20)
{
    MaxListSize = 1000  // Keep only last 1000 results
};
```

### Buffer list performance characteristics

- **Overhead**: ~10-20% slower than Series style for the same dataset
- **Memory**: Maintains internal buffers for lookback periods
- **Latency**: Optimized for incremental updates, O(1) or O(log n) per quote

See individual indicator documentation for specific examples.

## Streaming hub style indicators

Stream hub style provides real-time processing with observable patterns and state management. Multiple indicators can subscribe to a single `QuoteHub` for coordinated updates.

### When to use Stream hubs

**Ideal for:**

- Live data feeds and WebSocket integration
- Multiple indicators requiring synchronized updates
- Trading applications with low-latency requirements
- Real-time dashboards and monitoring
- Complex event-driven architectures

**Not ideal for:**

- One-time historical analysis (use Series style instead)
- Simple incremental processing (use Buffer lists instead)
- Scenarios without real-time requirements

### Stream hub implementation pattern

```csharp
// Create quote hub
QuoteHub<Quote> quoteHub = new();

// Subscribe indicators (observers)
{IndicatorName}Hub<Quote> hub1 = quoteHub.To{IndicatorName}(params);
{IndicatorName}Hub<Quote> hub2 = quoteHub.To{IndicatorName}(params);

// Stream quotes
foreach (Quote quote in liveQuotes)
{
    quoteHub.Add(quote);  // Propagates to all observers
    
    // Access results
    var result1 = hub1.Results.LastOrDefault();
    var result2 = hub2.Results.LastOrDefault();
}
```

### Chaining indicators

Stream hubs support indicator chaining for derived indicators:

```csharp
QuoteHub<Quote> quoteHub = new();

// Chain RSI from EMA
EmaHub<Quote> emaHub = quoteHub.ToEma(20);
RsiHub<Quote> rsiHub = emaHub.ToRsi(14);  // RSI of EMA

// Or chain directly
RsiHub<Quote> rsiOfEma = quoteHub.ToEma(20).ToRsi(14);
```

### State management and rollback

Stream hubs support late-arriving data and corrections:

```csharp
QuoteHub<Quote> quoteHub = new();
SmaHub<Quote> smaHub = quoteHub.ToSma(20);

// Add quotes
quoteHub.Add(quote1);
quoteHub.Add(quote2);

// Late-arriving data with earlier timestamp
quoteHub.Insert(lateQuote);  // Triggers recalculation

// Remove incorrect quote
quoteHub.Remove(badQuote);   // Triggers recalculation
```

### Stream hub performance characteristics

- **Overhead**: ~20-30% slower than Series style for the same dataset
- **Memory**: Maintains cache and state for all subscribed indicators
- **Latency**: Optimized for real-time per-quote updates, typically <1ms per quote
- **Scalability**: Supports multiple concurrent observers with single propagation

See individual indicator documentation for specific streaming examples.

## Utilities

See [Utilities and helper functions]({{site.baseurl}}/utilities/#content) for additional tools.
