using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class Seeking : TestBase
{
    [TestMethod]
    public void Find()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(20);

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

        EmaResult r = emaResults.Find(findDate);
        Assert.AreEqual(249.3519, r.Ema.Round(4));
    }
}
