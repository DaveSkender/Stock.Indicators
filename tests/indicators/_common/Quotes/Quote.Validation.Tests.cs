using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class QuoteValidation : TestBase
{
    [TestMethod]
    public void Validate()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();

        List<Quote> h = quotes.Validate().ToList();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // sample values
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h[501].Date);

        DateTime spotDate = DateTime.ParseExact("02/01/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[20].Date);
    }

    [TestMethod]
    public void ValidateLong()
    {
        List<Quote> h = longishQuotes.Validate().ToList();

        // proper quantities
        Assert.AreEqual(5285, h.Count);

        // sample values
        DateTime lastDate = DateTime.ParseExact("09/04/2020", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h[5284].Date);
    }

    [TestMethod]
    public void ValidateCut()
    {
        // if quotes post-cleaning, is cut down in size it should not corrupt the results

        IEnumerable<Quote> quotes = TestData.GetDefault(200);
        List<Quote> h = quotes.Validate().ToList();

        // should be 200 periods, initially
        Assert.AreEqual(200, h.Count);

        // should be 20 results and no index corruption
        List<SmaResult> r1 = Indicator.GetSma(h.TakeLast(20), 14).ToList();
        Assert.AreEqual(20, r1.Count);

        for (int i = 1; i < r1.Count; i++)
        {
            Assert.IsTrue(r1[i].Date >= r1[i - 1].Date);
        }

        // should be 50 results and no index corruption
        List<SmaResult> r2 = Indicator.GetSma(h.TakeLast(50), 14).ToList();
        Assert.AreEqual(50, r2.Count);

        for (int i = 1; i < r2.Count; i++)
        {
            Assert.IsTrue(r2[i].Date >= r2[i - 1].Date);
        }

        // should be original 200 periods and no index corruption, after temp mods
        Assert.AreEqual(200, h.Count);

        for (int i = 1; i < h.Count; i++)
        {
            Assert.IsTrue(h[i].Date >= h[i - 1].Date);
        }
    }

    /* BAD QUOTES EXCEPTIONS */
    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Duplicate date found.")]
    public void DuplicateHistory()
    {
        List<Quote> badHistory =
        [
            new Quote { Date = DateTime.ParseExact("2017-01-03", "yyyy-MM-dd", EnglishCulture), Open = 214.86m, High = 220.33m, Low = 210.96m, Close = 216.99m, Volume = 5923254 },
            new Quote { Date = DateTime.ParseExact("2017-01-04", "yyyy-MM-dd", EnglishCulture), Open = 214.75m, High = 228.00m, Low = 214.31m, Close = 226.99m, Volume = 11213471 },
            new Quote { Date = DateTime.ParseExact("2017-01-05", "yyyy-MM-dd", EnglishCulture), Open = 226.42m, High = 227.48m, Low = 221.95m, Close = 226.75m, Volume = 5911695 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", EnglishCulture), Open = 226.93m, High = 230.31m, Low = 225.45m, Close = 229.01m, Volume = 5527893 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", EnglishCulture), Open = 228.97m, High = 231.92m, Low = 228.00m, Close = 231.28m, Volume = 3979484 }
        ];

        badHistory.Validate();
    }
}
