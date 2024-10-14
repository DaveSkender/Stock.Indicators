namespace StaticSeries;

[TestClass]
public class Tsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .ToTsi();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(465, results.Count(x => x.Tsi != null));
        Assert.AreEqual(459, results.Count(x => x.Signal != null));

        // sample values
        TsiResult r2 = results[37];
        Assert.AreEqual(53.1204, r2.Tsi.Round(4));
        Assert.AreEqual(null, r2.Signal);

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
    public void UseReusable()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .Use(CandlePart.Close)
            .ToTsi();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(465, results.Count(x => x.Tsi != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .ToSma(2)
            .ToTsi();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(464, results.Count(x => x.Tsi != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToTsi()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(456, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<TsiResult> r = BadQuotes
            .ToTsi();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Tsi is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<TsiResult> r = BigQuotes
            .ToTsi();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<TsiResult> r0 = Noquotes
            .ToTsi();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<TsiResult> r1 = Onequote
            .ToTsi();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<TsiResult> results = Quotes
            .ToTsi()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (25 + 13 + 250), results.Count);

        TsiResult last = results[^1];
        Assert.AreEqual(-28.3513, last.Tsi.Round(4));
        Assert.AreEqual(-29.3597, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToTsi(0));

        // bad smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToTsi(25, 0));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.ToTsi(25, 13, -1));
    }
}
