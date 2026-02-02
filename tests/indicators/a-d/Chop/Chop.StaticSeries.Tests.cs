namespace StaticSeries;

[TestClass]
public class Chop : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<ChopResult> sut = Quotes
            .ToChop();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Chop != null).Should().HaveCount(488);

        // sample values
        ChopResult r1 = sut[13];
        r1.Chop.Should().BeNull();

        ChopResult r2 = sut[14];
        r2.Chop.Should().BeApproximately(69.9967, Money4);

        ChopResult r3 = sut[249];
        r3.Chop.Should().BeApproximately(41.8499, Money4);

        ChopResult r4 = sut[501];
        r4.Chop.Should().BeApproximately(38.6526, Money4);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToChop()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(479);
    }

    [TestMethod]
    public void SmallLookback()
    {
        const int lookbackPeriods = 2;
        IReadOnlyList<ChopResult> sut = Quotes
            .ToChop(lookbackPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Chop != null).Should().HaveCount(500);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<ChopResult> sut = Quotes.ToChop(14);
        sut.IsBetween(static x => x.Chop, 0, 100);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ChopResult> r = BadQuotes
            .ToChop(20);

        r.Should().HaveCount(502);
        r.Where(static x => x.Chop is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ChopResult> r0 = Noquotes
            .ToChop();

        r0.Should().BeEmpty();

        IReadOnlyList<ChopResult> r1 = Onequote
            .ToChop();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ChopResult> sut = Quotes
            .ToChop()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 14);

        ChopResult last = sut[^1];
        last.Chop.Should().BeApproximately(38.6526, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToChop(1))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
