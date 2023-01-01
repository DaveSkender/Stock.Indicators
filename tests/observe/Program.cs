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

        // todo: replace with real WebSocket
        Collection<Quote> quotesList = quotes
            .ToSortedCollection();

        QuoteProvider provider = new();
        EmaObserver observer = provider.GetEma(14);

        int length = Math.Min(40, quotesList.Count);

        for (int i = 0; i < length; i++)
        {
            Thread.Sleep(50); // emulate pause

            Quote q = quotesList[i];
            provider.Add(q);

            EmaResult? emaR = observer.Results.LastOrDefault();

            if (emaR != null)
            {
                string msg = $"{emaR!.Date:s} {emaR!.Ema:N4}";
                Console.WriteLine(msg);
            }
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongest()
        => File.ReadAllLines("SNP62YR.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .ToList();
}

