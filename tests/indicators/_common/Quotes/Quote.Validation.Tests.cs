namespace Utilities;

// quote validation

public partial class Quotes : TestBase
{
    [TestMethod]
    public void Validate()
    {
        IReadOnlyList<Quote> quotes = Data.GetDefault();

        IReadOnlyList<Quote> h = quotes.Validate();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // sample values
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, h[501].Timestamp);

        DateTime spotDate = DateTime.ParseExact("02/01/2017", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(spotDate, h[20].Timestamp);
    }

    [TestMethod]
    public void ValidateLong()
    {
        IReadOnlyList<Quote> h = LongishQuotes.Validate();

        // proper quantities
        Assert.AreEqual(5285, h.Count);

        // sample values
        DateTime lastDate = DateTime.ParseExact("09/04/2020", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, h[5284].Timestamp);
    }

    [TestMethod]
    public void ValidateCut()
    {
        // if quotes post-cleaning, is cut down in size it should not corrupt the results

        IReadOnlyList<Quote> quotes = Data.GetDefault(200);

        IReadOnlyList<Quote> h = quotes.Validate();

        // should be 200 periods, initially
        Assert.AreEqual(200, h.Count);

        // should be 20 results and no index corruption
        IReadOnlyList<SmaResult> r1 = h.TakeLast(20).GetSma(14).ToList();
        Assert.AreEqual(20, r1.Count);

        for (int i = 1; i < r1.Count; i++)
        {
            Assert.IsTrue(r1[i].Timestamp >= r1[i - 1].Timestamp);
        }

        // should be 50 results and no index corruption
        IReadOnlyList<SmaResult> r2 = h.TakeLast(50).GetSma(14).ToList();
        Assert.AreEqual(50, r2.Count);

        for (int i = 1; i < r2.Count; i++)
        {
            Assert.IsTrue(r2[i].Timestamp >= r2[i - 1].Timestamp);
        }

        // should be original 200 periods and no index corruption, after temp mods
        Assert.AreEqual(200, h.Count);

        for (int i = 1; i < h.Count; i++)
        {
            Assert.IsTrue(h[i].Timestamp >= h[i - 1].Timestamp);
        }
    }

    /* BAD QUOTES EXCEPTIONS */
    [TestMethod]
    public void ValidateDuplicates()
    {
        IReadOnlyList<Quote> badHistory = new List<Quote>
        {
            new(Timestamp: DateTime.ParseExact("2017-01-03", "yyyy-MM-dd", englishCulture), Open: 214.86m, High: 220.33m, Low: 210.96m, Close: 216.99m, Volume: 5923254),
            new(Timestamp: DateTime.ParseExact("2017-01-04", "yyyy-MM-dd", englishCulture), Open: 214.75m, High: 228.00m, Low: 214.31m, Close: 226.99m, Volume: 11213471),
            new(Timestamp: DateTime.ParseExact("2017-01-05", "yyyy-MM-dd", englishCulture), Open: 226.42m, High: 227.48m, Low: 221.95m, Close: 226.75m, Volume: 5911695),
            new(Timestamp: DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", englishCulture), Open: 226.93m, High: 230.31m, Low: 225.45m, Close: 229.01m, Volume: 5527893),
            new(Timestamp: DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", englishCulture), Open: 228.97m, High: 231.92m, Low: 228.00m, Close: 231.28m, Volume: 3979484)
        };

        Assert.ThrowsException<InvalidQuotesException>(() => badHistory.Validate());
    }
}
