using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Kama : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int erPeriods = 10;
        int fastPeriods = 2;
        int slowPeriods = 30;

        List<KamaResult> results = quotes.GetKama(erPeriods, fastPeriods, slowPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Where(x => x.ER != null).Count());
        Assert.AreEqual(493, results.Where(x => x.Kama != null).Count());

        // sample values
        KamaResult r1 = results[8];
        Assert.AreEqual(null, r1.ER);
        Assert.AreEqual(null, r1.Kama);

        KamaResult r2 = results[9];
        Assert.AreEqual(null, r2.ER);
        Assert.AreEqual(213.75m, r2.Kama);

        KamaResult r3 = results[10];
        Assert.AreEqual(0.2465, Math.Round((double)r3.ER, 4));
        Assert.AreEqual(213.7713m, Math.Round((decimal)r3.Kama, 4));

        KamaResult r4 = results[24];
        Assert.AreEqual(0.2136, Math.Round((double)r4.ER, 4));
        Assert.AreEqual(214.7423m, Math.Round((decimal)r4.Kama, 4));

        KamaResult r5 = results[149];
        Assert.AreEqual(0.3165, Math.Round((double)r5.ER, 4));
        Assert.AreEqual(235.5510m, Math.Round((decimal)r5.Kama, 4));

        KamaResult r6 = results[249];
        Assert.AreEqual(0.3182, Math.Round((double)r6.ER, 4));
        Assert.AreEqual(256.0898m, Math.Round((decimal)r6.Kama, 4));

        KamaResult r7 = results[501];
        Assert.AreEqual(0.2214, Math.Round((double)r7.ER, 4));
        Assert.AreEqual(240.1138m, Math.Round((decimal)r7.Kama, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<KamaResult> r = Indicator.GetKama(badQuotes);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<KamaResult> r0 = noquotes.GetKama();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<KamaResult> r1 = onequote.GetKama();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int erPeriods = 10;
        int fastPeriods = 2;
        int slowPeriods = 30;

        List<KamaResult> results = quotes.GetKama(erPeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - Math.Max(erPeriods + 100, erPeriods * 10), results.Count);

        KamaResult last = results.LastOrDefault();
        Assert.AreEqual(0.2214, Math.Round((double)last.ER, 4));
        Assert.AreEqual(240.1138m, Math.Round((decimal)last.Kama, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad ER period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKama(quotes, 0, 2, 30));

        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKama(quotes, 10, 0, 30));

        // bad slow period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKama(quotes, 10, 5, 5));
    }
}
