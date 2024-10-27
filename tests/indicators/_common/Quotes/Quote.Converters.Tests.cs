using System.Collections.ObjectModel;

// quote list converters

namespace Utilities;

[TestClass]
public partial class Quotes : TestBase
{
    [TestMethod]
    public void ToSortedList()
    {
        IReadOnlyList<Quote> quotes = Data.GetMismatch();

        IReadOnlyList<Quote> h = quotes.ToSortedList();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", TestBase.invariantCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", TestBase.invariantCulture);
        Assert.AreEqual(lastDate, h[^1].Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", TestBase.invariantCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }
}
