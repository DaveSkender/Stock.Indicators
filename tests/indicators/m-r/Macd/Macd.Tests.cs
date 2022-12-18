using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Macd : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<MacdResult> results =
            quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Macd != null));
        Assert.AreEqual(469, results.Count(x => x.Signal != null));
        Assert.AreEqual(469, results.Count(x => x.Histogram != null));

        // sample values
        MacdResult r49 = results[49];
        Assert.AreEqual(1.7203, NullMath.Round(r49.Macd, 4));
        Assert.AreEqual(1.9675, NullMath.Round(r49.Signal, 4));
        Assert.AreEqual(-0.2472, NullMath.Round(r49.Histogram, 4));
        Assert.AreEqual(224.1840, NullMath.Round(r49.FastEma, 4));
        Assert.AreEqual(222.4637, NullMath.Round(r49.SlowEma, 4));

        MacdResult r249 = results[249];
        Assert.AreEqual(2.2353, NullMath.Round(r249.Macd, 4));
        Assert.AreEqual(2.3141, NullMath.Round(r249.Signal, 4));
        Assert.AreEqual(-0.0789, NullMath.Round(r249.Histogram, 4));
        Assert.AreEqual(256.6780, NullMath.Round(r249.FastEma, 4));
        Assert.AreEqual(254.4428, NullMath.Round(r249.SlowEma, 4));

        MacdResult r501 = results[501];
        Assert.AreEqual(-6.2198, NullMath.Round(r501.Macd, 4));
        Assert.AreEqual(-5.8569, NullMath.Round(r501.Signal, 4));
        Assert.AreEqual(-0.3629, NullMath.Round(r501.Histogram, 4));
        Assert.AreEqual(245.4957, NullMath.Round(r501.FastEma, 4));
        Assert.AreEqual(251.7155, NullMath.Round(r501.SlowEma, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<MacdResult> results = quotes
            .Use(CandlePart.Close)
            .GetMacd()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Count(x => x.Macd != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<MacdResult> r = tupleNanny
            .GetMacd()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Macd is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<MacdResult> results = quotes
            .GetSma(2)
            .GetMacd()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(476, results.Count(x => x.Macd != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetMacd()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<MacdResult> r = badQuotes
            .GetMacd(10, 20, 5)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Macd is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<MacdResult> r0 = noquotes
            .GetMacd()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<MacdResult> r1 = onequote
            .GetMacd()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<MacdResult> results = quotes
            .GetMacd(fastPeriods, slowPeriods, signalPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (slowPeriods + signalPeriods + 250), results.Count);

        MacdResult last = results.LastOrDefault();
        Assert.AreEqual(-6.2198, NullMath.Round(last.Macd, 4));
        Assert.AreEqual(-5.8569, NullMath.Round(last.Signal, 4));
        Assert.AreEqual(-0.3629, NullMath.Round(last.Histogram, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMacd(0, 26, 9));

        // bad slow periods must be larger than faster period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMacd(12, 12, 9));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMacd(12, 26, -1));
    }
}
