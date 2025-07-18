namespace StaticSeries;

[TestClass]
public class Mfi : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<MfiResult> results = Quotes
            .ToMfi();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[439];
        Assert.AreEqual(69.0622, r1.Mfi.Round(4));

        MfiResult r2 = results[501];
        Assert.AreEqual(39.9494, r2.Mfi.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToMfi()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 4;

        IReadOnlyList<MfiResult> results = Quotes
            .ToMfi(lookbackPeriods);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(498, results.Count(x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[31];
        Assert.AreEqual(100, r1.Mfi.Round(4));

        MfiResult r2 = results[43];
        Assert.AreEqual(0, r2.Mfi.Round(4));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<MfiResult> r = BadQuotes
            .ToMfi(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mfi is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<MfiResult> r0 = Noquotes
            .ToMfi();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<MfiResult> r1 = Onequote
            .ToMfi();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;

        IReadOnlyList<MfiResult> results = Quotes
            .ToMfi(lookbackPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        MfiResult last = results[^1];
        Assert.AreEqual(39.9494, last.Mfi.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(()
            => Quotes.ToMfi(1));
}
