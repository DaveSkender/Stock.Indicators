using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class MamaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        double fastLimit = 0.5;
        double slowLimit = 0.05;

        List<MamaResult> results = quotes
            .GetMama(fastLimit, slowLimit)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(497, results.Count(x => x.Mama != null));

        // sample values
        MamaResult r1 = results[4];
        Assert.AreEqual(null, r1.Mama);
        Assert.AreEqual(null, r1.Fama);

        MamaResult r2 = results[5];
        Assert.AreEqual(213.73, r2.Mama);
        Assert.AreEqual(213.73, r2.Fama);

        MamaResult r3 = results[6];
        Assert.AreEqual(213.7850, r3.Mama.Round(4));
        Assert.AreEqual(213.7438, r3.Fama.Round(4));

        MamaResult r4 = results[25];
        Assert.AreEqual(215.9524, r4.Mama.Round(4));
        Assert.AreEqual(215.1407, r4.Fama.Round(4));

        MamaResult r5 = results[149];
        Assert.AreEqual(235.6593, r5.Mama.Round(4));
        Assert.AreEqual(234.3660, r5.Fama.Round(4));

        MamaResult r6 = results[249];
        Assert.AreEqual(256.8026, r6.Mama.Round(4));
        Assert.AreEqual(254.0605, r6.Fama.Round(4));

        MamaResult r7 = results[501];
        Assert.AreEqual(244.1092, r7.Mama.Round(4));
        Assert.AreEqual(252.6139, r7.Fama.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<MamaResult> results = quotes
            .Use(CandlePart.Close)
            .GetMama()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(497, results.Count(x => x.Mama != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<MamaResult> r = tupleNanny
            .GetMama()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mama is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<MamaResult> results = quotes
            .GetSma(2)
            .GetMama()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(496, results.Count(x => x.Mama != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetMama()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<MamaResult> r = badQuotes
            .GetMama()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mama is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<MamaResult> r0 = noquotes
            .GetMama()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<MamaResult> r1 = onequote
            .GetMama()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        double fastLimit = 0.5;
        double slowLimit = 0.05;

        List<MamaResult> results = quotes
            .GetMama(fastLimit, slowLimit)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 50, results.Count);

        MamaResult last = results.LastOrDefault();
        Assert.AreEqual(244.1092, last.Mama.Round(4));
        Assert.AreEqual(252.6139, last.Fama.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period (same as slow period)
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMama(0.5, 0.5));

        // bad fast period (cannot be 1 or more)
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMama(1, 0.5));

        // bad slow period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMama(0.5, 0));
    }
}
