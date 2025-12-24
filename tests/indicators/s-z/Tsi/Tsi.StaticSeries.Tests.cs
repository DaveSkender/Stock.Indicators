namespace StaticSeries;

[TestClass]
public class Tsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .ToTsi();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(465, results.Where(static x => x.Tsi != null));
        Assert.HasCount(459, results.Where(static x => x.Signal != null));

        // sample values
        TsiResult r2 = results[37];
        Assert.AreEqual(53.1204, r2.Tsi.Round(4));
        Assert.IsNull(r2.Signal);

        TsiResult r3A = results[43];
        Assert.AreEqual(46.0960, r3A.Tsi.Round(4));
        Assert.AreEqual(51.6916, r3A.Signal.Round(4));

        TsiResult r3B = results[44];
        Assert.AreEqual(42.5121, r3B.Tsi.Round(4));
        Assert.AreEqual(49.3967, r3B.Signal.Round(4));

        TsiResult r4 = results[149];
        Assert.AreEqual(29.0936, r4.Tsi.Round(4));
        Assert.AreEqual(28.0134, r4.Signal.Round(4));

        TsiResult r5 = results[249];
        Assert.AreEqual(41.9232, r5.Tsi.Round(4));
        Assert.AreEqual(42.4063, r5.Signal.Round(4));

        TsiResult r6 = results[501];
        Assert.AreEqual(-28.3513, r6.Tsi.Round(4));
        Assert.AreEqual(-29.3597, r6.Signal.Round(4));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<TsiResult> results = Quotes.ToTsi(25, 13, 7);
        results.IsBetween(x => x.Tsi, -100, 100);
        results.IsBetween(x => x.Signal, -100, 100);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .Use(CandlePart.Close)
            .ToTsi();

        Assert.HasCount(502, results);
        Assert.HasCount(465, results.Where(static x => x.Tsi != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .ToSma(2)
            .ToTsi();

        Assert.HasCount(502, results);
        Assert.HasCount(464, results.Where(static x => x.Tsi != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToTsi()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(456, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<TsiResult> r = BadQuotes
            .ToTsi();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Tsi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<TsiResult> r = BigQuotes
            .ToTsi();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<TsiResult> r0 = Noquotes
            .ToTsi();

        Assert.IsEmpty(r0);

        IReadOnlyList<TsiResult> r1 = Onequote
            .ToTsi();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .ToTsi()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (25 + 13 + 250), results);

        TsiResult last = results[^1];
        Assert.AreEqual(-28.3513, last.Tsi.Round(4));
        Assert.AreEqual(-29.3597, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToTsi(0));

        // bad smoothing period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToTsi(25, 0));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToTsi(25, 13, -1));
    }
}
