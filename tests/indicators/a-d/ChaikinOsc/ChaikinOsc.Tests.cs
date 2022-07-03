using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ChaikinOsc : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int fastPeriods = 3;
        int slowPeriods = 10;

        List<ChaikinOscResult> results = quotes.GetChaikinOsc(fastPeriods, slowPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Oscillator != null));

        // sample value
        ChaikinOscResult r = results[501];
        Assert.AreEqual(3439986548.42, NullMath.Round(r.Adl, 2));
        Assert.AreEqual(0.8052, NullMath.Round(r.MoneyFlowMultiplier, 4));
        Assert.AreEqual(118396116.25, NullMath.Round(r.MoneyFlowVolume, 2));
        Assert.AreEqual(-19135200.72, NullMath.Round(r.Oscillator, 2));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetChaikinOsc(3, 10)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ChaikinOscResult> r = Indicator.GetChaikinOsc(badQuotes, 5, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ChaikinOscResult> r0 = noquotes.GetChaikinOsc();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ChaikinOscResult> r1 = onequote.GetChaikinOsc();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 3;
        int slowPeriods = 10;

        List<ChaikinOscResult> results = quotes.GetChaikinOsc(fastPeriods, slowPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + 100), results.Count);

        ChaikinOscResult last = results.LastOrDefault();
        Assert.AreEqual(3439986548.42, NullMath.Round(last.Adl, 2));
        Assert.AreEqual(0.8052, NullMath.Round(last.MoneyFlowMultiplier, 4));
        Assert.AreEqual(118396116.25, NullMath.Round(last.MoneyFlowVolume, 2));
        Assert.AreEqual(-19135200.72, NullMath.Round(last.Oscillator, 2));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast lookback
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetChaikinOsc(quotes, 0));

        // bad slow lookback
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetChaikinOsc(quotes, 10, 5));
    }
}
