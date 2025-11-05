namespace Behavioral;

/// <summary>
/// BufferList convergence tests verify that BufferList implementations
/// produce consistent results with different amounts of historical data.
/// </summary>
[TestClass, TestCategory("Integration")]
public class ConvergenceBufferList : TestBase
{
    private static readonly int[] QuotesQuantities =
        [5, 14, 28, 40, 50, 75, 100, 110, 120, 130, 140, 150, 160, 175, 200, 250, 350, 500, 600, 700, 800, 900, 1000];

    [TestMethod]
    public void Adx()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            AdxList buffer = new(14) { qts };

            AdxResult l = buffer[^1];
            Console.WriteLine($"ADX(14) BufferList on {l.Timestamp:d} with {qts.Count,4} historical qts: {l.Adx:N8}");
        }
    }

    [TestMethod]
    public void Atr()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            AtrList buffer = new(14) { qts };

            AtrResult l = buffer[^1];
            Console.WriteLine($"ATR(14) BufferList on {l.Timestamp:d} with {qts.Count,4} periods: {l.Atr:N8}");
        }
    }

    [TestMethod]
    public void Ema()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            EmaList buffer = new(14) { qts };

            EmaResult l = buffer[^1];
            Console.WriteLine($"EMA(14) BufferList on {l.Timestamp:d} with {qts.Count,4} periods: {l.Ema:N8}");
        }
    }

    [TestMethod]
    public void Macd()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            MacdList buffer = new(12, 26, 9) { qts };

            MacdResult l = buffer[^1];
            Console.WriteLine($"MACD BufferList on {l.Timestamp:d} with {qts.Count,4} periods: {l.Macd:N8}");
        }
    }

    [TestMethod]
    public void Rsi()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            RsiList buffer = new(14) { qts };

            RsiResult l = buffer[^1];
            Console.WriteLine($"RSI(14) BufferList on {l.Timestamp:d} with {qts.Count,4} periods: {l.Rsi:N8}");
        }
    }

    [TestMethod]
    public void Sma()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            SmaList buffer = new(14) { qts };

            SmaResult l = buffer[^1];
            Console.WriteLine($"SMA(14) BufferList on {l.Timestamp:d} with {qts.Count,4} periods: {l.Sma:N8}");
        }
    }

    [TestMethod]
    public void Stoch()
    {
        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            StochList buffer = new(14, 3, 3) { qts };

            StochResult l = buffer[^1];
            Console.WriteLine($"STOCH BufferList on {l.Timestamp:d} with {qts.Count,4} periods: {l.K:N8}");
        }
    }
}
