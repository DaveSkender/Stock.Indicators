namespace Utilities;

[TestClass]
public class QuoteAggregateTests : TestBase
{
    [TestMethod]
    public void Aggregate()
    {
        IReadOnlyList<Quote> quotes = Data.GetIntraday();

        // aggregate
        List<Quote> results = quotes
            .Aggregate(PeriodSize.FifteenMinutes)
            .ToList();

        // proper quantities
        Assert.AreEqual(108, results.Count);

        // sample values
        Quote r0 = results[0];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:30", englishCulture), r0.Timestamp);
        Assert.AreEqual(367.40m, r0.Open);
        Assert.AreEqual(367.775m, r0.High);
        Assert.AreEqual(367.02m, r0.Low);
        Assert.AreEqual(367.24m, r0.Close);
        Assert.AreEqual(2401786m, r0.Volume);

        Quote r1 = results[1];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:45", englishCulture), r1.Timestamp);
        Assert.AreEqual(367.25m, r1.Open);
        Assert.AreEqual(367.44m, r1.High);
        Assert.AreEqual(366.69m, r1.Low);
        Assert.AreEqual(366.86m, r1.Close);
        Assert.AreEqual(1669983m, r1.Volume);

        Quote r2 = results[2];
        Assert.AreEqual(DateTime.Parse("2020-12-15 10:00", englishCulture), r2.Timestamp);
        Assert.AreEqual(366.85m, r2.Open);
        Assert.AreEqual(367.17m, r2.High);
        Assert.AreEqual(366.57m, r2.Low);
        Assert.AreEqual(366.97m, r2.Close);
        Assert.AreEqual(1396993m, r2.Volume);

        // no history scenario
        List<Quote> noQuotes = [];
        IEnumerable<Quote> noResults = noQuotes.Aggregate(PeriodSize.Day);
        Assert.IsFalse(noResults.Any());
    }

    [TestMethod]
    public void AggregateTimeSpan()
    {
        IReadOnlyList<Quote> quotes = Data.GetIntraday();

        // aggregate
        List<Quote> results = quotes
            .Aggregate(TimeSpan.FromMinutes(15))
            .ToList();

        // proper quantities
        Assert.AreEqual(108, results.Count);

        // sample values
        Quote r0 = results[0];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:30", englishCulture), r0.Timestamp);
        Assert.AreEqual(367.40m, r0.Open);
        Assert.AreEqual(367.775m, r0.High);
        Assert.AreEqual(367.02m, r0.Low);
        Assert.AreEqual(367.24m, r0.Close);
        Assert.AreEqual(2401786m, r0.Volume);

        Quote r1 = results[1];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:45", englishCulture), r1.Timestamp);
        Assert.AreEqual(367.25m, r1.Open);
        Assert.AreEqual(367.44m, r1.High);
        Assert.AreEqual(366.69m, r1.Low);
        Assert.AreEqual(366.86m, r1.Close);
        Assert.AreEqual(1669983m, r1.Volume);

        Quote r2 = results[2];
        Assert.AreEqual(DateTime.Parse("2020-12-15 10:00", englishCulture), r2.Timestamp);
        Assert.AreEqual(366.85m, r2.Open);
        Assert.AreEqual(367.17m, r2.High);
        Assert.AreEqual(366.57m, r2.Low);
        Assert.AreEqual(366.97m, r2.Close);
        Assert.AreEqual(1396993m, r2.Volume);

        // no history scenario
        List<Quote> noQuotes = [];
        IEnumerable<Quote> noResults = noQuotes.Aggregate(TimeSpan.FromDays(1));
        Assert.IsFalse(noResults.Any());
    }

    [TestMethod]
    public void AggregateMonth()
    {
        // aggregate
        List<Quote> results = Quotes
            .Aggregate(PeriodSize.Month)
            .ToList();

        // proper quantities
        Assert.AreEqual(24, results.Count);

        // sample values
        Quote r0 = results[0];
        Assert.AreEqual(DateTime.Parse("2017-01-01", englishCulture), r0.Timestamp);
        Assert.AreEqual(212.61m, r0.Open);
        Assert.AreEqual(217.02m, r0.High);
        Assert.AreEqual(211.52m, r0.Low);
        Assert.AreEqual(214.96m, r0.Close);
        Assert.AreEqual(1569087580m, r0.Volume);

        Quote r1 = results[1];
        Assert.AreEqual(DateTime.Parse("2017-02-01", englishCulture), r1.Timestamp);
        Assert.AreEqual(215.65m, r1.Open);
        Assert.AreEqual(224.20m, r1.High);
        Assert.AreEqual(214.29m, r1.Low);
        Assert.AreEqual(223.41m, r1.Close);
        Assert.AreEqual(1444958340m, r1.Volume);

        Quote r23 = results[23];
        Assert.AreEqual(DateTime.Parse("2018-12-01", englishCulture), r23.Timestamp);
        Assert.AreEqual(273.47m, r23.Open);
        Assert.AreEqual(273.59m, r23.High);
        Assert.AreEqual(229.42m, r23.Low);
        Assert.AreEqual(245.28m, r23.Close);
        Assert.AreEqual(3173255968m, r23.Volume);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad aggregation size.")]
    public void BadAggregationSize() =>

    // bad period size
    Quotes.Aggregate(TimeSpan.Zero);
}
