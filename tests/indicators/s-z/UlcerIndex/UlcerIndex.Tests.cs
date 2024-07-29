namespace StaticSeries;

[TestClass]
public class UlcerIndexTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .GetUlcerIndex();

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
            .GetUlcerIndex();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .GetSma(2)
            .GetUlcerIndex();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetUlcerIndex()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<UlcerIndexResult> r = BadQuotes
            .GetUlcerIndex(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UlcerIndex is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<UlcerIndexResult> r0 = Noquotes
            .GetUlcerIndex();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<UlcerIndexResult> r1 = Onequote
            .GetUlcerIndex();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UlcerIndexResult> results = Quotes
            .GetUlcerIndex()
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
            => Quotes.GetUlcerIndex(0));
}
