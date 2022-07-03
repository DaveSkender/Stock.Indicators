using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Vwap : TestBase
{
    private readonly IEnumerable<Quote> intraday = TestData.GetIntraday()
        .OrderBy(x => x.Date)
        .Take(391);

    [TestMethod]
    public void Standard()
    {
        List<VwapResult> results = intraday.GetVwap()
            .ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(391, results.Count);
        Assert.AreEqual(391, results.Count(x => x.Vwap != null));

        // sample values
        VwapResult r1 = results[0];
        Assert.AreEqual(367.4800, NullMath.Round(r1.Vwap, 4));

        VwapResult r2 = results[1];
        Assert.AreEqual(367.4223, NullMath.Round(r2.Vwap, 4));

        VwapResult r3 = results[369];
        Assert.AreEqual(367.9494, NullMath.Round(r3.Vwap, 4));

        VwapResult r4 = results[390];
        Assert.AreEqual(368.1804, NullMath.Round(r4.Vwap, 4));
    }

    [TestMethod]
    public void WithStartDate()
    {
        DateTime startDate =
            DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", EnglishCulture);

        List<VwapResult> results = Indicator.GetVwap(intraday, startDate)
            .ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(391, results.Count);
        Assert.AreEqual(361, results.Count(x => x.Vwap != null));

        // sample values
        VwapResult r1 = results[29];
        Assert.AreEqual(null, r1.Vwap);

        VwapResult r2 = results[30];
        Assert.AreEqual(366.8100, NullMath.Round(r2.Vwap, 4));

        VwapResult r3 = results[369];
        Assert.AreEqual(368.0511, NullMath.Round(r3.Vwap, 4));

        VwapResult r4 = results[390];
        Assert.AreEqual(368.2908, NullMath.Round(r4.Vwap, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetVwap()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<VwapResult> r = Indicator.GetVwap(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Vwap is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<VwapResult> r0 = noquotes.GetVwap();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<VwapResult> r1 = onequote.GetVwap();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        // no start date
        List<VwapResult> results = intraday.GetVwap()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(391, results.Count);

        VwapResult last = results.LastOrDefault();
        Assert.AreEqual(368.1804, NullMath.Round(last.Vwap, 4));

        // with start date
        DateTime startDate =
        DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", EnglishCulture);

        List<VwapResult> sdResults = Indicator.GetVwap(intraday, startDate)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(361, sdResults.Count);

        VwapResult sdLast = sdResults.LastOrDefault();
        Assert.AreEqual(368.2908, NullMath.Round(sdLast.Vwap, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad SMA period
        DateTime startDate =
            DateTime.ParseExact("2000-12-15", "yyyy-MM-dd", EnglishCulture);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetVwap(quotes, startDate));
    }
}
