namespace StaticSeries;

[TestClass]
public class KamaTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int erPeriods = 10;
        int fastPeriods = 2;
        int slowPeriods = 30;

        IReadOnlyList<KamaResult> results = Quotes
            .GetKama(erPeriods, fastPeriods, slowPeriods);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Er != null));
        Assert.AreEqual(493, results.Count(x => x.Kama != null));

        // sample values
        KamaResult r1 = results[8];
        Assert.AreEqual(null, r1.Er);
        Assert.AreEqual(null, r1.Kama);

        KamaResult r2 = results[9];
        Assert.AreEqual(null, r2.Er);
        Assert.AreEqual(213.7500, r2.Kama.Round(4));

        KamaResult r3 = results[10];
        Assert.AreEqual(0.2465, r3.Er.Round(4));
        Assert.AreEqual(213.7713, r3.Kama.Round(4));

        KamaResult r4 = results[24];
        Assert.AreEqual(0.2136, r4.Er.Round(4));
        Assert.AreEqual(214.7423, r4.Kama.Round(4));

        KamaResult r5 = results[149];
        Assert.AreEqual(0.3165, r5.Er.Round(4));
        Assert.AreEqual(235.5510, r5.Kama.Round(4));

        KamaResult r6 = results[249];
        Assert.AreEqual(0.3182, r6.Er.Round(4));
        Assert.AreEqual(256.0898, r6.Kama.Round(4));

        KamaResult r7 = results[501];
        Assert.AreEqual(0.2214, r7.Er.Round(4));
        Assert.AreEqual(240.1138, r7.Kama.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<KamaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetKama();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Kama != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<KamaResult> results = Quotes
            .GetSma(2)
            .GetKama();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Kama != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetKama()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<KamaResult> r = BadQuotes
            .GetKama();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Kama is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<KamaResult> r0 = Noquotes
            .GetKama();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<KamaResult> r1 = Onequote
            .GetKama();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int erPeriods = 10;
        int fastPeriods = 2;
        int slowPeriods = 30;

        IReadOnlyList<KamaResult> results = Quotes
            .GetKama(erPeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - Math.Max(erPeriods + 100, erPeriods * 10), results.Count);

        KamaResult last = results[^1];
        Assert.AreEqual(0.2214, last.Er.Round(4));
        Assert.AreEqual(240.1138, last.Kama.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad ER period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetKama(0));

        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetKama(10, 0));

        // bad slow period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetKama(10, 5, 5));
    }
}
