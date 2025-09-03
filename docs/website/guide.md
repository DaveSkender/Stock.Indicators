# Guide and Pro Tips

This guide explains how to use the Stock Indicators for .NET NuGet library in your own software tools and platforms. Whether you're just getting started or an advanced professional, learn about setup, example usage code, and instructions on how to use historical price quotes, make custom quote classes, chain indicators of indicators, and create custom technical indicators.

- [Installation and setup](#installation-and-setup)
- [Prerequisite data](#prerequisite-data)
- [Example usage](#example-usage)
- [Historical quotes](#historical-quotes)
- [Using custom quote classes](#using-custom-quote-classes)
- [Using custom results classes](#using-custom-results-classes)
- [Generating indicator of indicators](#generating-indicator-of-indicators)
- [Candlestick patterns](#candlestick-patterns)
- [Creating custom indicators](#creating-custom-indicators)
- [Utilities and helper functions](utilities.md)
- [Contributing guidelines](contributing.md)

## Getting started

### Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project. See more [help for installing packages](https://www.google.com/search?q=install+nuget+package).

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# package manager example
Install-Package Skender.Stock.Indicators
```

### Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You must get historical quotes from your own market data provider. For clarification, the `GetQuotesFromFeed()` method shown in the example below **is not part of this library**, but rather an example to represent your own acquisition of historical quotes.

Historical price data can be provided as a `List`, `IEnumerable`, or `ICollection` of the `Quote` class ([see below](#historical-quotes)); however, it can also be supplied as a generic [custom TQuote type](#using-custom-quote-classes) if you prefer to use your own quote model.

For additional configuration parameters, default values are provided when there is an industry standard. You can, of course, override these and provide your own values.

### Example usage

```csharp
using Skender.Stock.Indicators;

// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetQuotesFromFeed("SPY");

// calculate 20-period SMA
IEnumerable<SmaResult> results = quotes.GetSma(20);

// use results as needed
SmaResult result = results.LastOrDefault();
Console.WriteLine("SMA on {0} was ${1}", result.Date, result.Sma);
```

## Historical quotes

Quote data must be a collection of artifacts that include date, open, high, low, close, and volume elements. You can use the included `Quote` class or [provide your own custom quote class](#using-custom-quote-classes).

For additional details on supported formats, see the [Skender.Stock.Indicators package documentation](https://www.nuget.org/packages/Skender.Stock.Indicators).

## Using custom quote classes

You can use custom quote classes that have different field names or modified data. You must provide adapters for the custom quote to work.

For detailed examples on implementing custom quote types, refer to the code examples in the [documentation](https://www.nuget.org/packages/Skender.Stock.Indicators).

## Using custom results classes

Similar to custom quote classes, you can also create custom result classes for your specific needs.

## Generating indicator of indicators

You can generate "indicator of indicators" (e.g. get the Stochastic RSI of the RSI) easily with this library.

For example, to get a 14-period Stochastic RSI of the RSI (a type of 2nd derivative), you would:

```csharp
// first, get regular RSI
var rsiResults = quotes.GetRsi(14);

// second, convert RSI results into quotes
// this could be your own custom mapping function
var rsiQuotes = rsiResults
                 .Select(x => 
                    new Quote
                    {
                        Date = x.Date,
                        Close = x.Rsi
                    });

// third, get the Stochastic RSI using the RSI quotes
var stochRsi = rsiQuotes.GetStoch(14, 3, 3);
```

## Candlestick patterns

The library includes candlestick pattern recognition, such as Doji, Marubozu, and others.

For details on how to use these patterns, see the [Candlestick Patterns documentation](https://www.nuget.org/packages/Skender.Stock.Indicators).

## Creating custom indicators

You can create your own custom technical indicators that are compatible with this library. This allows you to extend the library's functionality with your own proprietary or specialized indicators.

For detailed examples on creating custom indicators, refer to the [Custom Indicators documentation](https://www.nuget.org/packages/Skender.Stock.Indicators).
