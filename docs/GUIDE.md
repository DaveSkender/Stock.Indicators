# Guide and Pro tips

- [Installation and setup](#installation-and-setup)
- [Prerequisite data](#prerequisite-data)
- [Example usage](#example-usage)
- [About historical quotes](#historical-quotes)
- [Using custom quote classes](#using-custom-quote-classes)
- [Validating historical quotes](#validating-historical-quotes)
- [Using derived results classes](#using-derived-results-classes)
- [Generating indicator of indicators](#generating-indicator-of-indicators)
- [Helper functions](#helper-functions)
- [Contributing guidelines](https://daveskender.github.io/Stock.Indicators/docs/CONTRIBUTING.html)

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

You can get historical quotes from your favorite stock data provider.
Historical price data is an `IEnumerable` of the `Quote` class ([see below](#historical-quotes)); however, it can also be supplied as a generic [custom quote type](#using-custom-quote-classes) if you prefer to use your own quote model.

For additional configuration parameters, default values are provided when there is an industry standard.
You can, of course, override these and provide your own values.

### Example usage

All indicator methods will produce all possible results for the provided history -- it is not just a single data point returned.  For example, if you provide 3 years worth of quote history for the SMA method, you'll get 3 years of SMA result values.

```csharp
using Skender.Stock.Indicators;

[..]

// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);

// use results as needed
SmaResult result = results.LastOrDefault();
Console.WriteLine("SMA on {0} was ${1}", result.Date, result.Sma);
```

```bash
SMA on 12/31/2018 was $251.86
```

See [individual indicator pages](INDICATORS.md) for specific usage guidance.

## Historical quotes

Historical price quotes are provided to the library in the standard OHLCV `IEnumerable<Quote>` format and should have a consistent frequency (day, hour, minute, etc).  See [using custom quote classes](#using-custom-quote-classes) if you prefer to use your own quote class.

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume

### Where can I get historical quote data?

There are many places to get stock market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, try [Polygon.io](https://polygon.io), [TwelveData](https://twelvedata.com), or [Alpha Vantage](https://www.alphavantage.co).

### How much historical quote data do I need?

Each indicator will need different amounts of price quote `history` to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, most use cases will require that you provide more than the minimum.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).  A `BadHistoryException` will be thrown if you do not provide sufficient history to produce any results.

:warning: IMPORTANT! Some indicators, especially those that are derived from [Exponential Moving Average](../indicators/Ema/README.md), use a smoothing technique where there is precision convergence over time.  While you can calculate these with the minimum amount of data, the precision to two decimal points often requires 250 or more preceding historical records.

For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results.  Occassionally, even more is required for optimal precision.

### Using custom quote classes

If you would like to use your own custom `MyCustomQuote` _quote_ class, to avoid needing to transpose into the library `Quote` class, you only need to add the `IQuote` interface.

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
IEnumerable<MyCustomQuote> myHistory = GetHistoryFromFeed("MSFT");

// example: get 20-period simple moving average
IEnumerable<SmaResult> results = Indicator.GetSma(myHistory,20);
```

#### Using custom quote property names

If you have a model that has different properties names, but the same meaning, you only need to map them.
Suppose your class has a property called `CloseDate` instead of `Date`, it could be represented like this:

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

### Validating historical quotes

Historical quotes are automatically re-sorted [ascending by date] on every call to the library.  This is needed to ensure that it is sequenced properly.  If you want a more advanced check of your `IEnumerable<TQuote> history` (historical quotes) you can _optionally_ validate it with the `history.Validate()` helper function.  It will check for duplicate dates and other bad data.  This comes at a small performance cost, so we did not automatically add these advanced validations in the indicator methods.  Of course, you can and should do your own validation of `history` prior to using it in this library.  Bad historical quotes data can produce unexpected results.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// advanced validation
IEnumerable<Quote> validatedHistory = history.Validate();
```

## Using derived results classes

The indicator result (e.g. `EmaResult`) classes can be extended in your code.  Here's an example of how you'd set that up:

```csharp
// your custom derived class
public class MyEma : EmaResult
{
  public int MyId { get; set; }
}

public void MyClass(){

  // fetch historical quotes from your favorite feed, in Quote format
  IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

  // compute indicator
  INumerable<EmaResult> emaResults = Indicator.GetEma(history,14);

  // convert to my Ema class list [using LINQ]
  List<MyEma> myEmaResults = emaResults
    .Select(x => new MyEma
      {
        MyId = 123,
        Date = x.Date,
        Ema = x.Ema
      })
    .ToList();

  // randomly selecting first record from the collection here for the example
  MyEma r = myEmaResults.FirstOrDefault();

  // use your custom quote data
  Console.WriteLine("On {0}, EMA was {1} for my EMA ID {2}.",
                     r.Date, r.Ema, r.MyId);
}
```

### Using nested results classes

If you prefer nested classes, here's an alternative method for customizing your results:

```csharp
// your custom nested class
public class MyEma
{
  public int MyId { get; set; }
  public EmaResult Result { get; set; }
}

public void MyClass(){

  // fetch historical quotes from your favorite feed, in Quote format
  IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

  // compute indicator
  INumerable<EmaResult> emaResults = Indicator.GetEma(history,14);

  // convert to my Ema class list [using LINQ]
  List<MyEma> myEmaResults = emaResults
    .Select(x => new MyEma
      {
        MyId = 123,
        Result = x
      })
    .ToList();

  // randomly selecting first record from the collection here for the example
  MyEma r = myEmaResults.FirstOrDefault();

  // use your custom quote data
  Console.WriteLine("On {0}, EMA was {1} for my EMA ID {2}.",
                     r.Result.Date, r.Result.Ema, r.MyId);
}
```

## Generating indicator of indicators

If you want to compute an indicator of indicators, such as an SMA of an ADX or an [RSI of an OBV](https://medium.com/@robswc/this-is-what-happens-when-you-combine-the-obv-and-rsi-indicators-6616d991773d), all you need to do is to take the results of one, reformat into a synthetic quote history, and send it through to another indicator.  Example:

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// calculate OBV
IEnumerable<ObvResult> obvResults = Indicator.GetObv(history);

// convert to synthetic history [using LINQ]
List<Quote> obvHistory = obvResults
  .Where(x => x.Obv != null)
  .Select(x => new Quote
    {
      Date = x.Date,
      Close = x.Obv
    })
  .ToList();

// calculate RSI of OBV
int lookbackPeriod = 14;
IEnumerable<RsiResult> results = Indicator.GetRsi(obvHistory, lookbackPeriod);
```

## Helper functions

### Find indicator result by date

`results.Find()` is a simple lookup for your indicator results collection.  Just specify the date you want returned.

```csharp
// fetch historical quotes from your favorite feed
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate indicator series
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);

// find result on a specific date
DateTime lookupDate = [..] // the date you want to find
SmaResult result = results.Find(lookupDate);
```
