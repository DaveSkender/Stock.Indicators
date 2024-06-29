namespace Tests.Indicators.Series;

[TestClass]
public class RocTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<RocResult> results = Quotes
            .GetRoc(20)
            .ToList();

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
        List<RocResult> results = Quotes
            .Use(CandlePart.Close)
            .GetRoc(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<RocResult> results = Quotes
            .GetSma(2)
            .GetRoc(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetRoc(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<RocResult> r = BadQuotes
            .GetRoc(35)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Roc is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<RocResult> r0 = Noquotes
            .GetRoc(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<RocResult> r1 = Onequote
            .GetRoc(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<RocResult> results = Quotes
            .GetRoc(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        RocResult last = results.LastOrDefault();
        Assert.AreEqual(-8.2482, last.Roc.Round(4));
    }

    [TestMethod]
    public void Exceptions() =>
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetRoc(0));
}
