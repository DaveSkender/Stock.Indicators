namespace StaticSeries;

[TestClass]
public class WilliamsR : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<WilliamsResult> results = Quotes
            .ToWilliamsR();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(static x => x.WilliamsR != null));

        // sample values
        WilliamsResult r1 = results[343];
        Assert.AreEqual(-19.8211, r1.WilliamsR.Round(4));

        WilliamsResult r2 = results[501];
        Assert.AreEqual(-52.0121, r2.WilliamsR.Round(4));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<WilliamsResult> results = Quotes.ToWilliamsR(14);
        results.IsBetween(x => x.WilliamsR, -100, 0);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToWilliamsR()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<WilliamsResult> results = BadQuotes
            .ToWilliamsR(20);

        Assert.HasCount(502, results);
        Assert.IsEmpty(results.Where(static x => x.WilliamsR is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<WilliamsResult> r0 = Noquotes
            .ToWilliamsR();

        Assert.IsEmpty(r0);

        IReadOnlyList<WilliamsResult> r1 = Onequote
            .ToWilliamsR();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<WilliamsResult> results = Quotes
            .ToWilliamsR()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 13, results);

        WilliamsResult last = results[^1];
        Assert.AreEqual(-52.0121, last.WilliamsR.Round(4));
    }

    [TestMethod]
    public void Boundary()
    {
        IReadOnlyList<WilliamsResult> results = Data
            .GetRandom(2500)
            .ToWilliamsR();

        results.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    [TestMethod]
    public void Issue1127_Original_BoundaryThreshold_Maintained()
    {
        // initialize
        IReadOnlyList<Quote> quotes = Data.QuotesFromCsv("_issue1127.williamr.original.csv");

        // get indicators
        IReadOnlyList<WilliamsResult> results = quotes
            .ToWilliamsR();

        results.Should().HaveCountGreaterThan(0);
        results.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    [TestMethod]
    public void Issue1127_Revisit_BoundaryThreshold_Maintained()
    {
        // initialize
        IReadOnlyList<Quote> quotes = Data.QuotesFromCsv("_issue1127.williamr.revisit.csv");

        // get indicators
        IReadOnlyList<WilliamsResult> results = quotes
            .ToWilliamsR();

        results.ToConsole(args: (nameof(WilliamsResult.WilliamsR), "F20"));

        results.Should().HaveCountGreaterThan(0);
        results.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToWilliamsR(0));
}
