namespace StaticSeries;

[TestClass]
public class ChaikinOsc : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int fastPeriods = 3;
        const int slowPeriods = 10;

        IReadOnlyList<ChaikinOscResult> results = Quotes
            .ToChaikinOsc(fastPeriods, slowPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Oscillator != null));

        // sample value
        ChaikinOscResult r = results[501];
        Assert.AreEqual(3439986548.42, r.Adl.Round(2));
        Assert.AreEqual(0.8052, r.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-19135200.72, r.Oscillator.Round(2));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToChaikinOsc()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ChaikinOscResult> r = BadQuotes
            .ToChaikinOsc(5, 15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Oscillator is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ChaikinOscResult> r0 = Noquotes
            .ToChaikinOsc();

        Assert.IsEmpty(r0);

        IReadOnlyList<ChaikinOscResult> r1 = Onequote
            .ToChaikinOsc();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        const int fastPeriods = 3;
        const int slowPeriods = 10;

        IReadOnlyList<ChaikinOscResult> results = Quotes
            .ToChaikinOsc(fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (slowPeriods + 100), results);

        ChaikinOscResult last = results[^1];
        Assert.AreEqual(3439986548.42, last.Adl.Round(2));
        Assert.AreEqual(0.8052, last.MoneyFlowMultiplier.Round(4));
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
