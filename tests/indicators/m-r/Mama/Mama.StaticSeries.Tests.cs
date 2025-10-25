namespace StaticSeries;

[TestClass]
public class Mama : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        const double fastLimit = 0.5;
        const double slowLimit = 0.05;

        IReadOnlyList<MamaResult> results = Quotes
            .ToMama(fastLimit, slowLimit);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(497, results.Where(static x => x.Mama != null));

        // sample values
        MamaResult r1 = results[4];
        Assert.IsNull(r1.Mama);
        Assert.IsNull(r1.Fama);

        MamaResult r2 = results[5];
        Assert.AreEqual(213.73, r2.Mama);
        Assert.AreEqual(213.73, r2.Fama);

        MamaResult r3 = results[6];
        Assert.AreEqual(213.7850, r3.Mama.Round(4));
        Assert.AreEqual(213.7438, r3.Fama.Round(4));

        MamaResult r4 = results[25];
        Assert.AreEqual(215.9524, r4.Mama.Round(4));
        Assert.AreEqual(215.1407, r4.Fama.Round(4));

        MamaResult r5 = results[149];
        Assert.AreEqual(235.6593, r5.Mama.Round(4));
        Assert.AreEqual(234.3660, r5.Fama.Round(4));

        MamaResult r6 = results[249];
        Assert.AreEqual(256.8026, r6.Mama.Round(4));
        Assert.AreEqual(254.0605, r6.Fama.Round(4));

        MamaResult r7 = results[501];
        Assert.AreEqual(244.1092, r7.Mama.Round(4));
        Assert.AreEqual(252.6139, r7.Fama.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<MamaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToMama();

        Assert.HasCount(502, results);
        Assert.HasCount(497, results.Where(static x => x.Mama != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<MamaResult> results = Quotes
            .ToSma(2)
            .ToMama();

        Assert.HasCount(502, results);
        Assert.HasCount(496, results.Where(static x => x.Mama != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToMama()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<MamaResult> r = BadQuotes
            .ToMama();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Mama is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<MamaResult> r0 = Noquotes.ToMama();

        Assert.IsEmpty(r0);

        IReadOnlyList<MamaResult> r1 = Onequote.ToMama();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        const double fastLimit = 0.5;
        const double slowLimit = 0.05;

        IReadOnlyList<MamaResult> results = Quotes
            .ToMama(fastLimit, slowLimit)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 50, results);

        MamaResult last = results[^1];
        Assert.AreEqual(244.1092, last.Mama.Round(4));
        Assert.AreEqual(252.6139, last.Fama.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period (same as slow period)
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMama(0.5, 0.5));

        // bad fast period (cannot be 1 or more)
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMama(1, 0.5));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMama(0.5, 0));
    }
}
