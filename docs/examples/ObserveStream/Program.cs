using Alpaca.Markets;
using Skender.Stock.Indicators;

namespace ObserveAlpaca;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Any())
        {
            Console.WriteLine(args);
        }

        await QuoteStream.SubscribeToQuotes("BTC/USD");
    }
}

public class QuoteStream
{
    public static async Task SubscribeToQuotes(string symbol)
    {
        Console.WriteLine("Press any key to exit the process.");
        Console.WriteLine("PLEASE WAIT. QUOTES ARRIVE EVERY MINUTE.");

        // get and validate keys, see README.md
        string? alpacaApiKey = Environment.GetEnvironmentVariable("AlpacaApiKey");
        string? alpacaSecret = Environment.GetEnvironmentVariable("AlpacaSecret");

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

        // connect to Alpaca WebSocket
        SecretKey secretKey = new(alpacaApiKey, alpacaSecret);

        IAlpacaCryptoStreamingClient client
            = Environments
            .Paper
            .GetAlpacaCryptoStreamingClient(secretKey);

        await client.ConnectAndAuthenticateAsync();

        AutoResetEvent[] waitObjects = new[]  // todo: is this needed?
        {
            new AutoResetEvent(false)
        };

        IAlpacaDataSubscription<IBar> quoteSubscription
            = client.GetMinuteBarSubscription(symbol);

        quoteSubscription.Received += (q) =>
        {
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

            Console.WriteLine($"{q.Symbol} {q.TimeUtc:s} ${q.Close:N2} | {q.TradeCount} trades");
        };

        await client.SubscribeAsync(quoteSubscription);

        // to stop watching on key press
        Console.ReadKey();

        provider.EndTransmission();
        await client.UnsubscribeAsync(quoteSubscription);
        await client.DisconnectAsync();

        Console.WriteLine("-- QUOTES STORED (last 10 only) --");
        foreach (Quote? pt in provider.Quotes.TakeLast(10))
        {
            Console.WriteLine($"{symbol} {pt.Date:s} ${pt.Close:N2}");
        }

        // show last 3 results for indicator results
        Console.WriteLine("-- EMA(14,CLOSE) RESULTS (last 3 only) --");
        foreach (EmaResult? e in ema.Results.TakeLast(3))
        {
            Console.WriteLine($"{symbol} {e.Date:s} ${e.Ema:N2}");
        }

        Console.WriteLine("-- EMA(10,HL2) CHAINED (last 3 only) --");
        foreach (EmaResult? e in emaChain.Results.TakeLast(3))
        {
            Console.WriteLine($"{symbol} {e.Date:s} ${e.Ema:N2}");
        }

        Console.WriteLine("-- SMA(5) RESULTS (last 3 only) --");
        foreach (SmaResult? s in sma.Results.TakeLast(3))
        {
            Console.WriteLine($"{symbol} {s.Date:s} ${s.Sma:N2}");
        }
    }
}