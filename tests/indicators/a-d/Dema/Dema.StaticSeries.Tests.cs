namespace StaticSeries;

[TestClass]
public class Dema : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<DemaResult> sut = Quotes
            .ToDema(20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Dema != null).Should().HaveCount(483);

        // sample values
        DemaResult r25 = sut[25];
        r25.Dema.Should().BeApproximately(215.7605, Money4);

        DemaResult r51 = sut[51];
        r51.Dema.Should().BeApproximately(225.8259, Money4);

        DemaResult r249 = sut[249];
        r249.Dema.Should().BeApproximately(258.4452, Money4);

        DemaResult r251 = sut[501];
        r251.Dema.Should().BeApproximately(241.1677, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<DemaResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToDema(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Dema != null).Should().HaveCount(483);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DemaResult> sut = Quotes
            .ToSma(2)
            .ToDema(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Dema != null).Should().HaveCount(482);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToDema(20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(474);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<DemaResult> r = BadQuotes
            .ToDema(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Dema is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<DemaResult> r0 = Noquotes
            .ToDema(5);

        r0.Should().BeEmpty();

        IReadOnlyList<DemaResult> r1 = Onequote
            .ToDema(5);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<DemaResult> sut = Quotes
            .ToDema(20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (40 + 100));

        DemaResult last = sut[^1];
        last.Dema.Should().BeApproximately(241.1677, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToDema(0));
}
