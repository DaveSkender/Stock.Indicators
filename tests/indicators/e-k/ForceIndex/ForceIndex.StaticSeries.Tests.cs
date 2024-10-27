namespace StaticSeries;

[TestClass]
public class ForceIndex : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<ForceIndexResult> r = Quotes.ToForceIndex(13).ToList();

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
        IReadOnlyList<SmaResult> results = Quotes
            .ToForceIndex(13)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ForceIndexResult> r = BadQuotes
            .ToForceIndex();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.ForceIndex is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ForceIndexResult> r0 = Noquotes
            .ToForceIndex(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ForceIndexResult> r1 = Onequote
            .ToForceIndex(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ForceIndexResult> results = Quotes
            .ToForceIndex(13)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (13 + 100), results.Count);

        ForceIndexResult last = results[^1];
        Assert.AreEqual(-16824018.428, Math.Round(last.ForceIndex.Value, 3));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToForceIndex(0));
}
