using Alpaca.Markets;
using Skender.Stock.Indicators;

namespace ObserveAlpaca;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length != 0)
        {
            Console.WriteLine(args);
        }

        string symbol = "BTC/USD";
        Console.WriteLine($"STREAMING QUOTES FOR {symbol}");
        Console.WriteLine();

        await SubscribeToQuotes(symbol);
    }

    public static async Task SubscribeToQuotes(string symbol)
    {
        // get and validate keys, see README.md
        string alpacaApiKey = Environment.GetEnvironmentVariable("AlpacaApiKey");
        string alpacaSecret = Environment.GetEnvironmentVariable("AlpacaSecret");

        if (string.IsNullOrEmpty(alpacaApiKey))
        {
            throw new ArgumentNullException(
                alpacaApiKey,
                $"API KEY missing, use `setx AlpacaApiKey \"ALPACA_API_KEY\"` to set.");
        }

        if (string.IsNullOrEmpty(alpacaSecret))
        {
            throw new ArgumentNullException(
                alpacaSecret,
                $"API SECRET missing, use `setx AlpacaApiSecret \"ALPACA_SECRET\"` to set.");
        }

        // initialize our quote provider and a few subscribers
        QuoteProvider provider = new();

        SmaObserver sma = provider.GetSma(3);
        EmaObserver ema = provider.GetEma(5);
        EmaObserver emaChain = provider
            .Use(CandlePart.HL2)
            .GetEma(7);

        // connect to Alpaca WebSocket
        SecretKey secretKey = new(alpacaApiKey, alpacaSecret);

        IAlpacaCryptoStreamingClient client
            = Environments
            .Paper
            .GetAlpacaCryptoStreamingClient(secretKey);

        await client.ConnectAndAuthenticateAsync();

        // TODO: is this needed?
        AutoResetEvent[] waitObjects = [new AutoResetEvent(false)];

        IAlpacaDataSubscription<IBar> quoteSubscription
            = client.GetMinuteBarSubscription(symbol);

        await client.SubscribeAsync(quoteSubscription);

        // console display header
        Console.WriteLine("A new quote will be shown when they arrive every minute.");
        Console.WriteLine("PLEASE WAIT > 8 MINUTES BEFORE EXITING TO SEE ALL 3 INDICATORS CALCULATED.");
        Console.WriteLine("Press any key to EXIT the process and to see results.");
        Console.WriteLine();

        Console.WriteLine("Date                   Close price      SMA(3)      EMA(5)  EMA(7,HL2)");
        Console.WriteLine("----------------------------------------------------------------------");

        // handle new quotes
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

            // display live results
            string liveMessage = $"{q.TimeUtc:u}    ${q.Close:N2}";

            SmaResult s = sma.Results.Last();
            EmaResult e = ema.Results.Last();
            EmaResult c = emaChain.Results.Last();

            if (s.Sma is not null)
            {
                liveMessage += $"{s.Sma,12:N1}";
            }

            if (e.Ema is not null)
            {
                liveMessage += $"{e.Ema,12:N1}";
            }

            if (c.Ema is not null)
            {
                liveMessage += $"{c.Ema,12:N1}";
            }

            Console.WriteLine(liveMessage);
        };

        // to stop watching on key press
        Console.ReadKey();

        // terminate subscriptions
        provider.EndTransmission();
        await client.UnsubscribeAsync(quoteSubscription);
        await client.DisconnectAsync();
    }
}