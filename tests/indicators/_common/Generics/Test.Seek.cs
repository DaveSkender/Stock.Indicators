using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class Seeking : TestBase
{
    [TestMethod]
    public void FindSeries()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(20);

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

        EmaResult r = emaResults.Find(findDate);
        Assert.AreEqual(249.3519, r.Ema.Round(4));
    }

    [TestMethod]
    public void FindSeriesNone()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(20);

        // find specific date
        DateTime findDate = DateTime.ParseExact("1928-10-29", "yyyy-MM-dd", EnglishCulture);

        EmaResult r = emaResults.Find(findDate);
        Assert.IsNull(r);
    }

    [TestMethod]
    public void FindSeriesIndex()
    {
        List<Quote> quotes = TestData
            .GetDefault()
            .ToSortedList();

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

        int i = quotes.FindIndex(findDate);
        Assert.AreEqual(501, i);
    }
}
