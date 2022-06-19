using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class QuoteHistory : TestBase
{
    [TestMethod]
    public void Validate()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();

        // clean
        List<Quote> h = quotes.Validate().ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, h.Count);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h[501].Date);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("02/01/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[20].Date);
    }

    [TestMethod]
    public void ValidateLong()
    {
        List<Quote> h = longishQuotes.Validate().ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(5285, h.Count);

        // check last date
        DateTime lastDate = DateTime.ParseExact("09/04/2020", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h[5284].Date);
    }

    [TestMethod]
    public void ValidateCut()
    {
        // if quotes post-cleaning, is cut down in size it should not corrupt the results

        IEnumerable<Quote> quotes = TestData.GetDefault(200);
        List<Quote> h = quotes.Validate().ToList();

        // assertions

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

    [TestMethod]
    public void Sort()
    {
        IEnumerable<Quote> quotes = TestData.GetMismatch();

        // clean
        List<Quote> h = quotes.ToSortedList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(firstDate, h[0].Date);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Date);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Date);
    }

    [TestMethod]
    public void Aggregate()
    {
        IEnumerable<Quote> quotes = TestData.GetIntraday();

        // aggregate
        List<Quote> results = quotes.Aggregate(PeriodSize.FifteenMinutes)
            .ToList();

        // assertions

        Assert.AreEqual(108, results.Count);

        // sample values
        Quote r0 = results[0];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:30", EnglishCulture), r0.Date);
        Assert.AreEqual(367.40m, r0.Open);
        Assert.AreEqual(367.775m, r0.High);
        Assert.AreEqual(367.02m, r0.Low);
        Assert.AreEqual(367.24m, r0.Close);
        Assert.AreEqual(2401786m, r0.Volume);

        Quote r1 = results[1];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:45", EnglishCulture), r1.Date);
        Assert.AreEqual(367.25m, r1.Open);
        Assert.AreEqual(367.44m, r1.High);
        Assert.AreEqual(366.69m, r1.Low);
        Assert.AreEqual(366.86m, r1.Close);
        Assert.AreEqual(1669983m, r1.Volume);

        Quote r2 = results[2];
        Assert.AreEqual(DateTime.Parse("2020-12-15 10:00", EnglishCulture), r2.Date);
        Assert.AreEqual(366.85m, r2.Open);
        Assert.AreEqual(367.17m, r2.High);
        Assert.AreEqual(366.57m, r2.Low);
        Assert.AreEqual(366.97m, r2.Close);
        Assert.AreEqual(1396993m, r2.Volume);

        // no history scenario
        List<Quote> noQuotes = new();
        IEnumerable<Quote> noResults = noQuotes.Aggregate(PeriodSize.Day);
        Assert.IsFalse(noResults.Any());
    }

    [TestMethod]
    public void AggregateTimeSpan()
    {
        IEnumerable<Quote> quotes = TestData.GetIntraday();

        // aggregate
        List<Quote> results = quotes.Aggregate(TimeSpan.FromMinutes(15))
            .ToList();

        // assertions

        Assert.AreEqual(108, results.Count);

        // sample values
        Quote r0 = results[0];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:30", EnglishCulture), r0.Date);
        Assert.AreEqual(367.40m, r0.Open);
        Assert.AreEqual(367.775m, r0.High);
        Assert.AreEqual(367.02m, r0.Low);
        Assert.AreEqual(367.24m, r0.Close);
        Assert.AreEqual(2401786m, r0.Volume);

        Quote r1 = results[1];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:45", EnglishCulture), r1.Date);
        Assert.AreEqual(367.25m, r1.Open);
        Assert.AreEqual(367.44m, r1.High);
        Assert.AreEqual(366.69m, r1.Low);
        Assert.AreEqual(366.86m, r1.Close);
        Assert.AreEqual(1669983m, r1.Volume);

        Quote r2 = results[2];
        Assert.AreEqual(DateTime.Parse("2020-12-15 10:00", EnglishCulture), r2.Date);
        Assert.AreEqual(366.85m, r2.Open);
        Assert.AreEqual(367.17m, r2.High);
        Assert.AreEqual(366.57m, r2.Low);
        Assert.AreEqual(366.97m, r2.Close);
        Assert.AreEqual(1396993m, r2.Volume);

        // no history scenario
        List<Quote> noQuotes = new();
        IEnumerable<Quote> noResults = noQuotes.Aggregate(TimeSpan.FromDays(1));
        Assert.IsFalse(noResults.Any());
    }

    [TestMethod]
    public void AggregateMonth()
    {
        // aggregate
        List<Quote> results = quotes.Aggregate(PeriodSize.Month)
            .ToList();

        // assertions

        Assert.AreEqual(24, results.Count);

        // sample values
        Quote r0 = results[0];
        Assert.AreEqual(DateTime.Parse("2017-01-01", EnglishCulture), r0.Date);
        Assert.AreEqual(212.61m, r0.Open);
        Assert.AreEqual(217.02m, r0.High);
        Assert.AreEqual(211.52m, r0.Low);
        Assert.AreEqual(214.96m, r0.Close);
        Assert.AreEqual(1569087580m, r0.Volume);

        Quote r1 = results[1];
        Assert.AreEqual(DateTime.Parse("2017-02-01", EnglishCulture), r1.Date);
        Assert.AreEqual(215.65m, r1.Open);
        Assert.AreEqual(224.20m, r1.High);
        Assert.AreEqual(214.29m, r1.Low);
        Assert.AreEqual(223.41m, r1.Close);
        Assert.AreEqual(1444958340m, r1.Volume);

        Quote r23 = results[23];
        Assert.AreEqual(DateTime.Parse("2018-12-01", EnglishCulture), r23.Date);
        Assert.AreEqual(273.47m, r23.Open);
        Assert.AreEqual(273.59m, r23.High);
        Assert.AreEqual(229.42m, r23.Low);
        Assert.AreEqual(245.28m, r23.Close);
        Assert.AreEqual(3173255968m, r23.Volume);
    }

    /* BAD QUOTES EXCEPTIONS */
    [TestMethod]
    [ExpectedException(typeof(InvalidQuotesException), "Duplicate date found.")]
    public void DuplicateHistory()
    {
        List<Quote> badHistory = new()
        {
            new Quote { Date = DateTime.ParseExact("2017-01-03", "yyyy-MM-dd", EnglishCulture), Open = 214.86m, High = 220.33m, Low = 210.96m, Close = 216.99m, Volume = 5923254 },
            new Quote { Date = DateTime.ParseExact("2017-01-04", "yyyy-MM-dd", EnglishCulture), Open = 214.75m, High = 228.00m, Low = 214.31m, Close = 226.99m, Volume = 11213471 },
            new Quote { Date = DateTime.ParseExact("2017-01-05", "yyyy-MM-dd", EnglishCulture), Open = 226.42m, High = 227.48m, Low = 221.95m, Close = 226.75m, Volume = 5911695 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", EnglishCulture), Open = 226.93m, High = 230.31m, Low = 225.45m, Close = 229.01m, Volume = 5527893 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", EnglishCulture), Open = 228.97m, High = 231.92m, Low = 228.00m, Close = 231.28m, Volume = 3979484 }
        };

        badHistory.Validate();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad aggregation size.")]
    public void BadAggregationSize()
    {
        // bad period size
        quotes.Aggregate(TimeSpan.Zero);
    }
}
