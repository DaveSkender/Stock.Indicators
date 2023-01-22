using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class Alma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 10;
        double offset = 0.85;
        double sigma = 6;

        List<AlmaResult> results = quotes
            .GetAlma(lookbackPeriods, offset, sigma)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Alma != null));

        // sample values
        AlmaResult r1 = results[8];
        Assert.AreEqual(null, r1.Alma);

        AlmaResult r2 = results[9];
        Assert.AreEqual(214.1839, r2.Alma.Round(4));

        AlmaResult r3 = results[24];
        Assert.AreEqual(216.0619, r3.Alma.Round(4));

        AlmaResult r4 = results[149];
        Assert.AreEqual(235.8609, r4.Alma.Round(4));

        AlmaResult r5 = results[249];
        Assert.AreEqual(257.5787, r5.Alma.Round(4));

        AlmaResult r6 = results[501];
        Assert.AreEqual(242.1871, r6.Alma.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<AlmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetAlma(10, 0.85, 6)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Alma != null));

        AlmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.1871, last.Alma.Round(4));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<AlmaResult> r = tupleNanny
            .GetAlma()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Alma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<AlmaResult> results = quotes
            .GetSma(2)
            .GetAlma(10, 0.85, 6)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Alma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        int lookbackPeriods = 10;
        double offset = 0.85;
        double sigma = 6;

        List<SmaResult> results = quotes
            .GetAlma(lookbackPeriods, offset, sigma)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        List<AlmaResult> r1 = TestData.GetBtcUsdNan().GetAlma(9, 0.85, 6).ToList();

        Assert.AreEqual(0, r1.Count(x => x.Alma is double and double.NaN));

        List<AlmaResult> r2 = TestData.GetBtcUsdNan().GetAlma(20, 0.85, 6).ToList();

        Assert.AreEqual(0, r2.Count(x => x.Alma is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        List<AlmaResult> r = badQuotes
            .GetAlma(14, 0.5, 3)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Alma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AlmaResult> r0 = noquotes
            .GetAlma()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<AlmaResult> r1 = onequote
            .GetAlma()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<AlmaResult> results = quotes
            .GetAlma(10, 0.85, 6)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        AlmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.1871, last.Alma.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlma(0, 1, 5));

        // bad offset
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlma(15, 1.1, 3));

        // bad sigma
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlma(10, 0.5, 0));
    }
}
