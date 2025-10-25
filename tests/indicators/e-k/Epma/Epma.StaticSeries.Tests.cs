namespace StaticSeries;

[TestClass]
public class Epma : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<EpmaResult> results = Quotes
            .ToEpma(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Epma != null));

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
            .ToEpma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<EpmaResult> results = Quotes
            .ToSma(2)
            .ToEpma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToEpma(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<EpmaResult> r = BadQuotes
            .ToEpma(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Epma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<EpmaResult> r0 = Noquotes
            .ToEpma(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<EpmaResult> r1 = Onequote
            .ToEpma(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<EpmaResult> results = Quotes
            .ToEpma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);

        EpmaResult last = results[^1];
        Assert.AreEqual(235.8131, last.Epma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToEpma(0));
}
