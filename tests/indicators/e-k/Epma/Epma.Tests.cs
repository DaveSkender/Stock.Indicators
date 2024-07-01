namespace Tests.Indicators.Series;

[TestClass]
public class EpmaTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<EpmaResult> results = Quotes
            .GetEpma(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Epma != null));

        // sample values
        EpmaResult r1 = results[18];
        Assert.IsNull(r1.Epma);

        EpmaResult r2 = results[19];
        Assert.AreEqual(215.6189, r2.Epma.Round(4));

        EpmaResult r3 = results[149];
        Assert.AreEqual(236.7060, r3.Epma.Round(4));

        EpmaResult r4 = results[249];
        Assert.AreEqual(258.5179, r4.Epma.Round(4));

        EpmaResult r5 = results[501];
        Assert.AreEqual(235.8131, r5.Epma.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        List<EpmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetEpma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<EpmaResult> results = Quotes
            .GetSma(2)
            .GetEpma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetEpma(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<EpmaResult> r = BadQuotes
            .GetEpma(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Epma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<EpmaResult> r0 = Noquotes
            .GetEpma(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<EpmaResult> r1 = Onequote
            .GetEpma(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<EpmaResult> results = Quotes
            .GetEpma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        EpmaResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, last.Epma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetEpma(0));
}
