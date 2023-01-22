using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class KamaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int erPeriods = 10;
        int fastPeriods = 2;
        int slowPeriods = 30;

        List<KamaResult> results = quotes
            .GetKama(erPeriods, fastPeriods, slowPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.ER != null));
        Assert.AreEqual(493, results.Count(x => x.Kama != null));

        // sample values
        KamaResult r1 = results[8];
        Assert.AreEqual(null, r1.ER);
        Assert.AreEqual(null, r1.Kama);

        KamaResult r2 = results[9];
        Assert.AreEqual(null, r2.ER);
        Assert.AreEqual(213.7500, r2.Kama.Round(4));

        KamaResult r3 = results[10];
        Assert.AreEqual(0.2465, r3.ER.Round(4));
        Assert.AreEqual(213.7713, r3.Kama.Round(4));

        KamaResult r4 = results[24];
        Assert.AreEqual(0.2136, r4.ER.Round(4));
        Assert.AreEqual(214.7423, r4.Kama.Round(4));

        KamaResult r5 = results[149];
        Assert.AreEqual(0.3165, r5.ER.Round(4));
        Assert.AreEqual(235.5510, r5.Kama.Round(4));

        KamaResult r6 = results[249];
        Assert.AreEqual(0.3182, r6.ER.Round(4));
        Assert.AreEqual(256.0898, r6.Kama.Round(4));

        KamaResult r7 = results[501];
        Assert.AreEqual(0.2214, r7.ER.Round(4));
        Assert.AreEqual(240.1138, r7.Kama.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<KamaResult> results = quotes
            .Use(CandlePart.Close)
            .GetKama()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Kama != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<KamaResult> r = tupleNanny
            .GetKama()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Kama is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<KamaResult> results = quotes
            .GetSma(2)
            .GetKama()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Kama != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetKama()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<KamaResult> r = badQuotes
            .GetKama()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Kama is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<KamaResult> r0 = noquotes
            .GetKama()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<KamaResult> r1 = onequote
            .GetKama()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int erPeriods = 10;
        int fastPeriods = 2;
        int slowPeriods = 30;

        List<KamaResult> results = quotes
            .GetKama(erPeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - Math.Max(erPeriods + 100, erPeriods * 10), results.Count);

        KamaResult last = results.LastOrDefault();
        Assert.AreEqual(0.2214, last.ER.Round(4));
        Assert.AreEqual(240.1138, last.Kama.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad ER period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetKama(0, 2, 30));

        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetKama(10, 0, 30));

        // bad slow period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetKama(10, 5, 5));
    }
}
