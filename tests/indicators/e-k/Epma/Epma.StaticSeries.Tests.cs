namespace StaticSeries;

[TestClass]
public class Epma : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<EpmaResult> results = Quotes
            .GetEpma(20);

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
        IReadOnlyList<EpmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetEpma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<EpmaResult> results = Quotes
            .ToSma(2)
            .GetEpma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetEpma(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<EpmaResult> r = BadQuotes
            .GetEpma(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Epma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<EpmaResult> r0 = Noquotes
            .GetEpma(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<EpmaResult> r1 = Onequote
            .GetEpma(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<EpmaResult> results = Quotes
            .GetEpma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        EpmaResult last = results[^1];
        Assert.AreEqual(235.8131, last.Epma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetEpma(0));
}
