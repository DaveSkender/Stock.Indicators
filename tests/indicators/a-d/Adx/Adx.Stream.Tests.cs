using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class AdxStreamTests : TestBase
{
    [TestMethod]
    public void Manual()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // initialize
        Adx adx = new(14);

        // roll through history
        for (int i = 0; i < length; i++)
        {
            adx.Increment(quotesList[i]);
        }

        // results
        List<AdxResult> resultList = adx.ProtectedResults;

        // time-series, for comparison
        List<AdxResult> seriesList = quotes
            .GetAdx(14)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            AdxResult s = seriesList[i];
            AdxResult r = resultList[i];

            Assert.AreEqual(s.Date, r.Date);
            Assert.AreEqual(s.Pdi, r.Pdi);
            Assert.AreEqual(s.Mdi, r.Mdi);
            Assert.AreEqual(s.Adx, r.Adx);
            Assert.AreEqual(s.Adxr, r.Adxr);
        }
    }
}
