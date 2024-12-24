namespace StaticSeries;

[TestClass]
public class Pmo : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<PmoResult> results = Quotes
            .ToPmo();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Count(x => x.Pmo != null));
        Assert.AreEqual(439, results.Count(x => x.Signal != null));

        // sample values
        PmoResult r1 = results[92];
        Assert.AreEqual(0.6159, r1.Pmo.Round(4));
        Assert.AreEqual(0.5582, r1.Signal.Round(4));

        PmoResult r2 = results[501];
        Assert.AreEqual(-2.7016, r2.Pmo.Round(4));
        Assert.AreEqual(-2.3117, r2.Signal.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<PmoResult> results = Quotes
            .Use(CandlePart.Close)
            .ToPmo();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Count(x => x.Pmo != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<PmoResult> results = Quotes
            .ToSma(2)
            .ToPmo();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(447, results.Count(x => x.Pmo != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToPmo()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(439, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<PmoResult> r = BadQuotes
            .ToPmo(25, 15, 5);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pmo is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<PmoResult> r0 = Noquotes
            .ToPmo();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<PmoResult> r1 = Onequote
            .ToPmo();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<PmoResult> results = Quotes
            .ToPmo()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (35 + 20 + 250), results.Count);

        PmoResult last = results[^1];
        Assert.AreEqual(-2.7016, last.Pmo.Round(4));
        Assert.AreEqual(-2.3117, last.Signal.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad time period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToPmo(1));

        // bad smoothing period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToPmo(5, 0));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToPmo(5, 5, 0));
    }
}
