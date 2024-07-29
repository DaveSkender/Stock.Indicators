namespace StaticSeries;

[TestClass]
public class SmmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .GetSmma(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Smma != null));

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
    public void UseReusable()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetSmma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Smma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .GetSma(2)
            .GetSmma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Smma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetSmma(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<SmmaResult> r = BadQuotes
            .GetSmma(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Smma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<SmmaResult> r0 = Noquotes
            .GetSmma(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<SmmaResult> r1 = Onequote
            .GetSmma(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmmaResult> results = Quotes
            .GetSmma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (20 + 100), results.Count);
        Assert.AreEqual(255.67462, Math.Round(results[^1].Smma.Value, 5));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetSmma(0));
}
