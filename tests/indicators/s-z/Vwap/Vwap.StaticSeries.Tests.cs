namespace StaticSeries;

[TestClass]
public class Vwap : StaticSeriesTestBase
{
    private static readonly IReadOnlyList<Quote> intraday = Data.GetIntraday()
        .OrderBy(static x => x.Timestamp)
        .Take(391)
        .ToList();

    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<VwapResult> results = intraday.ToVwap();

        // proper quantities
        Assert.HasCount(391, results);
        Assert.HasCount(391, results.Where(static x => x.Vwap != null));

        // sample values
        VwapResult r1 = results[0];
        Assert.AreEqual(367.4800, r1.Vwap.Round(4));

        VwapResult r2 = results[1];
        Assert.AreEqual(367.4223, r2.Vwap.Round(4));

        VwapResult r3 = results[369];
        Assert.AreEqual(367.9494, r3.Vwap.Round(4));

        VwapResult r4 = results[390];
        Assert.AreEqual(368.1804, r4.Vwap.Round(4));
    }

    [TestMethod]
    public void WithStartDate()
    {
        DateTime startDate =
            DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", invariantCulture);

        IReadOnlyList<VwapResult> results = intraday
            .ToVwap(startDate);

        // proper quantities
        Assert.HasCount(391, results);
        Assert.HasCount(361, results.Where(static x => x.Vwap != null));

        // sample values
        VwapResult r1 = results[29];
        Assert.IsNull(r1.Vwap);

        VwapResult r2 = results[30];
        Assert.AreEqual(366.8100, r2.Vwap.Round(4));

        VwapResult r3 = results[369];
        Assert.AreEqual(368.0511, r3.Vwap.Round(4));

        VwapResult r4 = results[390];
        Assert.AreEqual(368.2908, r4.Vwap.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToVwap()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<VwapResult> r = BadQuotes
            .ToVwap();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Vwap is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<VwapResult> r0 = Noquotes
            .ToVwap();

        Assert.IsEmpty(r0);

        IReadOnlyList<VwapResult> r1 = Onequote
            .ToVwap();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        // no start date
        IReadOnlyList<VwapResult> results = intraday
            .ToVwap()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(391, results);

        VwapResult last = results[^1];
        Assert.AreEqual(368.1804, last.Vwap.Round(4));

        // with start date
        DateTime startDate =
        DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", invariantCulture);

        IReadOnlyList<VwapResult> sdResults = intraday
            .ToVwap(startDate)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(361, sdResults);

        VwapResult sdLast = sdResults[^1];
        Assert.AreEqual(368.2908, sdLast.Vwap.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad SMA period
        DateTime startDate =
            DateTime.ParseExact("2000-12-15", "yyyy-MM-dd", invariantCulture);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => intraday.ToVwap(startDate));
    }
}
