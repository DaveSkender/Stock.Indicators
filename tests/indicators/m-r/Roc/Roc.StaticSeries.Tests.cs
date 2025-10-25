namespace StaticSeries;

[TestClass]
public class Roc : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<RocResult> results = Quotes
            .ToRoc(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Momentum != null));
        Assert.HasCount(482, results.Where(static x => x.Roc != null));

        // sample values
        RocResult r49 = results[49];
        Assert.AreEqual(4.96, r49.Momentum.Round(4));
        Assert.AreEqual(2.2465, r49.Roc.Round(4));

        RocResult r249 = results[249];
        Assert.AreEqual(6.25, r249.Momentum.Round(4));
        Assert.AreEqual(2.4827, r249.Roc.Round(4));

        RocResult r501 = results[501];
        Assert.AreEqual(-22.05, r501.Momentum.Round(4));
        Assert.AreEqual(-8.2482, r501.Roc.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<RocResult> results = Quotes
            .Use(CandlePart.Close)
            .ToRoc(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Roc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<RocResult> results = Quotes
            .ToSma(2)
            .ToRoc(20);

        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(static x => x.Roc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToRoc(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(473, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<RocResult> r = BadQuotes
            .ToRoc(35);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Roc is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<RocResult> r0 = Noquotes
            .ToRoc(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<RocResult> r1 = Onequote
            .ToRoc(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<RocResult> results = Quotes
            .ToRoc(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 20, results);

        RocResult last = results[^1];
        Assert.AreEqual(-8.2482, last.Roc.Round(4));
    }

    [TestMethod] // bad lookback period
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRoc(0));
}
