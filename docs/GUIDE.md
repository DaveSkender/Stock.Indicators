<!-- markdownlint-disable MD026 -->
# Guide and Pro tips

- [Prerequisite data](#prerequisite-data)
- [Example usage](#example-usage)
- [About historical quotes](#quote)
- [Validating history](#validating-history)
- [Using derived classes](#using-derived-classes)
- [Generating indicator of indicators](#generating-indicator-of-indicators)
- [Contributing guidelines](CONTRIBUTING.md)

## Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You can get historical quotes from your favorite stock data provider.
Historical data is an `IEnumerable` of the `Quote` class ([see below](#quote)).

For additional configuration parameters, default values are provided when there is an industry standard.
You can, of course, override these and provide your own values.

## Example usage

All indicator methods will produce all possible results for the provided history -- it is not just a single data point returned.  For example, if you provide 3 years worth of quote history for the SMA method, you'll get 3 years of SMA result values.

```csharp
using Skender.Stock.Indicators;

[..]

// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("MSFT");

// calculate 20-period SMA
IEnumerable<SmaResult> results = Indicator.GetSma(history,20);

// use results as needed
DateTime evalDate = DateTime.Parse("12/31/2018");
SmaResult result = results.Where(x=>x.Date==evalDate).FirstOrDefault();
Console.WriteLine("SMA on {0} was ${1}", result.Date, result.Sma);
```

```bash
SMA on 12/31/2018 was $251.86
```

See [individual indicator pages](INDICATORS.md) for specific guidance.

## Quote

Historical quotes should be of consistent time frequency (e.g. per minute, hour, day, etc.).

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume

More information:

- See [Validating history](#validating-history) to learn about optional pre-validation your historical data with the `ValidateHistory` helper function.  The indicator methods will re-sort your historical quotes, but it will not check for duplicates and other bad data.  This function will do that, but it is optional as it can impede performance if used in higher frequency applications.
- See [Using derived classes](#using-derived-classes) to learn how you can extend these classes.

### Where can I get historical quote data?

There are many places to get stock market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, try [TwelveData](https://twelvedata.com) or [Alpha Vantage](https://www.alphavantage.co).

### How much historical quote data do I need?

Each indicator will need different amounts to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, most use cases will require that you provide more than the minimum.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).  A `BadHistoryException` will be thrown if you do not provide enough history.

Note that some indicators, especially those that are derived from [Exponential Moving Average](../indicators/Ema/README.md), use a smoothing technique where there is convergence over time.  While you can calculate these with the minimum amount of data, the precision to two decimal points often requires 250 or more preceding historical records.

For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of total historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results.

## Validating history

Historical quotes are automatically re-sorted [ascending by date] on every call to the library.  This is needed to ensure that it is sequenced properly.  If you want to do a more advanced check of your `IEnumerable<Quote> history` (historical quotes) you can validate it with the helper cleaner, to check for duplicate dates and other bad data.  This comes at a small performance cost, so we did not automatically include it.

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// advanced cleaning
List<Quote> validatedHistory = Cleaners.ValidateHistory(history);
```

## Using derived classes

The `Quote` and indicator result (e.g. `EmaResult`) classes can be extended in your code.  Here's an example of how you'd set that up:

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
