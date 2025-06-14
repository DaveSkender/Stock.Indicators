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
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Momentum != null));
        Assert.AreEqual(482, results.Count(x => x.Roc != null));

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

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<RocResult> results = Quotes
            .ToSma(2)
            .ToRoc(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToRoc(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<RocResult> r = BadQuotes
            .ToRoc(35);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Roc is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<RocResult> r0 = Noquotes
            .ToRoc(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<RocResult> r1 = Onequote
            .ToRoc(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<RocResult> results = Quotes
            .ToRoc(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        RocResult last = results[^1];
        Assert.AreEqual(-8.2482, last.Roc.Round(4));
    }

    [TestMethod] // bad lookback period
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToRoc(0));
}
