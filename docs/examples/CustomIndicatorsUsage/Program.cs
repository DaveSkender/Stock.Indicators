using System.Collections.ObjectModel;
using System.Text.Json;
using Custom.Stock.Indicators;
using Skender.Stock.Indicators;

// USE CUSTOM INDICATORS exactly the same as
// other indicators in the library

// Fetch historical quotes from data provider.
// We're mocking with a simple JSON file import
string json = File.ReadAllText("quotes.data.json");

Collection<Quote> quotes = JsonSerializer
    .Deserialize<IReadOnlyCollection<Quote>>(json)
    .ToSortedCollection();

// Calculate 10-period custom AtrWma
IReadOnlyList<AtrWmaResult> results = quotes
            .GetAtrWma(10);

// Show results
Console.WriteLine("ATR WMA Results ---------------------------");

foreach (AtrWmaResult r in results.Take(30))
{
    // only showing first 30 records for brevity
    Console.WriteLine($"ATR WMA on {r.Date:u} was ${r.AtrWma:N3}");
}

// optional: demo of a converter (nulls to NaN)

Console.WriteLine();
Console.WriteLine("ATR WMA Results with NaN (optional) -------");

// TIP: converting ToList() and using For loops is faster to iterate
List<AtrWmaResult> resultsList = results
    .Take(30)
    .ToList();

for (int i = 0; i < resultsList.Count; i++)
{
    AtrWmaResult r = resultsList[i];
    r.AtrWma = r.AtrWma.Null2NaN();

    Console.WriteLine($"ATR WMA on {r.Date:u} was ${r.AtrWma:N3}");
}
