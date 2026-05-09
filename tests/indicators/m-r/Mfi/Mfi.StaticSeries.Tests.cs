namespace StaticSeries;

[TestClass]
public class Mfi : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<MfiResult> sut = Quotes
            .ToMfi();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Mfi != null).Should().HaveCount(488);

        // sample values
        MfiResult r1 = sut[439];
        r1.Mfi.Should().BeApproximately(69.0622, Money4);

        MfiResult r2 = sut[501];
        r2.Mfi.Should().BeApproximately(39.9494, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<MfiResult> sut = Quotes.ToMfi(14);
        sut.IsBetween(static x => x.Mfi, 0, 100);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToMfi()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(479);
    }

    [TestMethod]
    public void SmallLookback()
    {
        const int lookbackPeriods = 4;

        IReadOnlyList<MfiResult> sut = Quotes
            .ToMfi(lookbackPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Mfi != null).Should().HaveCount(498);

        // sample values
        MfiResult r1 = sut[31];
        r1.Mfi.Should().BeApproximately(100, Money4);

        MfiResult r2 = sut[43];
        r2.Mfi.Should().BeApproximately(0, Money4);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<MfiResult> r = BadQuotes
            .ToMfi(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Mfi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<MfiResult> r0 = Noquotes
            .ToMfi();

        r0.Should().BeEmpty();

        IReadOnlyList<MfiResult> r1 = Onequote
            .ToMfi();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const int lookbackPeriods = 14;

        IReadOnlyList<MfiResult> sut = Quotes
            .ToMfi(lookbackPeriods)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 14);

        MfiResult last = sut[^1];
        last.Mfi.Should().BeApproximately(39.9494, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToMfi(1))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
