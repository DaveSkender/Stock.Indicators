using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace ConsoleApp;

public class Program
{
    public static void Main()
    {

        // fetch historical quotes from data provider
        IEnumerable<Quote> quotes = GetHistoryFromFeed();

        // calculate 10-period SMA
        IEnumerable<SmaResult> results = quotes.GetSma(10);

        // show results
        Console.WriteLine("SMA Results ---------------------------");

        foreach (SmaResult r in results.TakeLast(10))
        // only showing last 10 records for brevity
        {
            Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}");
        }

        // optionally, you can lookup individual values by date
        DateTime lookupDate = DateTime
            .Parse("2021-08-12T17:08:17.9746795+02:00", CultureInfo.InvariantCulture);

        double? specificSma = results.Find(lookupDate).Sma;

        Console.WriteLine();
        Console.WriteLine("SMA on Specific Date ------------------");
        Console.WriteLine($"SMA on {lookupDate:u} was ${specificSma:N3}");

        // analyze results (compare to quote values)
        Console.WriteLine();
        Console.WriteLine("SMA Analysis --------------------------");

        /************************************************************
          Results are usually returned with the same number of
          elements as the provided quotes; see individual indicator
          docs for more information.

          As such, converting to List means they can be indexed
          with the same ordinal position.
         ************************************************************/

        List<Quote> quotesList = quotes
            .ToList();

        List<SmaResult> resultsList = results
            .ToList();

        for (int i = quotesList.Count - 25; i < quotesList.Count; i++)
        {
            // only showing ~25 records for brevity

            Quote q = quotesList[i];
            SmaResult r = resultsList[i];

            bool isBullish = (double)q.Close > r.Sma;

            Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}"
                            + $" and Bullishness is {isBullish}");
        }
    }


    private static IEnumerable<Quote> GetHistoryFromFeed()
    {
        /************************************************************

         We're mocking a data provider here by simply importing a
         JSON file, a similar format of many public APIs.

         This approach will vary widely depending on where you are
         getting your quote history.

         See https://github.com/DaveSkender/Stock.Indicators/discussions/579
         for free or inexpensive market data providers and examples.

         The return type of IEnumerable<Quote> can also be List<Quote>
         or ICollection<Quote> or other IEnumerable compatible types.

         ************************************************************/

        string json = File.ReadAllText("quotes.data.json");

        List<Quote> quotes = JsonConvert.DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .OrderBy(x => x.Date)
            .ToList();

        return quotes;
    }
}
