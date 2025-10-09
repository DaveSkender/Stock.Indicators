
using Skender.Stock.Indicators;

namespace Test.Application;

public static class Simulate
{
    public static void Menu(string scenario = "C")
    {
        Go go = new();

        switch (scenario)
        {
            case "A": go.QuoteHub(); break;
            case "B": go.EmaHub(); break;
            case "C": go.MultipleSubscribers(); break;
            default: break;
        }
    }
}

public class Go
{
    private readonly bool verbose = true;
    private static readonly QuoteHub<Quote> quoteHub = new();

    private static readonly IReadOnlyList<Quote> quotesList = Tests.Data.Data.GetDefault();

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
        EmaHub<Quote> emaHub = quoteHub.ToEmaHub(14);

        if (!verbose)
        {
            return;
        }

        // initialize console display
        Console.WriteLine("""
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
        EmaHub<Quote> emaHub = quoteHub.ToEmaHub(14);

        if (!verbose)
        {
            return;
        }

        // initialize console display
        Console.WriteLine("""
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

    private static void SendToConsole<T>(Quote q, EmaHub<T> emaHub)
        where T : IReusable
    {
        string m = $"{q.Timestamp:yyyy-MM-dd}   ${q.Close:N2}";

        EmaResult e = emaHub.Results[^1];

        if (e.Ema is not null)
        {
            m += $"{e.Ema,10:N3}";
        }
        else
        {
            m += $"{"[null]",10}";
        }

        Console.WriteLine(m);
    }

    internal void MultipleSubscribers()
    {
        SmaHub<Quote> smaHub = quoteHub.ToSma(3);
        EmaHub<Quote> emaHub = quoteHub.ToEmaHub(5);
        EmaHub<QuotePart> useChain = quoteHub.ToQuotePartHub(CandlePart.HL2).ToEmaHub(7);
        EmaHub<SmaResult> emaChain = quoteHub.ToSma(4).ToEmaHub(4);

        if (!verbose)
        {
            return;
        }

        // initialize console display
        Console.WriteLine("""
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
        SmaHub<Quote> smaHub,
        EmaHub<Quote> emaHub,
        EmaHub<QuotePart> useChain,
        EmaHub<SmaResult> emaChain)
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
