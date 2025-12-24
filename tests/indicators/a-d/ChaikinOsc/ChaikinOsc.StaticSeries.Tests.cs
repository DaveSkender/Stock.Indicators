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
        Assert.AreEqual(3439986548.42, r.Adl.Round(2));
        r.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        Assert.AreEqual(118396116.25, r.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-19135200.72, r.Oscillator.Round(2));
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
        Assert.IsEmpty(r.Where(static x => x.Oscillator is double v && double.IsNaN(v)));
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
        Assert.AreEqual(3439986548.42, last.Adl.Round(2));
        last.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        Assert.AreEqual(118396116.25, last.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-19135200.72, last.Oscillator.Round(2));
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
