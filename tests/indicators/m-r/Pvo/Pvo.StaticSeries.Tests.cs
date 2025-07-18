namespace StaticSeries;

[TestClass]
public class Pvo : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        IReadOnlyList<PvoResult> results =
            Quotes.ToPvo(fastPeriods, slowPeriods, signalPeriods);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Pvo != null));
        Assert.AreEqual(469, results.Count(x => x.Signal != null));
        Assert.AreEqual(469, results.Count(x => x.Histogram != null));

        // sample values
        PvoResult r1 = results[24];
        Assert.IsNull(r1.Pvo);
        Assert.IsNull(r1.Signal);
        Assert.IsNull(r1.Histogram);

        PvoResult r2 = results[33];
        Assert.AreEqual(1.5795, r2.Pvo.Round(4));
        Assert.AreEqual(-3.5530, r2.Signal.Round(4));
        Assert.AreEqual(5.1325, r2.Histogram.Round(4));

        PvoResult r3 = results[149];
        Assert.AreEqual(-7.1910, r3.Pvo.Round(4));
        Assert.AreEqual(-5.1159, r3.Signal.Round(4));
        Assert.AreEqual(-2.0751, r3.Histogram.Round(4));

        PvoResult r4 = results[249];
        Assert.AreEqual(-6.3667, r4.Pvo.Round(4));
        Assert.AreEqual(1.7333, r4.Signal.Round(4));
        Assert.AreEqual(-8.1000, r4.Histogram.Round(4));

        PvoResult r5 = results[501];
        Assert.AreEqual(10.4395, r5.Pvo.Round(4));
        Assert.AreEqual(12.2681, r5.Signal.Round(4));
        Assert.AreEqual(-1.8286, r5.Histogram.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToPvo()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<PvoResult> r = BadQuotes
            .ToPvo(10, 20, 5);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pvo is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<PvoResult> r0 = Noquotes
            .ToPvo();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<PvoResult> r1 = Onequote
            .ToPvo();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        IReadOnlyList<PvoResult> results = Quotes
            .ToPvo(fastPeriods, slowPeriods, signalPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

        PvoResult last = results[^1];
        Assert.AreEqual(10.4395, last.Pvo.Round(4));
        Assert.AreEqual(12.2681, last.Signal.Round(4));
        Assert.AreEqual(-1.8286, last.Histogram.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToPvo(0));

        // bad slow periods must be larger than faster period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToPvo(12, 12));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToPvo(12, 26, -1));
    }
}
