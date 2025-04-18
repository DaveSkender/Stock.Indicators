namespace StaticSeries;

[TestClass]
public class Macd : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        IReadOnlyList<MacdResult> results =
            Quotes.ToMacd(fastPeriods, slowPeriods, signalPeriods);

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
        IReadOnlyList<MacdResult> results = Quotes
            .Use(CandlePart.Close)
            .ToMacd();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Macd != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<MacdResult> results = Quotes
            .ToSma(2)
            .ToMacd();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(476, results.Count(x => x.Macd != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToMacd()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<MacdResult> r = BadQuotes
            .ToMacd(10, 20, 5);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Macd is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<MacdResult> r0 = Noquotes
            .ToMacd();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<MacdResult> r1 = Onequote
            .ToMacd();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        IReadOnlyList<MacdResult> results = Quotes
            .ToMacd(fastPeriods, slowPeriods, signalPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

        MacdResult last = results[^1];
        Assert.AreEqual(-6.2198, last.Macd.Round(4));
        Assert.AreEqual(-5.8569, last.Signal.Round(4));
        Assert.AreEqual(-0.3629, last.Histogram.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToMacd(0));

        // bad slow periods must be larger than faster period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToMacd(12, 12));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToMacd(12, 26, -1));
    }
}
