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

        QuoteStream quoteStream = new();
        await quoteStream.SubscribeToQuotes("BTC/USD");
    }
}

public class QuoteStream
{
    private readonly string alpacaApiKey = Environment.GetEnvironmentVariable("ALPACA_KEY");
    private readonly string alpacaSecret = Environment.GetEnvironmentVariable("ALPACA_SECRET");

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
        QuoteProvider<Quote> provider = new();

        Sma sma = provider.AttachSma(3);
        Ema ema = provider.AttachEma(5);
        Ema useChain = provider
            .Use(CandlePart.HL2)
            .AttachEma(7);
        Ema emaChain = provider
            .AttachSma(4)
            .AttachEma(4);

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

            // display live results
            string liveMessage = $"{q.TimeUtc:u}    ${q.Close:N2}";

            SmaResult s = sma.Results.Last();
            EmaResult e = ema.Results.Last();
            EmaResult u = useChain.Results.Last();
            EmaResult c = emaChain.Results.Last();

            if (s.Sma is not null)
            {
                liveMessage += $"{s.Sma,12:N1}";
            }

            if (e.Ema is not null)
            {
                liveMessage += $"{e.Ema,12:N1}";
            }

            if (u.Ema is not null)
            {
                liveMessage += $"{u.Ema,12:N1}";
            }

            if (c.Ema is not null)
            {
                liveMessage += $"{c.Ema,12:N1}";
            }

            Console.WriteLine(liveMessage);
        };

        // to stop watching on key press
        Console.ReadKey();

        // end observation
        provider.EndTransmission();

        // close WebSocket
        await client.UnsubscribeAsync(quoteSubscription);
        await client.DisconnectAsync();
    }
}