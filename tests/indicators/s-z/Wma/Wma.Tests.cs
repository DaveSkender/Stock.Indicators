namespace Tests.Indicators;

[TestClass]
public class WmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<WmaResult> results = quotes
            .GetWma(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Wma != null));

        // sample values
        WmaResult r1 = results[149];
        Assert.AreEqual(235.5253, r1.Wma.Round(4));

        WmaResult r2 = results[501];
        Assert.AreEqual(246.5110, r2.Wma.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<WmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetWma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(483, results.Count(x => x.Wma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<WmaResult> r = tupleNanny
            .GetWma(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.Wma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<WmaResult> results = quotes
            .GetSma(2)
            .GetWma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(482, results.Count(x => x.Wma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetWma(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chaining()
    {
        List<WmaResult> standard = quotes
            .GetWma(17)
            .ToList();

        List<WmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetWma(17)
            .ToList();

        // assertions
        for (int i = 0; i < results.Count; i++)
        {
            WmaResult s = standard[i];
            WmaResult c = results[i];

            Assert.AreEqual(s.Date, c.Date);
            Assert.AreEqual(s.Wma, c.Wma);
        }
    }

    [TestMethod]
    public void BadData()
    {
        List<WmaResult> r = badQuotes
            .GetWma(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Wma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<WmaResult> r0 = noquotes
            .GetWma(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<WmaResult> r1 = onequote
            .GetWma(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<WmaResult> results = quotes
            .GetWma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 19, results);

        WmaResult last = results.LastOrDefault();
        Assert.AreEqual(246.5110, last.Wma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetWma(0));
}
