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
        IReadOnlyList<WilliamsResult> results = Quotes
            .ToWilliamsR();

        results.IsBetween(static x => x.WilliamsR, -100d, 0d);
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
        IReadOnlyList<Quote> quotes = File.ReadAllLines("_testdata/issues/issue1127.quotes.williamr.original.csv")
            .Skip(1)
            .Select(Test.Data.Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .ToList();

        int length = quotes.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> results = quotes
            .ToWilliamsR();

        results.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    [TestMethod]
    public void Issue1127_Revisit_BoundaryThreshold_Maintained()
    {
        // initialize
        IReadOnlyList<Quote> quotes = File.ReadAllLines("_testdata/issues/issue1127.quotes.williamr.revisit.csv")
            .Skip(1)
            .Select(Test.Data.Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .ToList();

        int length = quotes.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> results = quotes
            .ToWilliamsR();

        Dictionary<string, string> args = new()
        {
            { "WilliamsR", "N20" }
        };

        Console.WriteLine(results.ToStringOut(args));

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
