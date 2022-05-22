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
        Assert.AreEqual(493, results.Where(x => x.Alma != null).Count());

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
    public void Chaining()
    {
        int lookbackPeriods = 10;
        double offset = 0.85;
        double sigma = 6;

        List<AlmaResult> standard = quotes
            .GetAlma(lookbackPeriods, offset, sigma)
            .ToList();

        List<AlmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetAlma(lookbackPeriods, offset, sigma)
            .ToList();

        // assertions
        for (int i = 0; i < results.Count; i++)
        {
            AlmaResult s = standard[i];
            AlmaResult c = results[i];

            Assert.AreEqual(s.Date, c.Date);
            Assert.AreEqual(s.Alma, c.Alma);
        }
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AlmaResult> r = Indicator.GetAlma(badQuotes, 14, 0.5, 3);
        Assert.AreEqual(502, r.Count());
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
