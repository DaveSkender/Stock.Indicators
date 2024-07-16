namespace Series;

[TestClass]
public class MacdTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<MacdResult> results =
            Quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Macd != null));
        Assert.AreEqual(469, results.Count(x => x.Signal != null));
        Assert.AreEqual(469, results.Count(x => x.Histogram != null));

        // sample values
        MacdResult r49 = results[49];
        Assert.AreEqual(1.7203, r49.Macd.Round(4));
        Assert.AreEqual(1.9675, r49.Signal.Round(4));
        Assert.AreEqual(-0.2472, r49.Histogram.Round(4));
        Assert.AreEqual(224.1840, r49.FastEma.Round(4));
        Assert.AreEqual(222.4637, r49.SlowEma.Round(4));

        MacdResult r249 = results[249];
        Assert.AreEqual(2.2353, r249.Macd.Round(4));
        Assert.AreEqual(2.3141, r249.Signal.Round(4));
        Assert.AreEqual(-0.0789, r249.Histogram.Round(4));
        Assert.AreEqual(256.6780, r249.FastEma.Round(4));
        Assert.AreEqual(254.4428, r249.SlowEma.Round(4));

        MacdResult r501 = results[501];
        Assert.AreEqual(-6.2198, r501.Macd.Round(4));
        Assert.AreEqual(-5.8569, r501.Signal.Round(4));
        Assert.AreEqual(-0.3629, r501.Histogram.Round(4));
        Assert.AreEqual(245.4957, r501.FastEma.Round(4));
        Assert.AreEqual(251.7155, r501.SlowEma.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        List<MacdResult> results = Quotes
            .Use(CandlePart.Close)
            .GetMacd()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Macd != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<MacdResult> results = Quotes
            .GetSma(2)
            .GetMacd()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(476, results.Count(x => x.Macd != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetMacd()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<MacdResult> r = BadQuotes
            .GetMacd(10, 20, 5)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Macd is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<MacdResult> r0 = Noquotes
            .GetMacd()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<MacdResult> r1 = Onequote
            .GetMacd()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<MacdResult> results = Quotes
            .GetMacd(fastPeriods, slowPeriods, signalPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

        MacdResult last = results.LastOrDefault();
        Assert.AreEqual(-6.2198, last.Macd.Round(4));
        Assert.AreEqual(-5.8569, last.Signal.Round(4));
        Assert.AreEqual(-0.3629, last.Histogram.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetMacd(0));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetMacd(12, 12));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetMacd(12, 26, -1));
    }
}
