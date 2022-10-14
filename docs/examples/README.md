---
title: Example usage code
description: The Stock Indicators for .NET library is a simple yet flexible tool to help you build your financial market systems.  Here's a few complete working examples that you can download and try yourself.
permalink: /examples/
relative_path: examples/README.md
layout: page
---

# {{ page.title }}

To help you get started, here are a few minimalist .NET 6.0 C# projects that you can review.  They are complete working examples.

- `ConsoleApp` is a minimalist example of how to use the library (start here)
- `Backtest` is a slightly more complicated example of how to analyze results
- `CustomIndicatorsLibrary` shows how you can [create your own custom indicators]({{site.baseurl}}/custom-indicators/#content)
- `CustomIndicatorsUsage` shows how you'd use a custom indicator just like any other in the main library

For more information on how to use this library overall, see the [Guide and Pro Tips]({{site.baseurl}}/guide/#content).

To run the sample projects:

1. [Download the ZIP file](Skender.Stock.Indicators-Examples.zip) and extract contents
2. Open `Examples.sln` in [Visual Studio](https://visualstudio.microsoft.com)
3. Review the code in the `Program.cs` file
4. Run the `ConsoleApp` by any one of the following methods:
   - pressing the `CTRL+F5` key
   - execute `dotnet run` CLI command in the `ConsoleApp` folder
   - clicking the play button

     ![how to execute the code](run.png)

## Backtest example

``` csharp
/* This is a basic 20-year backtest-style analysis of
 * Stochastic RSI.  It will buy-to-open (BTO) one share
 * when the Stoch RSI (%K) is below 20 and crosses over the 
 * Signal (%D). The reverse Sell-to-Close (STC) and 
 * Sell-To-Open (STO) occurs when the Stoch RSI is above 80 and
 * crosses below the Signal.
 * 
 * As a result, there will always be one open LONG or SHORT
 * position that is opened and closed at signal crossover
 * points in the overbought and oversold regions of the indicator.
 */

// fetch historical quotes from data provider
List<Quote> quotesList = GetHistoryFromFeed()
     .ToList();

// calculate Stochastic RSI
List<StochRsiResult> resultsList =
     quotesList
        .GetStochRsi(14, 14, 3, 1)
        .ToList();

// initialize
decimal trdPrice = 0;
decimal trdQty = 0;
decimal rlzGain = 0;

Console.WriteLine("   Date         Close  StRSI Signal  Cross    Net Gains");
Console.WriteLine("-------------------------------------------------------");

// roll through history
for (int i = 1; i < quotesList.Count; i++)
{
  Quote q = quotesList[i];
  StochRsiResult e = resultsList[i];     // evaluation period
  StochRsiResult l = resultsList[i - 1]; // last (prior) period
  string cross = string.Empty;

  // unrealized gain on open trade
  decimal trdGain = trdQty * (q.Close - trdPrice);

  // check for LONG event
  // condition: Stoch RSI was <= 20 and Stoch RSI crosses over Signal
  if (l.StochRsi <= 20
   && l.StochRsi < l.Signal
   && e.StochRsi >= e.Signal
   && trdQty != 1)
  {
    // emulates BTC + BTO
    rlzGain += trdGain;
    trdQty = 1;
    trdPrice = q.Close;
    cross = "LONG";
  }

  // check for SHORT event
  // condition: Stoch RSI was >= 80 and Stoch RSI crosses under Signal
  if (l.StochRsi >= 80
   && l.StochRsi > l.Signal
   && e.StochRsi <= e.Signal
   && trdQty != -1)
  {
    // emulates STC + STO
    rlzGain += trdGain;
    trdQty = -1;
    trdPrice = q.Close;
    cross = "SHORT";
  }

  if (cross != string.Empty)
  {
    Console.WriteLine(
    $"{q.Date,10:yyyy-MM-dd} " +
    $"{q.Close,10:c2}" +
    $"{e.StochRsi,7:N1}" +
    $"{e.Signal,7:N1}" +
    $"{cross,7}" +
    $"{rlzGain + trdGain,13:c2}");
  }
```

### Console output

```console
TBD: add console output
```

### More info

- Tutorial: [Create a simple C# console app](https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-console)
- These files can also be found in the [/docs/examples]({{site.github.repository_url}}/tree/main/docs/examples) GitHub repo folder
