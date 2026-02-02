namespace StaticSeries;

[TestClass]
public class WilliamsR : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<WilliamsResult> sut = Quotes
            .ToWilliamsR();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.WilliamsR != null).Should().HaveCount(489);

        // sample values
        WilliamsResult r1 = sut[343];
        r1.WilliamsR.Should().BeApproximately(-19.8211, Money4);

        WilliamsResult r2 = sut[501];
        r2.WilliamsR.Should().BeApproximately(-52.0121, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<WilliamsResult> sut = Quotes.ToWilliamsR(14);
        sut.IsBetween(static x => x.WilliamsR, -100, 0);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToWilliamsR()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(480);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<WilliamsResult> sut = BadQuotes
            .ToWilliamsR(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.WilliamsR is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<WilliamsResult> r0 = Noquotes
            .ToWilliamsR();

        r0.Should().BeEmpty();

        IReadOnlyList<WilliamsResult> r1 = Onequote
            .ToWilliamsR();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<WilliamsResult> sut = Quotes
            .ToWilliamsR()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 13);

        WilliamsResult last = sut[^1];
        last.WilliamsR.Should().BeApproximately(-52.0121, Money4);
    }

    [TestMethod]
    public void Boundary()
    {
        IReadOnlyList<WilliamsResult> sut = Data
            .GetRandom(2500)
            .ToWilliamsR();

        sut.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    [TestMethod]
    public void Issue1127_Original_BoundaryThreshold_Maintained()
    {
        // initialize
        IReadOnlyList<Quote> quotes = Data.QuotesFromCsv("_issue1127.williamr.original.csv");

        // get indicators
        IReadOnlyList<WilliamsResult> sut = quotes
            .ToWilliamsR();

        sut.Should().HaveCountGreaterThan(0);
        sut.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    [TestMethod]
    public void Issue1127_Revisit_BoundaryThreshold_Maintained()
    {
        // initialize
        IReadOnlyList<Quote> quotes = Data.QuotesFromCsv("_issue1127.williamr.revisit.csv");

        // get indicators
        IReadOnlyList<WilliamsResult> sut = quotes
            .ToWilliamsR();

        sut.ToConsole(args: (nameof(WilliamsResult.WilliamsR), "F20"));

        sut.Should().HaveCountGreaterThan(0);
        sut.IsBetween(static x => x.WilliamsR, -100d, 0d);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToWilliamsR(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
