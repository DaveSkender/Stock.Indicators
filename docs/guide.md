---
title: Guide and Pro tips
permalink: /guide/
relative_path: guide.md
layout: page
redirect_from:
 - /docs/GUIDE.html
---

# {{ page.title }}

- [Installation and setup](#installation-and-setup)
- [Prerequisite data](#prerequisite-data)
- [Example usage](#example-usage)
- [Historical quotes](#historical-quotes)
- [Using custom quote classes](#using-custom-quote-classes)
- [Using custom results classes](#using-custom-results-classes)
- [Generating indicator of indicators](#generating-indicator-of-indicators)
- [Candlestick patterns](#candlestick-patterns)
- [Creating custom indicators]({{site.baseurl}}/custom-indicators/#content)
- [Utilities and Helper functions]({{site.baseurl}}/utilities/#content)
- [Contributing guidelines]({{site.github.repository_url}}/blob/main/docs/contributing.md#readme)

## Getting started

### Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project.  See [more help](https://www.google.com/search?q=install+nuget+package) for installing packages.

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# package manager example
Install-Package Skender.Stock.Indicators
```

### Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You must get historical quotes from your own market data provider.  For clarification, the `GetHistoryFromFeed()` method shown in the example below and throughout our documentation **is not part of this library**, but rather an example to represent your own acquisition of historical quotes.

Historical price data can be provided as an `List`, `IEnumerable`, or `ICollection` of the `Quote` class ([see below](#historical-quotes)); however, it can also be supplied as a generic [custom TQuote type](#using-custom-quote-classes) if you prefer to use your own quote model.

For additional configuration parameters, default values are provided when there is an industry standard.  You can, of course, override these and provide your own values.

### Example usage

All indicator methods will produce all possible results for the provided historical quotes as a time series dataset -- it is not just a single data point returned.  For example, if you provide 3 years worth of historical quotes for the SMA method, you'll get 3 years of SMA result values.

```csharp
using Skender.Stock.Indicators;

[..]

// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA
IEnumerable<SmaResult> results = quotes.GetSma(20);

// use results as needed for your use case (example only)
foreach (SmaResult r in results)
{
    Console.WriteLine($"SMA on {r.Date:d} was ${r.Sma:N4}");
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

See [individual indicator pages]({{site.baseurl}}/indicators/) for specific usage guidance.

More examples available:

- [Example usage code]({{site.baseurl}}/examples/#content) in a simple working console application
- [Demo site](https://stock-charts.azurewebsites.net) (a stock chart)

## Historical quotes

You must provide historical price quotes to the library in the standard OHLCV `IEnumerable<Quote>` or a compatible `List` or `ICollection` format.  It should have a consistent period frequency (day, hour, minute, etc).  See [using custom quote classes](#using-custom-quote-classes) if you prefer to use your own quote class.

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume

### Where can I get historical quote data?

There are many places to get financial market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, see our ongoing [discussion on market data]({{site.github.repository_url}}/discussions/579) for ideas.

### How much historical quote data do I need?

Each indicator will need different amounts of price `quotes` to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, **most use cases will require that you provide more than the minimum**.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).

:warning: IMPORTANT! **Applying the _minimum_ amount of quote history as possible is NOT a good way to optimize your system.**  Some indicators use a smoothing technique that converges to better precision over time.  While you can calculate these with the minimum amount of quote data, the precision to two decimal points often requires 250 or more preceding historical records.

For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results.  Occasionally, even more is required for optimal precision.

See [discussion on warmup and convergence]({{site.github.repository_url}}/discussions/688) for more information.

### Using custom quote classes

If you would like to use your own custom `MyCustomQuote` class, to avoid needing to transpose into the library `Quote` class, you only need to add the `IQuote` interface.

```csharp
using Skender.Stock.Indicators;

[..]

public class MyCustomQuote : IQuote
{
    // required base properties
    public DateTime Date { get; set; }
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
IEnumerable<MyCustomQuote> myQuotes = GetHistoryFromFeed("MSFT");

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = myQuotes.GetSma(20);
```

#### Using custom quote property names

If you have a model that has different properties names, but the same meaning, you only need to map them.  For example, if your class has a property called `CloseDate` instead of `Date`, it could be represented like this:

```csharp
public class MyCustomQuote : IQuote
{
    // required base properties
    DateTime IQuote.Date => CloseDate;
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; set; }
}
```

Note the use of explicit interface (property declaration is `IQuote.Date`), this is because having two properties that expose the same information can be confusing, this way `Date` property is only accessible when working with the included `Quote` type, while if you are working with a `MyCustomQuote` the `Date` property will be hidden, avoiding confusion.

For more information on explicit interfaces, refer to the [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation).

## Using custom results classes

The indicator result classes can be customized in your code.  There are many ways to do this, but the benefit of using derived `ResultBase` is that your custom class will inherit all of the [utility results extension methods]({{site.baseurl}}/utilities/#utilities-for-indicator-results).  Here's one example:

```csharp
// your custom class with an EMA profile
public class MyEma : ResultBase
{
  // my properties
  public int MyId { get; set; }
  public double? Ema { get; set; }
}

public void MyClass(){

  // fetch historical quotes from your feed (your method)
  IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

  // compute indicator
  INumerable<EmaResult> emaResults = quotes.GetEma(14);

  // convert to my Ema class list [using LINQ]
  List<MyEma> myEmaResults = emaResults
    .Select(e => new MyEma
      {
        MyId = 123,
        Date = e.Date,
        Ema = e.Ema
      })
    .ToList();

  // randomly selecting first record from the
  // collection here for the example
  MyEma r = myEmaResults.FirstOrDefault();

  // use your custom quote data
  Console.WriteLine($"On {r.Date}, EMA was {r.Ema} for my EMA ID {r.MyId}.");
}
```

## Generating indicator of indicators

If you want to compute an indicator of indicators, such as an SMA of an ADX or an [RSI of an OBV](https://medium.com/@robswc/this-is-what-happens-when-you-combine-the-obv-and-rsi-indicators-6616d991773d), all you need to do is to take the results of one and chain it to another.  Example:

```csharp
// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetHistoryFromFeed("SPY");

// calculate RSI of OBV
IEnumerable<RsiResult> results
  = quotes
    .GetObv()
    .GetRsi(14);

// or with two separate operations
IEnumerable<ObvResult> obvResults = quotes.GetObv();
IEnumerable<RsiResult> rsiOfObv = obvResults.GetRsi(14);
```

## Candlestick patterns

[Candlestick Patterns]({{site.baseurl}}/indicators/#candlestick-pattern) are a unique form of indicator and have a common output model.

{% include candle-result.md %}

### Match

When a candlestick pattern is recognized, it produces a matching signal.  In some cases, an intrinsic confirmation is also available.  In cases where previous bars were used to identify a pattern, they are indicates as the basis for the signal.  This `enum` can also be referenced as an `int` value.  [Documentation for each candlestick pattern]({{site.baseurl}}/indicators/#candlestick-pattern) will indicate whether confirmation and/or basis information is produced.

| type | int | description
|-- |--: |--
| `Match.BullConfirmed` | 200 | Confirmation of a prior bull signal
| `Match.BullSignal` | 100 | Matching bullish pattern
| `Match.BullBasis` | 10 | Bars supporting a bullish signal
| `Match.Neutral` | 1 | Matching for non-directional patterns
| `Match.None` | 0 | No match
| `Match.BearBasis` | -10 | Bars supporting a bearish signal
| `Match.BearSignal` | -100 | Matching bearish pattern
| `Match.BearConfirmed` | -200 | Confirmation of a prior bear signal

### Candle

The `CandleProperties` class is an extended version of `Quote`, and contains additional calculated properties.

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume
| `Size` | decimal | `High-Low`
| `Body` | decimal | `|Open-Close|`
| `UpperWick` | decimal | Upper wick size
| `LowerWick` | decimal | Lower wick size
| `BodyPct` | double | `Body/Size`
| `UpperWickPct` | double | `UpperWick/Size`
| `LowerWickPct` | double | `LowerWick/Size`
| `IsBullish` | bool | `Close>Open` direction
| `IsBearish` | bool | `Close<Open` direction

## Utilities

See [Utilities and Helper functions]({{site.baseurl}}/utilities/#content) for additional tools.
