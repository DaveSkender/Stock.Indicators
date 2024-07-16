using System.Collections.ObjectModel;

namespace Utilities;

[TestClass]
public class QuoteUtilityTests : TestBase
{
    [TestMethod]
    public void QuoteToSortedCollection()
    {
        IReadOnlyList<Quote> quotes = Data.GetMismatch();

        Collection<Quote> h = quotes.ToSortedCollection();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }

    [TestMethod]
    public void QuoteToSortedList()
    {
        IReadOnlyList<Quote> quotes = Data.GetMismatch();

        List<Quote> h = quotes.ToSortedList();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }
}
