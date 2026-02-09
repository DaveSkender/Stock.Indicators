namespace StaticSeries;

[TestClass]
public class Hma : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<HmaResult> sut = Quotes
            .ToHma(20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Hma != null).Should().HaveCount(480);

        // sample values
        HmaResult r1 = sut[149];
        r1.Hma.Should().BeApproximately(236.0835, Money4);

        HmaResult r2 = sut[501];
        r2.Hma.Should().BeApproximately(235.6972, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<HmaResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToHma(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Hma != null).Should().HaveCount(480);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HmaResult> sut = Quotes
            .ToSma(2)
            .ToHma(19);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Hma != null).Should().HaveCount(480);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToHma(20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(471);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<HmaResult> r = BadQuotes
            .ToHma(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Hma is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<HmaResult> r0 = Noquotes
            .ToHma(5);

        r0.Should().BeEmpty();

        IReadOnlyList<HmaResult> r1 = Onequote
            .ToHma(5);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HmaResult> sut = Quotes
            .ToHma(20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(480);

        HmaResult last = sut[^1];
        last.Hma.Should().BeApproximately(235.6972, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToHma(1))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
