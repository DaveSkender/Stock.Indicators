namespace Series;

[TestClass]
public class ForceIndexTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<ForceIndexResult> r = Quotes.GetForceIndex(13).ToList();

        // proper quantities
        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(489, r.Count(x => x.ForceIndex != null));

        // sample values
        Assert.IsNull(r[12].ForceIndex);

        Assert.AreEqual(10668240.778, Math.Round(r[13].ForceIndex.Value, 3));
        Assert.AreEqual(15883211.364, Math.Round(r[24].ForceIndex.Value, 3));
        Assert.AreEqual(7598218.196, Math.Round(r[149].ForceIndex.Value, 3));
        Assert.AreEqual(23612118.994, Math.Round(r[249].ForceIndex.Value, 3));
        Assert.AreEqual(-16824018.428, Math.Round(r[501].ForceIndex.Value, 3));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetForceIndex(13)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<ForceIndexResult> r = BadQuotes
            .GetForceIndex()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.ForceIndex is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<ForceIndexResult> r0 = Noquotes
            .GetForceIndex(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<ForceIndexResult> r1 = Onequote
            .GetForceIndex(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<ForceIndexResult> results = Quotes
            .GetForceIndex(13)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (13 + 100), results.Count);

        ForceIndexResult last = results.LastOrDefault();
        Assert.AreEqual(-16824018.428, Math.Round(last.ForceIndex.Value, 3));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetForceIndex(0));
}
