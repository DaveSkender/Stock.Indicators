namespace StaticSeries;

[TestClass]
public class UlcerIndex : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<UlcerIndexResult> sut = Quotes
            .ToUlcerIndex();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.UlcerIndex != null).Should().HaveCount(489);

        // sample value
        UlcerIndexResult r = sut[501];
        r.UlcerIndex.Should().BeApproximately(5.7255, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<UlcerIndexResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToUlcerIndex();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.UlcerIndex != null).Should().HaveCount(489);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<UlcerIndexResult> sut = Quotes
            .ToSma(2)
            .ToUlcerIndex();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.UlcerIndex != null).Should().HaveCount(488);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToUlcerIndex()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(480);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<UlcerIndexResult> r = BadQuotes
            .ToUlcerIndex(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.UlcerIndex is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<UlcerIndexResult> r0 = Noquotes
            .ToUlcerIndex();

        r0.Should().BeEmpty();

        IReadOnlyList<UlcerIndexResult> r1 = Onequote
            .ToUlcerIndex();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UlcerIndexResult> sut = Quotes
            .ToUlcerIndex()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 13);

        UlcerIndexResult last = sut[^1];
        last.UlcerIndex.Should().BeApproximately(5.7255, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToUlcerIndex(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
