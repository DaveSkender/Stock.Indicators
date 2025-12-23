namespace StaticSeries;

[TestClass]
public class Chop : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<ChopResult> results = Quotes
            .ToChop();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(static x => x.Chop != null));

        // sample values
        ChopResult r1 = results[13];
        Assert.IsNull(r1.Chop);

        ChopResult r2 = results[14];
        Assert.AreEqual(69.9967, r2.Chop.Round(4));

        ChopResult r3 = results[249];
        Assert.AreEqual(41.8499, r3.Chop.Round(4));

        ChopResult r4 = results[501];
        Assert.AreEqual(38.6526, r4.Chop.Round(4));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToChop()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(479, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        const int lookbackPeriods = 2;
        IReadOnlyList<ChopResult> results = Quotes
            .ToChop(lookbackPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(500, results.Where(static x => x.Chop != null));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<ChopResult> results = Quotes
            .ToChop();

        results.IsBetween(static x => x.Chop, 0d, 100d);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ChopResult> r = BadQuotes
            .ToChop(20);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Chop is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ChopResult> r0 = Noquotes
            .ToChop();

        Assert.IsEmpty(r0);

        IReadOnlyList<ChopResult> r1 = Onequote
            .ToChop();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ChopResult> results = Quotes
            .ToChop()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 14, results);

        ChopResult last = results[^1];
        Assert.AreEqual(38.6526, last.Chop.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToChop(1));
}
