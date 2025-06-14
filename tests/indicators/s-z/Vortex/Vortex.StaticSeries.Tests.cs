namespace StaticSeries;

[TestClass]
public class Vortex : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<VortexResult> results = Quotes
            .ToVortex(14);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Pvi != null));

        // sample values
        VortexResult r1 = results[13];
        Assert.IsNull(r1.Pvi);
        Assert.IsNull(r1.Nvi);

        VortexResult r2 = results[14];
        Assert.AreEqual(1.0460, r2.Pvi.Round(4));
        Assert.AreEqual(0.8119, r2.Nvi.Round(4));

        VortexResult r3 = results[29];
        Assert.AreEqual(1.1300, r3.Pvi.Round(4));
        Assert.AreEqual(0.7393, r3.Nvi.Round(4));

        VortexResult r4 = results[249];
        Assert.AreEqual(1.1558, r4.Pvi.Round(4));
        Assert.AreEqual(0.6634, r4.Nvi.Round(4));

        VortexResult r5 = results[501];
        Assert.AreEqual(0.8712, r5.Pvi.Round(4));
        Assert.AreEqual(1.1163, r5.Nvi.Round(4));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<VortexResult> r = BadQuotes
            .ToVortex(20);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pvi is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<VortexResult> r0 = Noquotes
            .ToVortex(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<VortexResult> r1 = Onequote
            .ToVortex(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<VortexResult> results = Quotes
            .ToVortex(14)
            .Condense();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        VortexResult last = results[^1];
        Assert.AreEqual(0.8712, last.Pvi.Round(4));
        Assert.AreEqual(1.1163, last.Nvi.Round(4));
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<VortexResult> results = Quotes
            .ToVortex(14)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        VortexResult last = results[^1];
        Assert.AreEqual(0.8712, last.Pvi.Round(4));
        Assert.AreEqual(1.1163, last.Nvi.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToVortex(1));
}
