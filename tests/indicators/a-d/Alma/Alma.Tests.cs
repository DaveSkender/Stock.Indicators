using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Alma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 10;
        double offset = 0.85;
        double sigma = 6;

        List<AlmaResult> results = quotes.GetAlma(lookbackPeriods, offset, sigma)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Alma != null));

        // sample values
        AlmaResult r1 = results[8];
        Assert.AreEqual(null, r1.Alma);

        AlmaResult r2 = results[9];
        Assert.AreEqual(214.1839, NullMath.Round(r2.Alma, 4));

        AlmaResult r3 = results[24];
        Assert.AreEqual(216.0619, NullMath.Round(r3.Alma, 4));

        AlmaResult r4 = results[149];
        Assert.AreEqual(235.8609, NullMath.Round(r4.Alma, 4));

        AlmaResult r5 = results[249];
        Assert.AreEqual(257.5787, NullMath.Round(r5.Alma, 4));

        AlmaResult r6 = results[501];
        Assert.AreEqual(242.1871, NullMath.Round(r6.Alma, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<AlmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetAlma(10, 0.85, 6);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Alma != null));

        AlmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.1871, NullMath.Round(last.Alma, 4));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<AlmaResult> r = tupleNanny.GetAlma();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Alma is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<AlmaResult> results = quotes
            .GetSma(2)
            .GetAlma(10, 0.85, 6);

        Assert.AreEqual(502, results.Count());
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
        IEnumerable<AlmaResult> r1 = TestData.GetBtcUsdNan()
            .GetAlma(9, 0.85, 6);

        Assert.AreEqual(0, r1.Count(x => x.Alma is double and double.NaN));

        IEnumerable<AlmaResult> r2 = TestData.GetBtcUsdNan()
            .GetAlma(20, 0.85, 6);

        Assert.AreEqual(0, r2.Count(x => x.Alma is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AlmaResult> r = Indicator.GetAlma(badQuotes, 14, 0.5, 3);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Alma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AlmaResult> r0 = noquotes.GetAlma();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AlmaResult> r1 = onequote.GetAlma();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<AlmaResult> results = quotes.GetAlma(10, 0.85, 6)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        AlmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.1871, NullMath.Round(last.Alma, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAlma(quotes, 0, 1, 5));

        // bad offset
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAlma(quotes, 15, 1.1, 3));

        // bad sigma
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAlma(quotes, 10, 0.5, 0));
    }
}
