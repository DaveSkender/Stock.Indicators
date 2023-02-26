---
title: Custom indicators
description: The Stock Indicators for .NET library includes extra features to make it easy for you to extend and to add your own custom indicators.  Here's an example.
permalink: /custom-indicators/
relative_path: examples/CustomIndicators/README.md
layout: page
---

# Custom indicators

At some point in your journey, you may want to create your own custom indicators.
The following is an example of how you'd create your own.
This example is also included in our [example usage code]({{site.baseurl}}/examples/#content).

## STEP 1: Create the Results class

Create your results class by extending the library `ResultBase` class.  This will allow you to inherit many of the [utility functions]({{site.baseurl}}/utilities/#utilities-for-indicator-results), such as `RemoveWarmupPeriods()`.

```csharp
using Skender.Stock.Indicators;
namespace Custom.Stock.Indicators;

// custom results class
public class AtrWmaResult : ResultBase, IReusableResult
{
  // date property is inherited here,
  // so you only need to add custom items
  public double? AtrWma { get; set; }

  // to enable further chaining
  double? IReusableResult.Value => AtrWma;
}
```

## STEP 2: Create your custom indicator

Create your custom indicator algorithm in the same style as our main library so the API functions identically.

```csharp
using System.Collections.ObjectModel;
using Skender.Stock.Indicators;
namespace Custom.Stock.Indicators;

public static class CustomIndicator
{
  // Custom ATR WMA calculation
  public static IEnumerable<AtrWmaResult> GetAtrWma<TQuote>(
    this IEnumerable<TQuote> quotes,
    int lookbackPeriods)
    where TQuote : IQuote
  {
    // sort quotes and convert to collection or list
    Collection<TQuote> quotesList = quotes
      .ToSortedCollection();

    // initialize results
    List<AtrWmaResult> results = new(quotesList.Count);

    // perform pre-requisite calculations to get ATR values
    List<AtrResult> atrResults = quotes
      .GetAtr(lookbackPeriods)
      .ToList();

    // roll through quotes
    for (int i = 0; i < quotesList.Count; i++)
    {
      TQuote q = quotesList[i];

      AtrWmaResult r = new()
      {
        Date = q.Date
      };

      // only do calculations after incalculable periods
      if (i >= lookbackPeriods - 1)
      {
        double? sumWma = 0;
        double? sumAtr = 0;

        for (int p = i - lookbackPeriods + 1; p <= i; p++)
        {
          double close = (double)quotesList[p].Close;
          double? atr = atrResults[p]?.Atr;

          sumWma += atr * close;
          sumAtr += atr;
        }

        r.AtrWma = sumWma / sumAtr;
      }

      // add record to results
      results.Add(r);
    }

    return results;
  }
}
```

## STEP 3: Use your indicator just like the others

```csharp
using Skender.Stock.Indicators;
using Custom.Stock.Indicators; // your custom library

[..]

// fetch historical quotes from your feed (your method)
IEnumerable<Quote> quotes = GetQuotesFromFeed("MSFT");

// calculate 10-period ATR WMA
IEnumerable<AtrWmaResult> results = quotes.GetAtrWma(10);

// use results as needed for your use case (example only)
foreach (AtrWmaResult r in results)
{
  Console.WriteLine($"ATR WMA on {r.Date:d} was ${r.AtrWma:N4}");
}
```

```console
ATR WMA on 4/19/2018 was $255.0590
ATR WMA on 4/20/2018 was $255.2015
ATR WMA on 4/23/2018 was $255.6135
ATR WMA on 4/24/2018 was $255.5105
ATR WMA on 4/25/2018 was $255.6570
ATR WMA on 4/26/2018 was $255.9705
..
```
