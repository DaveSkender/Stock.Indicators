using Alpaca.Markets;
using Skender.Stock.Indicators;
using Tests.Common;

namespace ObserveAlpaca;

internal class Program
{
    //private static readonly IEnumerable<Quote> quotes = GetLongest();

    private static async Task Main(string[] args)
    {
        if (args.Any())
        {
            Console.WriteLine(args);
        }

        Paca alpaca = new();
        await alpaca.SubscribeToQuotes("BTC/USD");

        //// todo: replace with real WebSocket
        //Collection<Quote> quotesList = quotes
        //    .ToSortedCollection();

        //QuoteProvider provider = new();
        //EmaObserver observer = provider.GetEma(14);

        //int length = Math.Min(40, quotesList.Count);

        //for (int i = 0; i < length; i++)
        //{
        //    Thread.Sleep(50); // emulate pause

        //    Quote q = quotesList[i];
        //    provider.Add(q);

        //    EmaResult? emaR = observer.Results.LastOrDefault();

        //    if (emaR != null)
        //    {
        //        string msg = $"{emaR!.Date:s} {emaR!.Ema:N4}";
        //        Console.WriteLine(msg);
        //    }
        //}

        //observer.Unsubscribe();
        //provider.EndTransmission();
    }

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongest()
        => File.ReadAllLines("SNP62YR.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .ToList();

}

public class Paca
{
    private readonly string? alpacaApiKey = Environment.GetEnvironmentVariable("AlpacaApiKey");
    private readonly string? alpacaSecret = Environment.GetEnvironmentVariable("AlpacaSecret");

    public async Task SubscribeToQuotes(string symbol)
    {
        Console.WriteLine("PLEASE WAIT. QUOTES ARRIVE EVERY MINUTE.");
        Console.WriteLine("Press any key to exit the process...");

        if (alpacaApiKey == null)
        {
            throw new ArgumentNullException(alpacaApiKey);
        }

        if (alpacaSecret == null)
        {
            throw new ArgumentNullException(alpacaSecret);
        }

        // initialize our quote provider and a few subscribers
        QuoteProvider provider = new();

        EmaObserver ema = provider.GetEma(14);
        SmaObserver sma = provider.GetSma(5);
        EmaObserver emaChain = provider
            .Use(CandlePart.HL2)
            .GetEma(10);

        // connect to Alpaca websocket
        SecretKey secretKey = new(alpacaApiKey, alpacaSecret);

        IAlpacaCryptoStreamingClient client
            = Environments
            .Paper
            .GetAlpacaCryptoStreamingClient(secretKey);

        await client.ConnectAndAuthenticateAsync();

        AutoResetEvent[] waitObjects = new[]
        {
            new AutoResetEvent(false)
        };

        IAlpacaDataSubscription<IBar> alpacaSubscription
            = client.GetMinuteBarSubscription(symbol);

        alpacaSubscription.Received += (q) =>
        {
            Console.WriteLine($"{q.Symbol} {q.TimeUtc:s} ${q.Close:N2} | {q.TradeCount} trades");

            // add to our provider
            provider.Add(new Quote
            {
                Date = q.TimeUtc,
                Open = q.Open,
                High = q.High,
                Low = q.Low,
                Close = q.Close,
                Volume = q.Volume
            });
        };

        await client.SubscribeAsync(alpacaSubscription);

        // to stop watching
        Console.ReadKey();

        provider.EndTransmission();
        await client.UnsubscribeAsync(alpacaSubscription);
        await client.DisconnectAsync();

        Console.WriteLine("-- QUOTES STORED --");
        List<Quote> provQuotes = provider.Quotes.ToList();
        int provLength = provQuotes.Count;
        for (int i = 0; i < provLength; i++)
        {
            Quote pq = provQuotes[i];
            Console.WriteLine($"{symbol} {pq.Date:s} ${pq.Close:N2}");
        }

        // show last 3 results for indicator results

        Console.WriteLine("-- EMA RESULTS (last 3 only) --");
        List<EmaResult> e1 = ema.Results.ToList();
        for (int i = Math.Max(0, e1.Count - 3); i < e1.Count; i++)
        {
            EmaResult r = e1[i];
            Console.WriteLine($"{symbol} {r.Date:s} ${r.Ema:N2}");
        }

        Console.WriteLine("-- EMA CHAINED (last 3 only) --");
        List<EmaResult> e2 = emaChain.Results.ToList();
        for (int i = Math.Max(0, e2.Count - 3); i < e2.Count; i++)
        {
            EmaResult r = e2[i];
            Console.WriteLine($"{symbol} {r.Date:s} ${r.Ema:N2}");
        }

        Console.WriteLine("-- SMA RESULTS (last 3 only) --");
        List<SmaResult> s1 = sma.Results.ToList();
        for (int i = Math.Max(0, s1.Count - 3); i < s1.Count; i++)
        {
            SmaResult r = s1[i];
            Console.WriteLine($"{symbol} {r.Date:s} ${r.Sma:N2}");
        }
    }
}