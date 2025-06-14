namespace StaticSeries;

[TestClass]
public class StochRsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard() // Fast RSI
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 1;

        IReadOnlyList<StochRsiResult> results =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(475, results.Count(x => x.StochRsi != null));
        Assert.AreEqual(473, results.Count(x => x.Signal != null));

        // sample values
        StochRsiResult r1 = results[31];
        Assert.AreEqual(93.3333, r1.StochRsi.Round(4));
        Assert.AreEqual(97.7778, r1.Signal.Round(4));

        StochRsiResult r2 = results[152];
        Assert.AreEqual(0, r2.StochRsi);
        Assert.AreEqual(0, r2.Signal);

        StochRsiResult r3 = results[249];
        Assert.AreEqual(36.5517, r3.StochRsi.Round(4));
        Assert.AreEqual(27.3094, r3.Signal.Round(4));

        StochRsiResult r4 = results[501];
        Assert.AreEqual(97.5244, r4.StochRsi.Round(4));
        Assert.AreEqual(89.8385, r4.Signal.Round(4));
    }

    [TestMethod]
    public void SlowRsi()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> results =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.StochRsi != null));
        Assert.AreEqual(471, results.Count(x => x.Signal != null));

        // sample values
        StochRsiResult r1 = results[31];
        Assert.AreEqual(97.7778, r1.StochRsi.Round(4));
        Assert.AreEqual(99.2593, r1.Signal.Round(4));

        StochRsiResult r2 = results[152];
        Assert.AreEqual(0, r2.StochRsi);
        Assert.AreEqual(20.0263, r2.Signal.Round(4));

        StochRsiResult r3 = results[249];
        Assert.AreEqual(27.3094, r3.StochRsi.Round(4));
        Assert.AreEqual(33.2716, r3.Signal.Round(4));

        StochRsiResult r4 = results[501];
        Assert.AreEqual(89.8385, r4.StochRsi.Round(4));
        Assert.AreEqual(73.4176, r4.Signal.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<StochRsiResult> results = Quotes
            .Use(CandlePart.Close)
            .ToStochRsi(14, 14, 3);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(475, results.Count(x => x.StochRsi != null));
        Assert.AreEqual(0, results.Count(x => x.StochRsi is double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StochRsiResult> results = Quotes
            .ToSma(2)
            .ToStochRsi(14, 14, 3);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.StochRsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToStochRsi(14, 14, 3, 3)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(464, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<StochRsiResult> r = BadQuotes
            .ToStochRsi(15, 20, 3, 2);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.StochRsi is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<StochRsiResult> r0 = Noquotes
            .ToStochRsi(10, 20, 3);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<StochRsiResult> r1 = Onequote
            .ToStochRsi(8, 13, 2);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> results = Quotes
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .RemoveWarmupPeriods();

        // assertions
        int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
        Assert.AreEqual(502 - removeQty, results.Count);

        StochRsiResult last = results[^1];
        Assert.AreEqual(89.8385, last.StochRsi.Round(4));
        Assert.AreEqual(73.4176, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToStochRsi(0, 14, 3));

        // bad STO period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToStochRsi(14, 0, 3, 3));

        // bad STO signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToStochRsi(14, 14, 0));

        // bad STO smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToStochRsi(14, 14, 3, 0));
    }
}
