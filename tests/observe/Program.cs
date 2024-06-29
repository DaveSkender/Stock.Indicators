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
    private readonly string _alpacaKey = Environment.GetEnvironmentVariable("ALPACA_KEY");
    private readonly string _alpacaSecret = Environment.GetEnvironmentVariable("ALPACA_SECRET");

    internal QuoteStream()
    {
        if (string.IsNullOrEmpty(_alpacaKey))
        {
            throw new ArgumentNullException(
                _alpacaKey,
                "API KEY missing, use `setx ALPACA_KEY \"MY_ALPACA_KEY\"` to set.");
        }

        if (string.IsNullOrEmpty(_alpacaSecret))
        {
            throw new ArgumentNullException(
                _alpacaSecret,
                "API SECRET missing, use `setx ALPACA_SECRET \"MY_ALPACA_SECRET\"` to set.");
        }
    }

    public async Task SubscribeToQuotes(string symbol)
    {
        Console.WriteLine("Press any key to exit the process...");
        Console.WriteLine("PLEASE WAIT. QUOTES ARRIVE EVERY MINUTE.");

        if (string.IsNullOrEmpty(_alpacaKey))
        {
            throw new ArgumentNullException(_alpacaKey);
        }

        if (string.IsNullOrEmpty(_alpacaSecret))
        {
            throw new ArgumentNullException(_alpacaSecret);
        }

        // initialize our quote provider and a few subscribers
        QuoteProvider<Quote> provider = new();

        Sma<Quote> sma = provider.ToSma(3);
        Ema<Quote> ema = provider.ToEma(5);
        Ema<Reusable> useChain = provider
            .Use(CandlePart.Hl2)
            .ToEma(7);
        Ema<SmaResult> emaChain = provider
            .ToSma(4)
            .ToEma(4);

        // connect to Alpaca websocket
        SecretKey secretKey = new(_alpacaKey, _alpacaSecret);

        IAlpacaCryptoStreamingClient client
            = Environments
            .Paper
            .GetAlpacaCryptoStreamingClient(secretKey);

        await client.ConnectAndAuthenticateAsync();

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
        quoteSubscription.Received += q => {
            // add to our provider
            provider.Add(new Quote {
                Timestamp = q.TimeUtc,
                Open = q.Open,
                High = q.High,
                Low = q.Low,
                Close = q.Close,
                Volume = q.Volume
            });

            // display live results
            string liveMessage = $"{q.TimeUtc:u}    ${q.Close:N2}";

            SmaResult s = sma.Results[^1];
            EmaResult e = ema.Results[^1];
            EmaResult u = useChain.Results[^1];
            EmaResult c = emaChain.Results[^1];

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
