namespace StaticSeries;

[TestClass]
public class Kama : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int erPeriods = 10;
        const int fastPeriods = 2;
        const int slowPeriods = 30;

        IReadOnlyList<KamaResult> results = Quotes
            .ToKama(erPeriods, fastPeriods, slowPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(492, results.Where(static x => x.Er != null));
        Assert.HasCount(493, results.Where(static x => x.Kama != null));

        // sample values
        KamaResult r1 = results[8];
        Assert.IsNull(r1.Er);
        Assert.IsNull(r1.Kama);

        KamaResult r2 = results[9];
        Assert.IsNull(r2.Er);
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
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<KamaResult> results = Quotes
            .ToKama();

        TestAsserts.AlwaysBounded(results, x => x.Er, 0, 1);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<KamaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToKama();

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Kama != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<KamaResult> results = Quotes
            .ToSma(2)
            .ToKama();

        Assert.HasCount(502, results);
        Assert.HasCount(492, results.Where(static x => x.Kama != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToKama()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<KamaResult> r = BadQuotes
            .ToKama();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Kama is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<KamaResult> r0 = Noquotes
            .ToKama();

        Assert.IsEmpty(r0);

        IReadOnlyList<KamaResult> r1 = Onequote
            .ToKama();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        const int erPeriods = 10;
        const int fastPeriods = 2;
        const int slowPeriods = 30;

        IReadOnlyList<KamaResult> results = Quotes
            .ToKama(erPeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - Math.Max(erPeriods + 100, erPeriods * 10), results);

        KamaResult last = results[^1];
        Assert.AreEqual(0.2214, last.Er.Round(4));
        Assert.AreEqual(240.1138, last.Kama.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad ER period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKama(0));

        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKama(10, 0));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKama(10, 5, 5));
    }
}
