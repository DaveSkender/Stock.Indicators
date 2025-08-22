namespace Tests.Indicators;

[TestClass]
public class EpmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<EpmaResult> results = quotes
            .GetEpma(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
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
    public void UseTuple()
    {
        List<EpmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetEpma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<EpmaResult> r = tupleNanny
            .GetEpma(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.AreEqual(0, r.Count(x => x.Epma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<EpmaResult> results = quotes
            .GetSma(2)
            .GetEpma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(x => x.Epma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetEpma(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<EpmaResult> r = badQuotes
            .GetEpma(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.Epma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<EpmaResult> r0 = noquotes
            .GetEpma(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<EpmaResult> r1 = onequote
            .GetEpma(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<EpmaResult> results = quotes
            .GetEpma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 19, results);

        EpmaResult last = results.LastOrDefault();
        Assert.AreEqual(235.8131, last.Epma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(()
            => quotes.GetEpma(0));
}
