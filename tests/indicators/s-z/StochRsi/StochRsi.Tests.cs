namespace Tests.Indicators.Series;

[TestClass]
public class StochRsiTests : SeriesTestBase
{
    [TestMethod]
    public void FastRsi()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 1;

        List<StochRsiResult> results =
            Quotes.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToList();

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

        List<StochRsiResult> results =
            Quotes.GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToList();

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
        List<StochRsiResult> results = Quotes
            .Use(CandlePart.Close)
            .GetStochRsi(14, 14, 3)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(475, results.Count(x => x.StochRsi != null));
        Assert.AreEqual(0, results.Count(x => x.StochRsi is double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<StochRsiResult> results = Quotes
            .GetSma(2)
            .GetStochRsi(14, 14, 3)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.StochRsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetStochRsi(14, 14, 3, 3)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(464, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<StochRsiResult> r = BadQuotes
            .GetStochRsi(15, 20, 3, 2)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.StochRsi is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<StochRsiResult> r0 = Noquotes
            .GetStochRsi(10, 20, 3)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<StochRsiResult> r1 = Onequote
            .GetStochRsi(8, 13, 2)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 3;

        List<StochRsiResult> results = Quotes
            .GetStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
        Assert.AreEqual(502 - removeQty, results.Count);

        StochRsiResult last = results.LastOrDefault();
        Assert.AreEqual(89.8385, last.StochRsi.Round(4));
        Assert.AreEqual(73.4176, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStochRsi(0, 14, 3));

        // bad STO period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStochRsi(14, 0, 3, 3));

        // bad STO signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStochRsi(14, 14, 0));

        // bad STO smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetStochRsi(14, 14, 3, 0));
    }
}
