namespace StaticSeries;

[TestClass]
public class Vwap : StaticSeriesTestBase
{
    private readonly IEnumerable<Quote> intraday = Data.GetIntraday()
        .OrderBy(x => x.Timestamp)
        .Take(391);

    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<VwapResult> results = intraday.GetVwap();

        // proper quantities
        Assert.AreEqual(391, results.Count);
        Assert.AreEqual(391, results.Count(x => x.Vwap != null));

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
            DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", englishCulture);

        IReadOnlyList<VwapResult> results = intraday
            .GetVwap(startDate);

        // proper quantities
        Assert.AreEqual(391, results.Count);
        Assert.AreEqual(361, results.Count(x => x.Vwap != null));

        // sample values
        VwapResult r1 = results[29];
        Assert.AreEqual(null, r1.Vwap);

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
            .GetVwap()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<VwapResult> r = BadQuotes
            .GetVwap();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Vwap is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<VwapResult> r0 = Noquotes
            .GetVwap();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<VwapResult> r1 = Onequote
            .GetVwap();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        // no start date
        IReadOnlyList<VwapResult> results = intraday
            .GetVwap()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(391, results.Count);

        VwapResult last = results[^1];
        Assert.AreEqual(368.1804, last.Vwap.Round(4));

        // with start date
        DateTime startDate =
        DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", englishCulture);

        IReadOnlyList<VwapResult> sdResults = intraday
            .GetVwap(startDate)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(361, sdResults.Count);

        VwapResult sdLast = sdResults[^1];
        Assert.AreEqual(368.2908, sdLast.Vwap.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad SMA period
        DateTime startDate =
            DateTime.ParseExact("2000-12-15", "yyyy-MM-dd", englishCulture);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetVwap(startDate));
    }
}
