using Alpaca.Markets;
using Skender.Stock.Indicators;

namespace ObserveStreaming;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length != 0)
        {
            Console.WriteLine(args);
        }

        QuoteStream quoteStream = new();
        await quoteStream.SubscribeToQuotes("BTC/USD");
    }
}

public class QuoteStream
{
    private readonly string alpacaApiKey = Environment.GetEnvironmentVariable("ALPACA_KEY");
    private readonly string alpacaSecret = Environment.GetEnvironmentVariable("ALPACA_SECRET");

    internal QuoteStream()
    {
        if (string.IsNullOrEmpty(alpacaApiKey))
        {
            throw new ArgumentNullException(
                alpacaApiKey,
                $"API KEY missing, use `setx ALPACA_KEY \"MY_ALPACA_KEY\"` to set.");
        }

        if (string.IsNullOrEmpty(alpacaSecret))
        {
            throw new ArgumentNullException(
                alpacaSecret,
                $"API SECRET missing, use `setx ALPACA_SECRET \"MY_ALPACA_SECRET\"` to set.");
        }
    }

    public async Task SubscribeToQuotes(string symbol)
    {
        Console.WriteLine("Press any key to exit the process...");
        Console.WriteLine("PLEASE WAIT. QUOTES ARRIVE EVERY MINUTE.");

        if (string.IsNullOrEmpty(alpacaApiKey))
        {
            throw new ArgumentNullException(alpacaApiKey);
        }

        if (string.IsNullOrEmpty(alpacaSecret))
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

        // TODO: is this needed?
        AutoResetEvent[] waitObjects = [new AutoResetEvent(false)];

        IAlpacaDataSubscription<IBar> quoteSubscription
            = client.GetMinuteBarSubscription(symbol);

        await client.SubscribeAsync(quoteSubscription);

        // console display header
        Console.WriteLine("A new quote will be shown when they arrive every minute.");
        Console.WriteLine("PLEASE WAIT > 8 MINUTES BEFORE EXITING TO SEE ALL 4 INDICATORS CALCULATED.");
        Console.WriteLine("Press any key to EXIT the process and to see results.");
        Console.WriteLine();

        Console.WriteLine("Date                   Close price      SMA(3)      EMA(5)  EMA(7,HL2)  SMA/EMA(8)");
        Console.WriteLine("----------------------------------------------------------------------------------");

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

            Console.WriteLine($"{q.Symbol} {q.TimeUtc:s} ${q.Close:N2} | {q.TradeCount} trades");
        };

        await client.SubscribeAsync(quoteSubscription);

        // to stop watching on key press
        Console.ReadKey();

        // end observation
        provider.EndTransmission();

        // close WebSocket
        await client.UnsubscribeAsync(quoteSubscription);
        await client.DisconnectAsync();
    }
}
