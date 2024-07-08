namespace ObserveStreaming;

internal class Program
{
    private static void Main(string[] args)
    {
        // arg 0: true/false to show console (default: true)
        // arg 1: quotes per minute (default: 0 [no delays])

        bool log = true;
        int qpm = 0;

        if (args.Length != 0)
        {
            Console.WriteLine(args);

            if (args.Length > 0)
            {
                log = bool.Parse(args[0]);
            }

            if (args.Length > 1)
            {
                qpm = int.Parse(args[1]);
            }
        }

        Scenarios scenarios = new(log, qpm);
        scenarios.MultipleSubscribers();
    }
}

public class Scenarios
{
    private readonly bool showConsole;
    private readonly int quotesPerMinute;

    private static readonly QuoteHub<Quote> provider = new();

    private static readonly IReadOnlyList<Quote> quotes
        = TestData.GetDefault().ToSortedCollection();

    internal Scenarios(bool log, int qpm)
    {
        showConsole = log;
        quotesPerMinute = qpm;
    }

    public void MultipleSubscribers()
    {
        SmaHub<Quote> sma = provider.ToSma(3);
        EmaHub<Quote> ema = provider.ToEma(5);
        EmaHub<Reusable> useChain = provider.Use(CandlePart.HL2).ToEma(7);
        EmaHub<SmaResult> emaChain = provider.ToSma(4).ToEma(4);

        // initialize console display
        if (showConsole)
        {
            Console.WriteLine("""
            Date                Close price      SMA(3)      EMA(5)  EMA(7,HL2)  SMA/EMA(8)
            -------------------------------------------------------------------------------
            """);
        }

        // add quotes to provider
        foreach (Quote q in quotes)
        {
            provider.Add(q);

            // wait for next quote
            Timewarp();

            // send to console
            SendToConsole(q, sma, ema, useChain, emaChain);
        }
    }

    private void SendToConsole(
        Quote q,
        SmaHub<Quote> sma,
        EmaHub<Quote> ema,
        EmaHub<Reusable> useChain,
        EmaHub<SmaResult> emaChain)
    {
        if (!showConsole)
        {
            return;
        }

        // display live results
        string liveMessage = $"{q.Timestamp:u}    ${q.Close:N2}";

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
    }

    /// <summary>
    /// Emulate quote arrival rate.
    /// Use '0' to disable.
    /// </summary>
    private void Timewarp()
    {
        if (quotesPerMinute is 0)
        {
            return;
        }

        Thread.Sleep(60000 / quotesPerMinute);
    }
}
