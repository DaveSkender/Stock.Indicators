namespace StaticSeries;

[TestClass]
public class ChaikinOsc : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int fastPeriods = 3;
        const int slowPeriods = 10;

        IReadOnlyList<ChaikinOscResult> sut = Quotes
            .ToChaikinOsc(fastPeriods, slowPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Oscillator != null).Should().HaveCount(493);

        // sample value
        ChaikinOscResult r = sut[501];
        r.Adl.Should().BeApproximately(3439986548.42, 0.005);
        r.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        r.MoneyFlowVolume.Should().BeApproximately(118396116.25, 0.005);
        r.Oscillator.Should().BeApproximately(-19135200.72, 0.005);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToChaikinOsc()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(484);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ChaikinOscResult> r = BadQuotes
            .ToChaikinOsc(5, 15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Oscillator is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ChaikinOscResult> r0 = Noquotes
            .ToChaikinOsc();

        r0.Should().BeEmpty();

        IReadOnlyList<ChaikinOscResult> r1 = Onequote
            .ToChaikinOsc();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const int fastPeriods = 3;
        const int slowPeriods = 10;

        IReadOnlyList<ChaikinOscResult> sut = Quotes
            .ToChaikinOsc(fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (slowPeriods + 100));

        ChaikinOscResult last = sut[^1];
        last.Adl.Should().BeApproximately(3439986548.42, 0.005);
        last.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        last.MoneyFlowVolume.Should().BeApproximately(118396116.25, 0.005);
        last.Oscillator.Should().BeApproximately(-19135200.72, 0.005);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast lookback
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToChaikinOsc(0));

        // bad slow lookback
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToChaikinOsc(10, 5));
    }
}
