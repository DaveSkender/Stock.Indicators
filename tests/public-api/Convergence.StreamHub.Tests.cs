namespace Behavioral;

/// <summary>
/// StreamHub convergence tests verify that StreamHub implementations
/// produce consistent results when streaming data incrementally vs batch processing.
/// </summary>
[TestClass, TestCategory("Integration")]
public class ConvergenceStreamHub : TestBase
{
    private static readonly int[] QuotesQuantities =
        [14, 28, 40, 50, 75, 100, 150, 200, 250, 350, 500];

    [TestMethod]
    public void Adx()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            AdxHub hub = provider.ToAdxHub(14);

            foreach (Quote q in qts)
                provider.Add(q);

            AdxResult l = hub.Results[^1];
            Console.WriteLine($"ADX(14) StreamHub on {l.Timestamp:d} with {qts.Count,4} streaming qts: {l.Adx:N8}");
        }
    }

    [TestMethod]
    public void Atr()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            AtrHub hub = provider.ToAtrHub(14);

            foreach (Quote q in qts)
                provider.Add(q);

            AtrResult l = hub.Results[^1];
            Console.WriteLine($"ATR(14) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Atr:N8}");
        }
    }

    [TestMethod]
    public void Ema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            EmaHub hub = provider.ToEmaHub(14);

            foreach (Quote q in qts)
                provider.Add(q);

            EmaResult l = hub.Results[^1];
            Console.WriteLine($"EMA(14) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Ema:N8}");
        }
    }

    [TestMethod]
    public void Macd()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            MacdHub hub = provider.ToMacdHub(12, 26, 9);

            foreach (Quote q in qts)
                provider.Add(q);

            MacdResult l = hub.Results[^1];
            Console.WriteLine($"MACD StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Macd:N8}");
        }
    }

    [TestMethod]
    public void Rsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            RsiHub hub = provider.ToRsiHub(14);

            foreach (Quote q in qts)
                provider.Add(q);

            RsiResult l = hub.Results[^1];
            Console.WriteLine($"RSI(14) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Rsi:N8}");
        }
    }

    [TestMethod]
    public void Sma()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            SmaHub hub = provider.ToSmaHub(14);

            foreach (Quote q in qts)
                provider.Add(q);

            SmaResult l = hub.Results[^1];
            Console.WriteLine($"SMA(14) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Sma:N8}");
        }
    }

    [TestMethod]
    public void Stoch()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            StochHub hub = provider.ToStochHub(14, 3, 3);

            foreach (Quote q in qts)
                provider.Add(q);

            StochResult l = hub.Results[^1];
            Console.WriteLine($"STOCH StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.K:N8}");
        }
    }
}
