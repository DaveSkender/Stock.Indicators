namespace StaticSeries;

[TestClass]
public class Mfi : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<MfiResult> results = Quotes
            .ToMfi();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[439];
        Assert.AreEqual(69.0622, r1.Mfi.Round(4));

        MfiResult r2 = results[501];
        Assert.AreEqual(39.9494, r2.Mfi.Round(4));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<MfiResult> results = Quotes.ToMfi(14);
        TestAsserts.AlwaysBounded(results, x => x.Mfi, 0, 100);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToMfi()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(479, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        const int lookbackPeriods = 4;

        IReadOnlyList<MfiResult> results = Quotes
            .ToMfi(lookbackPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(498, results.Where(static x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[31];
        Assert.AreEqual(100, r1.Mfi.Round(4));

        MfiResult r2 = results[43];
        Assert.AreEqual(0, r2.Mfi.Round(4));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<MfiResult> r = BadQuotes
            .ToMfi(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Mfi is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<MfiResult> r0 = Noquotes
            .ToMfi();

        Assert.IsEmpty(r0);

        IReadOnlyList<MfiResult> r1 = Onequote
            .ToMfi();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        const int lookbackPeriods = 14;

        IReadOnlyList<MfiResult> results = Quotes
            .ToMfi(lookbackPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 14, results);

        MfiResult last = results[^1];
        Assert.AreEqual(39.9494, last.Mfi.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMfi(1));
}
