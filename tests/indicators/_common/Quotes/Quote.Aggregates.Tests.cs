namespace Utilities;

// quote aggregates

public partial class Quotes : TestBase
{
    [TestMethod]
    public void Aggregate()
    {
        IReadOnlyList<Quote> quotes = Data.GetIntraday();

        // aggregate
        IReadOnlyList<Quote> sut = quotes
            .Aggregate(PeriodSize.FifteenMinutes);

        // proper quantities
        sut.Should().HaveCount(108);

        // sample values
        Quote r0 = sut[0];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:30", invariantCulture), r0.Timestamp);
        r0.Open.Should().Be(367.40m);
        r0.High.Should().Be(367.775m);
        r0.Low.Should().Be(367.02m);
        r0.Close.Should().Be(367.24m);
        r0.Volume.Should().Be(2401786m);

        Quote r1 = sut[1];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:45", invariantCulture), r1.Timestamp);
        r1.Open.Should().Be(367.25m);
        r1.High.Should().Be(367.44m);
        r1.Low.Should().Be(366.69m);
        r1.Close.Should().Be(366.86m);
        r1.Volume.Should().Be(1669983m);

        Quote r2 = sut[2];
        Assert.AreEqual(DateTime.Parse("2020-12-15 10:00", invariantCulture), r2.Timestamp);
        r2.Open.Should().Be(366.85m);
        r2.High.Should().Be(367.17m);
        r2.Low.Should().Be(366.57m);
        r2.Close.Should().Be(366.97m);
        r2.Volume.Should().Be(1396993m);

        // no history scenario
        IReadOnlyList<Quote> noQuotes = [];
        IReadOnlyList<Quote> noResults = noQuotes.Aggregate(PeriodSize.Day);
        Assert.IsFalse(noResults.Any());
    }

    [TestMethod]
    public void AggregateTimeSpan()
    {
        IReadOnlyList<Quote> quotes = Data.GetIntraday();

        // aggregate
        IReadOnlyList<Quote> sut = quotes
            .Aggregate(TimeSpan.FromMinutes(15));

        // proper quantities
        sut.Should().HaveCount(108);

        // sample values
        Quote r0 = sut[0];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:30", invariantCulture), r0.Timestamp);
        r0.Open.Should().Be(367.40m);
        r0.High.Should().Be(367.775m);
        r0.Low.Should().Be(367.02m);
        r0.Close.Should().Be(367.24m);
        r0.Volume.Should().Be(2401786m);

        Quote r1 = sut[1];
        Assert.AreEqual(DateTime.Parse("2020-12-15 09:45", invariantCulture), r1.Timestamp);
        r1.Open.Should().Be(367.25m);
        r1.High.Should().Be(367.44m);
        r1.Low.Should().Be(366.69m);
        r1.Close.Should().Be(366.86m);
        r1.Volume.Should().Be(1669983m);

        Quote r2 = sut[2];
        Assert.AreEqual(DateTime.Parse("2020-12-15 10:00", invariantCulture), r2.Timestamp);
        r2.Open.Should().Be(366.85m);
        r2.High.Should().Be(367.17m);
        r2.Low.Should().Be(366.57m);
        r2.Close.Should().Be(366.97m);
        r2.Volume.Should().Be(1396993m);

        // no history scenario
        IReadOnlyList<Quote> noQuotes = [];
        IReadOnlyList<Quote> noResults = noQuotes.Aggregate(TimeSpan.FromDays(1));
        Assert.IsFalse(noResults.Any());
    }

    [TestMethod]
    public void AggregateMonth()
    {
        // aggregate
        IReadOnlyList<Quote> sut = Quotes
            .Aggregate(PeriodSize.Month);

        // proper quantities
        sut.Should().HaveCount(24);

        // sample values
        Quote r0 = sut[0];
        Assert.AreEqual(DateTime.Parse("2017-01-01", invariantCulture), r0.Timestamp);
        r0.Open.Should().Be(212.61m);
        r0.High.Should().Be(217.02m);
        r0.Low.Should().Be(211.52m);
        r0.Close.Should().Be(214.96m);
        r0.Volume.Should().Be(1569087580m);

        Quote r1 = sut[1];
        Assert.AreEqual(DateTime.Parse("2017-02-01", invariantCulture), r1.Timestamp);
        r1.Open.Should().Be(215.65m);
        r1.High.Should().Be(224.20m);
        r1.Low.Should().Be(214.29m);
        r1.Close.Should().Be(223.41m);
        r1.Volume.Should().Be(1444958340m);

        Quote r23 = sut[23];
        Assert.AreEqual(DateTime.Parse("2018-12-01", invariantCulture), r23.Timestamp);
        r23.Open.Should().Be(273.47m);
        r23.High.Should().Be(273.59m);
        r23.Low.Should().Be(229.42m);
        r23.Close.Should().Be(245.28m);
        r23.Volume.Should().Be(3173255968m);
    }

    [TestMethod]  // bad period size
    public void AggregateBadSize()
        => FluentActions
            .Invoking(static () => Quotes.Aggregate(TimeSpan.Zero))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
