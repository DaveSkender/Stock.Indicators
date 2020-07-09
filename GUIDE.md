# Guide and Pro Tips

- [Example usage](#example-usage)
- [Using the Quote class](#quote)
- [Cleaning history(optional)](#cleaning-history)
- [Using derived classes](#using-derived-classes)

## Example usage

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

See individual indicator pages for specific guidance.

## Quote

Historical quotes should be of consistent time frequency (e.g. per minute, hour, day, etc.).

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | long | Volume

There is also a public read-only `Index` property in this class that is set internally, so **do not try to set the Index value**.  We set this `Index` property to `public` visibility in case you want to use it in your own wrapper code.  See [Cleaning History](#cleaning-history)) section below if you want to pre-clean the history and get `Index` values in your `IEnumerable<Quote> history` data (optional).  You can also derive and extend classes (optional), see the [Using Derived Classes](#using-derived-classes) section below.

## Cleaning history

If you intend to use the same composed `IEnumerable<Quote> history` in multiple calls and want to have the same `Index` values mapped into results, we recommend you pre-clean it to initialize those index values.  This will add the `Index` value and sort by the `Date` provided; it will also perform basic checks for data quality.

You only need to do this if you want to use the `Index` value in your own wrapper software; otherwise, there is no need as `history` is cleaned on every call, internally.  If you pre-clean, the provided `history` will be used as-is without additional cleaning.  The original `Date` and composed `Index` is always returned in resultsets.

```csharp
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// pre-clean
history = Cleaners.PrepareHistory(history);
```

## Using derived classes

The `Quote` and indicator result (e.g. `EmaResult`) classes can be extended in your code; however, the `Index` field `set` accessor is protected since it must be controlled by the library to ensure proper indicator calculations.  This adds a bit of complexity to deriving classes in your project due to accessibility rules; however, using the `new` keyword to reset the `Index` will allow you to continue using `Index` computed values.  You'd only need to do this in cases where you are extending and adding properties.  Here's an example of how you'd set that up:

```csharp
// your custom derived class (note the "new" keyword for Index)
public class Ema : EmaResult
{
  public new int Index { get; set; }
  public int MyId { get; set; }
}

public void MyClass(){

  // fetch historical quotes from your favorite feed, in Quote format
  IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

  // pre-clean
  history = Cleaners.PrepareHistory(history);

  // compute indicator
  INumerable<EmaResult> emaResults = Indicator.GetEma(history,14);

  // convert to my Ema class list [using LINQ]
  List<Ema> myEmaResults = emaResults
    .Select(x => new Ema
      {
        Index = (int)x.Index,
        Date = x.Date,
        Ema = x.Ema,
        MyId = 123
      })
    .ToList();

  // randomly selecting first record from the collection here for the example
  Ema r = myEmaResults.FirstOrDefault();

  // use your custom quote data
  Console.WriteLine("Index was {0} on {1} with EMA value of {2} for my EMA ID {3}.",
                     r.Index, r.Date, r.Ema, r.MyId);
}
```
