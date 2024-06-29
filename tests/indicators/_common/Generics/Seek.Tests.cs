namespace Tests.Common;

[TestClass]
public class Seeking : SeriesTestBase
{
    [TestMethod]
    public void FindSeries()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(20);

        // find specific date
        DateTime findDate
            = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", englishCulture);

        EmaResult r = emaResults.Find(findDate);
        Assert.AreEqual(249.3519, r.Ema.Round(4));
    }

    [TestMethod]
    public void FindSeriesNone()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(20);

        // find specific date
        DateTime findDate
            = DateTime.ParseExact("1928-10-29", "yyyy-MM-dd", englishCulture);

        Assert.ThrowsException<InvalidOperationException>(
            () => emaResults.Find(findDate));
    }

    [TestMethod]
    public void FindSeriesIndex()
    {
        List<Quote> quotes = TestData
            .GetDefault()
            .ToSortedList();

        // find specific date
        DateTime findDate
            = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", englishCulture);

        int i = quotes.FindIndex(findDate);
        Assert.AreEqual(501, i);
    }
}
