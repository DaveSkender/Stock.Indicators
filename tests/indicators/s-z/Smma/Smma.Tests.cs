namespace Tests.Indicators;

[TestClass]
public class SmmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<SmmaResult> results = quotes
            .GetSmma(20)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Smma != null));

        // starting calculations at proper index
        Assert.IsNull(results[18].Smma);
        Assert.IsNotNull(results[19].Smma);

        // sample values
        Assert.AreEqual(214.52500, Math.Round(results[19].Smma.Value, 5));
        Assert.AreEqual(214.55125, Math.Round(results[20].Smma.Value, 5));
        Assert.AreEqual(214.58319, Math.Round(results[21].Smma.Value, 5));
        Assert.AreEqual(225.78071, Math.Round(results[100].Smma.Value, 5));
        Assert.AreEqual(255.67462, Math.Round(results[501].Smma.Value, 5));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<SmmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetSmma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(x => x.Smma != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<SmmaResult> r = tupleNanny
            .GetSmma(6)
            .ToList();

        Assert.HasCount(200, r);
        Assert.IsEmpty(r.Where(x => x.Smma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void Chainee()
    {
        List<SmmaResult> results = quotes
            .GetSma(2)
            .GetSmma(20)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(x => x.Smma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetSmma(20)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<SmmaResult> r = badQuotes
            .GetSmma(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Smma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<SmmaResult> r0 = noquotes
            .GetSmma(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<SmmaResult> r1 = onequote
            .GetSmma(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<SmmaResult> results = quotes
            .GetSmma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - (20 + 100), results);
        Assert.AreEqual(255.67462, Math.Round(results.LastOrDefault().Smma.Value, 5));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetSmma(0));
}
