using System.Collections.ObjectModel;
using Internal.Tests;
using Skender.Stock.Indicators;

internal class Program
{
    private static readonly IEnumerable<Quote> quotes = GetLongest();

    private static void Main(string[] args)
    {
        if (args.Any())
        {
            Console.WriteLine(args);
        }

        // todo: replace with real WebSocket feed?
        Collection<Quote> quotesList = quotes
            .ToSortedCollection();

        QuoteProvider provider = new();
        EmaObserver obsEma = new(provider, 14);

        int length = Math.Min(40, quotesList.Count);

        for (int i = 0; i < length; i++)
        {
            Thread.Sleep(50);

            Quote q = quotesList[i];
            provider.Add(q);  // todo: use passthru EMA so it returns new EMA?

            EmaResult? emaR = obsEma.Results.LastOrDefault();

            if (emaR != null)
            {
                string msg = $"{emaR!.Date:s} {emaR!.Ema:N4}";
                Console.WriteLine(msg);
            }
        }

        obsEma.Unsubscribe();
        provider.EndTransmission();
    }

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongest()
        => File.ReadAllLines("longest.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .ToList();
}

