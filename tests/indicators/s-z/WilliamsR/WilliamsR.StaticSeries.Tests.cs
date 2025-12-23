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

        // analyze boundary
        for (int i = 0; i < results.Count; i++)
        {
            WilliamsResult r = results[i];

            r.WilliamsR?.Should().BeInRange(-100d, 0d);
        }
    }

    [TestMethod]
    public void Issue1127_Revisit_BoundaryThreshold_Maintained()
    {
        // initialize
        IReadOnlyList<Quote> quotes
            = File.ReadAllLines("_testdata/issues/issue1127.quotes.williamr.revisit.csv")
                .Skip(1)
                .Select(Test.Data.Utilities.QuoteFromCsv)
                .OrderByDescending(static x => x.Timestamp)
                .ToList();

        int length = quotes.Count;

        // get indicators
        IReadOnlyList<WilliamsResult> resultsList = quotes
            .ToWilliamsR();

        Console.WriteLine($"%R from {length} quotes.");

        // analyze boundary
        for (int i = 0; i < length; i++)
        {
            Quote q = quotes[i];
            WilliamsResult r = resultsList[i];

            Console.WriteLine($"{q.Timestamp:s} {r.WilliamsR}");

            r.WilliamsR?.Should().BeInRange(-100d, 0d);
        }
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToWilliamsR(0));
}
