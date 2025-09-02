namespace Tests.Indicators;

[TestClass]
public class CciTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CciResult> results = quotes
            .GetCci(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Cci != null));

        // sample value
        CciResult r = results[501];
        Assert.AreEqual(-52.9946, r.Cci.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetCci(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<CciResult> r = badQuotes
            .GetCci(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Cci is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CciResult> r0 = noquotes
            .GetCci()
            .ToList();

        Assert.IsEmpty(r0);

        List<CciResult> r1 = onequote
            .GetCci()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<CciResult> results = quotes
            .GetCci(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 19, results);

        CciResult last = results.LastOrDefault();
        Assert.AreEqual(-52.9946, last.Cci.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetCci(0));
}
