namespace Tests.Indicators.Series;

[TestClass]
public class UlcerIndexTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<UlcerIndexResult> results = Quotes
            .GetUlcerIndex()
            .ToList();

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
        List<UlcerIndexResult> results = Quotes
            .Use(CandlePart.Close)
            .GetUlcerIndex()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<UlcerIndexResult> results = Quotes
            .GetSma(2)
            .GetUlcerIndex()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.UlcerIndex != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetUlcerIndex()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<UlcerIndexResult> r = BadQuotes
            .GetUlcerIndex(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UlcerIndex is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<UlcerIndexResult> r0 = Noquotes
            .GetUlcerIndex()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<UlcerIndexResult> r1 = Onequote
            .GetUlcerIndex()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<UlcerIndexResult> results = Quotes
            .GetUlcerIndex()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        UlcerIndexResult last = results.LastOrDefault();
        Assert.AreEqual(5.7255, last.UlcerIndex.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetUlcerIndex(0));
}
