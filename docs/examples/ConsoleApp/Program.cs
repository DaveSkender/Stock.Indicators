using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using Skender.Stock.Indicators;

/* BASIC CONSOLE APP | SIMPLE MOVING AVERAGE */

// Fetch historical quotes from data provider.
// We're mocking with a simple JSON file import
string json = File.ReadAllText("quotes.data.json");

Collection<Quote> quotes = JsonSerializer
    .Deserialize<IReadOnlyCollection<Quote>>(json)
    .ToSortedCollection();

// Calculate 10-period SMA
IEnumerable<SmaResult> results = quotes.GetSma(10);

// show results
Console.WriteLine("SMA Results ---------------------------");

foreach (SmaResult r in results.TakeLast(10))
{
    // only showing last 10 records for brevity
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

List<SmaResult> resultsList
    = results.ToList();

for (int i = quotes.Count - 25; i < quotes.Count; i++)
{
    // only showing ~25 records for brevity

    Quote q = quotes[i];
    SmaResult r = resultsList[i];

    bool isBullish = (double)q.Close > r.Sma;

    Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}"
                    + $" and Bullishness is {isBullish}");
}
