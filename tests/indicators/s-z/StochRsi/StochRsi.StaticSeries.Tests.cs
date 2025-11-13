namespace StaticSeries;

[TestClass]
public class StochRsi : StaticSeriesTestBase
{
    /// <summary>
    /// Fast RSI
    /// </summary>
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;

        IReadOnlyList<StochRsiResult> results =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(475, results.Where(static x => x.StochRsi != null));
        Assert.HasCount(473, results.Where(static x => x.Signal != null));

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
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> results =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(473, results.Where(static x => x.StochRsi != null));
        Assert.HasCount(471, results.Where(static x => x.Signal != null));

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

        Assert.HasCount(502, results);
        Assert.HasCount(475, results.Where(static x => x.StochRsi != null));
        Assert.IsEmpty(results.Where(static x => x.StochRsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StochRsiResult> results = Quotes
            .ToSma(2)
            .ToStochRsi(14, 14, 3);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.StochRsi != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToStochRsi(14, 14, 3, 3)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(464, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<StochRsiResult> r = BadQuotes
            .ToStochRsi(15, 20, 3, 2);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.StochRsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<StochRsiResult> r0 = Noquotes
            .ToStochRsi(10, 20, 3);

        Assert.IsEmpty(r0);

        IReadOnlyList<StochRsiResult> r1 = Onequote
            .ToStochRsi(8, 13, 2);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> results = Quotes
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .RemoveWarmupPeriods();

        // assertions
        const int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
        Assert.HasCount(502 - removeQty, results);

        StochRsiResult last = results[^1];
        Assert.AreEqual(89.8385, last.StochRsi.Round(4));
        Assert.AreEqual(73.4176, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(0, 14, 3));

        // bad STO period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 0, 3, 3));

        // bad STO signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 14, 0));

        // bad STO smoothing period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 14, 3, 0));
    }
}
