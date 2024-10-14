namespace StaticSeries;

[TestClass]
public class ChaikinOsc : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int fastPeriods = 3;
        int slowPeriods = 10;

        IReadOnlyList<ChaikinOscResult> results = Quotes
            .GetChaikinOsc(fastPeriods, slowPeriods);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Oscillator != null));

        // sample value
        ChaikinOscResult r = results[501];
        Assert.AreEqual(3439986548.42, r.Adl.Round(2));
        Assert.AreEqual(0.8052, r.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-19135200.72, r.Oscillator.Round(2));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetChaikinOsc()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ChaikinOscResult> r = BadQuotes
            .GetChaikinOsc(5, 15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ChaikinOscResult> r0 = Noquotes
            .GetChaikinOsc();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ChaikinOscResult> r1 = Onequote
            .GetChaikinOsc();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 3;
        int slowPeriods = 10;

        IReadOnlyList<ChaikinOscResult> results = Quotes
            .GetChaikinOsc(fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + 100), results.Count);

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
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetChaikinOsc(0));

        // bad slow lookback
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetChaikinOsc(10, 5));
    }
}
