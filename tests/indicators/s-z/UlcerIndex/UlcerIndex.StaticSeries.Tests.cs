namespace StaticSeries;

[TestClass]
public class UlcerIndex : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .ToUlcerIndex();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.UlcerIndex != null));

        // sample value
        UlcerIndexResult r = results[501];
        Assert.AreEqual(5.7255, r.UlcerIndex.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .Use(CandlePart.Close)
            .ToUlcerIndex();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .ToSma(2)
            .ToUlcerIndex();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToUlcerIndex()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<UlcerIndexResult> r = BadQuotes
            .ToUlcerIndex(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UlcerIndex is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<UlcerIndexResult> r0 = Noquotes
            .ToUlcerIndex();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<UlcerIndexResult> r1 = Onequote
            .ToUlcerIndex();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .ToUlcerIndex()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        UlcerIndexResult last = results[^1];
        Assert.AreEqual(5.7255, last.UlcerIndex.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToUlcerIndex(0));
}
