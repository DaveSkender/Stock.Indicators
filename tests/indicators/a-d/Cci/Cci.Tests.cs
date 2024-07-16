namespace Series;

[TestClass]
public class CciTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<CciResult> results = Quotes
            .GetCci()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Cci != null));

        // sample value
        CciResult r = results[501];
        Assert.AreEqual(-52.9946, r.Cci.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetCci()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<CciResult> r = BadQuotes
            .GetCci(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Cci is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<CciResult> r0 = Noquotes
            .GetCci()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CciResult> r1 = Onequote
            .GetCci()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<CciResult> results = Quotes
            .GetCci()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CciResult last = results.LastOrDefault();
        Assert.AreEqual(-52.9946, last.Cci.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetCci(0));
}
