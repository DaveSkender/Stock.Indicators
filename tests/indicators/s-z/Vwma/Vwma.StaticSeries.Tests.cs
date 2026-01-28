namespace StaticSeries;

[TestClass]
public class Vwma : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<VwmaResult> sut = Quotes
            .ToVwma(10);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Vwma != null).Should().HaveCount(493);

        // sample values
        VwmaResult r8 = sut[8];
        r8.Vwma.Should().BeNull();

        sut[9].Vwma.Should().BeApproximately(213.981942, Money6);
        sut[24].Vwma.Should().BeApproximately(215.899211, Money6);
        sut[99].Vwma.Should().BeApproximately(226.302760, Money6);
        sut[249].Vwma.Should().BeApproximately(257.053654, Money6);
        sut[501].Vwma.Should().BeApproximately(242.101548, Money6);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToVwma(10)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(484);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<VwmaResult> r = BadQuotes
            .ToVwma(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Vwma is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<VwmaResult> r0 = Noquotes
            .ToVwma(4);

        r0.Should().BeEmpty();

        IReadOnlyList<VwmaResult> r1 = Onequote
            .ToVwma(4);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<VwmaResult> sut = Quotes
            .ToVwma(10)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 9);

        VwmaResult last = sut[^1];
        last.Vwma.Should().BeApproximately(242.101548, Money6);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToVwma(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
