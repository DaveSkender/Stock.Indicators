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
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Pvi != null));

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

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Pvi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<VortexResult> r0 = Noquotes
            .ToVortex(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<VortexResult> r1 = Onequote
            .ToVortex(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<VortexResult> results = Quotes
            .ToVortex(14)
            .Condense();

        // assertions
        Assert.HasCount(502 - 14, results);

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
        Assert.HasCount(502 - 14, results);

        VortexResult last = results[^1];
        Assert.AreEqual(0.8712, last.Pvi.Round(4));
        Assert.AreEqual(1.1163, last.Nvi.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToVortex(1));
}
