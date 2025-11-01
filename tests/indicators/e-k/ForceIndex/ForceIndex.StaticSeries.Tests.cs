namespace StaticSeries;

[TestClass]
public class ForceIndex : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<ForceIndexResult> r = Quotes.ToForceIndex(13).ToList();

        // proper quantities
        Assert.HasCount(502, r);
        Assert.HasCount(489, r.Where(static x => x.ForceIndex != null));

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

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ForceIndexResult> r = BadQuotes
            .ToForceIndex();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.ForceIndex is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ForceIndexResult> r0 = Noquotes
            .ToForceIndex(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<ForceIndexResult> r1 = Onequote
            .ToForceIndex(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ForceIndexResult> results = Quotes
            .ToForceIndex(13)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (13 + 100), results);

        ForceIndexResult last = results[^1];
        Assert.AreEqual(-16824018.428, Math.Round(last.ForceIndex.Value, 3));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToForceIndex(0));
}
