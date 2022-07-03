using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class StdDev : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<StdDevResult> results = quotes.GetStdDev(10).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
        Assert.AreEqual(493, results.Count(x => x.ZScore != null));
        Assert.AreEqual(false, results.Any(x => x.StdDevSma != null));

        // sample values
        StdDevResult r1 = results[8];
        Assert.AreEqual(null, r1.StdDev);
        Assert.AreEqual(null, r1.Mean);
        Assert.AreEqual(null, r1.ZScore);
        Assert.AreEqual(null, r1.StdDevSma);

        StdDevResult r2 = results[9];
        Assert.AreEqual(0.5020, NullMath.Round(r2.StdDev, 4));
        Assert.AreEqual(214.0140, NullMath.Round(r2.Mean, 4));
        Assert.AreEqual(-0.525917, NullMath.Round(r2.ZScore, 6));
        Assert.AreEqual(null, r2.StdDevSma);

        StdDevResult r3 = results[249];
        Assert.AreEqual(0.9827, NullMath.Round(r3.StdDev, 4));
        Assert.AreEqual(257.2200, NullMath.Round(r3.Mean, 4));
        Assert.AreEqual(0.783563, NullMath.Round(r3.ZScore, 6));
        Assert.AreEqual(null, r3.StdDevSma);

        StdDevResult r4 = results[501];
        Assert.AreEqual(5.4738, NullMath.Round(r4.StdDev, 4));
        Assert.AreEqual(242.4100, NullMath.Round(r4.Mean, 4));
        Assert.AreEqual(0.524312, NullMath.Round(r4.ZScore, 6));
        Assert.AreEqual(null, r4.StdDevSma);
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<StdDevResult> results = quotes
            .Use(CandlePart.Close)
            .GetStdDev(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<StdDevResult> r = tupleNanny.GetStdDev(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.StdDev is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<StdDevResult> results = quotes
            .GetSma(2)
            .GetStdDev(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(492, results.Count(x => x.StdDev != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetStdDev(10)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void WithSma()
    {
        int lookbackPeriods = 10;
        int smaPeriods = 5;
        List<StdDevResult> results = Indicator.GetStdDev(quotes, lookbackPeriods, smaPeriods).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.StdDev != null));
        Assert.AreEqual(493, results.Count(x => x.ZScore != null));
        Assert.AreEqual(489, results.Count(x => x.StdDevSma != null));

        // sample values
        StdDevResult r1 = results[19];
        Assert.AreEqual(1.1642, NullMath.Round(r1.StdDev, 4));
        Assert.AreEqual(-0.065282, NullMath.Round(r1.ZScore, 6));
        Assert.AreEqual(1.1422, NullMath.Round(r1.StdDevSma, 4));

        StdDevResult r2 = results[501];
        Assert.AreEqual(5.4738, NullMath.Round(r2.StdDev, 4));
        Assert.AreEqual(0.524312, NullMath.Round(r2.ZScore, 6));
        Assert.AreEqual(7.6886, NullMath.Round(r2.StdDevSma, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<StdDevResult> r = badQuotes.GetStdDev(15, 3);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.StdDev is double and double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<StdDevResult> r = bigQuotes.GetStdDev(200, 3);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<StdDevResult> r0 = noquotes.GetStdDev(10);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<StdDevResult> r1 = onequote.GetStdDev(10);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<StdDevResult> results = quotes.GetStdDev(10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        StdDevResult last = results.LastOrDefault();
        Assert.AreEqual(5.4738, NullMath.Round(last.StdDev, 4));
        Assert.AreEqual(242.4100, NullMath.Round(last.Mean, 4));
        Assert.AreEqual(0.524312, NullMath.Round(last.ZScore, 6));
        Assert.AreEqual(null, last.StdDevSma);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStdDev(quotes, 1));

        // bad SMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetStdDev(quotes, 14, 0));
    }
}
