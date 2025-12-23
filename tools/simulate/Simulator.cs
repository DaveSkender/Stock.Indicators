using Skender.Stock.Indicators;
using TestData = Test.Data.Data;

namespace Test.Application;

public static class Simulate
{
    public static void Menu(string scenario = "A")
    {
        Go go = new();

        switch (scenario)
        {
            case "A": go.QuoteHub(); break;
            case "B": go.EmaHub(); break;
            case "C": go.ManySubscribers(); break;
        }
    }
}

public class Go
{
    private readonly bool verbose = true;
    private static readonly QuoteHub quoteHub = new();

    private static readonly IReadOnlyList<Quote> quotesList = TestData.GetDefault();

    private static readonly int quotesLength = quotesList.Count;

    public Go()
    {
        if (verbose)
        {
            Prefill();
        }
    }

    private static void Prefill()
    {
        // prefill quotes to provider
        for (int i = 0; i < quotesLength; i++)
        {
            quoteHub.Add(quotesList[i]);
        }
    }

    internal void QuoteHub()
    {
        if (!verbose)
        {
            return;
        }

        // initialize console display
        Console.WriteLine("""

        QUOTE HUB | SCENARIO C

        Date     Close price
        --------------------
        """);

        // add quotes to provider
        for (int i = 0; i < quotesLength; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);

            // wait for next quote
            Timewarp();

            // send to console
            SendToConsole(q);
        }
    }

    private static void SendToConsole(Quote q)
    {
        string m = $"{q.Timestamp:yyyy-MM-dd}   ${q.Close:N2}";
        Console.WriteLine(m);
    }

    internal void EmaHub()
    {
        EmaHub emaHub = quoteHub.ToEmaHub(14);

        if (!verbose)
        {
            return;
        }

        // initialize console display
        Console.WriteLine("""

        EMA HUB | SCENARIO B

        Date     Close price   EMA(14)
        ------------------------------
        """);

        // add quotes to provider
        for (int i = 0; i < quotesLength; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);

            // wait for next quote
            Timewarp();

            // send to console
            SendToConsole(q, emaHub);
        }
    }

    private static void SendToConsole(Quote q, EmaHub emaHub)
    {
        string m = $"{q.Timestamp:yyyy-MM-dd}   ${q.Close:N2}";

        EmaResult e = emaHub.Results[^1];

        m += e.Ema is not null
            ? $"{e.Ema,10:N3}"
            : $"{"[null]",10}";

        Console.WriteLine(m);
    }

    internal void ManySubscribers()
    {
        SmaHub smaHub = quoteHub.ToSmaHub(3);
        EmaHub emaHub = quoteHub.ToEmaHub(5);
        EmaHub useChain = quoteHub.ToQuotePartHub(CandlePart.HL2).ToEmaHub(7);
        EmaHub emaChain = quoteHub.ToSmaHub(4).ToEmaHub(4);

        if (!verbose)
        {
            return;
        }

        // initialize console display
        Console.WriteLine("""

        MANY SUBSCRIBERS | SCENARIO C

        Date     Close price   SMA(3)  EMA(5)  EMA(7,HL2)  SMA/EMA(8)
        -------------------------------------------------------------
        """);

        // add quotes to provider
        for (int i = 0; i < quotesLength; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);

            // wait for next quote
            Timewarp();

            // send to console
            SendToConsole(q, smaHub, emaHub, useChain, emaChain);
        }
    }

    private static void SendToConsole(
        Quote q,
        SmaHub smaHub,
        EmaHub emaHub,
        EmaHub useChain,
        EmaHub emaChain)
    {
        string m = $"{q.Timestamp:yyyy-MM-dd}   ${q.Close:N2}";

        SmaResult s = smaHub.Results[^1];
        EmaResult e = emaHub.Results[^1];
        EmaResult u = useChain.Results[^1];
        EmaResult c = emaChain.Results[^1];

        if (s.Sma is not null)
        {
            m += $"{s.Sma,8:N1}";
        }

        if (e.Ema is not null)
        {
            m += $"{e.Ema,8:N1}";
        }

        if (u.Ema is not null)
        {
            m += $"{u.Ema,12:N1}";
        }

        if (c.Ema is not null)
        {
            m += $"{c.Ema,12:N1}";
        }

        Console.WriteLine(m);
    }

    /// <summary>
    /// Emulate quote arrival rate.
    /// Use '0' to disable.
    /// </summary>
    private static void Timewarp(int quotesPerMinute = 0)
    {
        if (quotesPerMinute == 0)
        {
            return;
        }

        Thread.Sleep(60000 / quotesPerMinute);
    }
}
