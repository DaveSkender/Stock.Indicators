namespace Tests.Indicators;

[TestClass]
public class HmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<HmaResult> results = quotes
            .GetHma(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));

        // sample values
        HmaResult r1 = results[149];
        Assert.AreEqual(236.0835, r1.Hma.Round(4));

        HmaResult r2 = results[501];
        Assert.AreEqual(235.6972, r2.Hma.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<HmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetHma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<HmaResult> r = tupleNanny
            .GetHma(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.Hma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<HmaResult> results = quotes
            .GetSma(2)
            .GetHma(19)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(480, results.Count(x => x.Hma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetHma(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(471, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<HmaResult> r = badQuotes
            .GetHma(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Hma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<HmaResult> r0 = noquotes
            .GetHma(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<HmaResult> r1 = onequote
            .GetHma(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<HmaResult> results = quotes
            .GetHma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(480, results);

        HmaResult last = results.LastOrDefault();
        Assert.AreEqual(235.6972, last.Hma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetHma(1));
}
