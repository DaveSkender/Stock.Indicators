using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Ultimate : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<UltimateResult> results = quotes.GetUltimate(7, 14, 28)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Ultimate != null));

        // sample values
        UltimateResult r1 = results[74];
        Assert.AreEqual(51.7770, NullMath.Round(r1.Ultimate, 4));

        UltimateResult r2 = results[249];
        Assert.AreEqual(45.3121, NullMath.Round(r2.Ultimate, 4));

        UltimateResult r3 = results[501];
        Assert.AreEqual(49.5257, NullMath.Round(r3.Ultimate, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetUltimate()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(465, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<UltimateResult> r = Indicator.GetUltimate(badQuotes, 1, 2, 3);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Ultimate is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<UltimateResult> r0 = noquotes.GetUltimate();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<UltimateResult> r1 = onequote.GetUltimate();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<UltimateResult> results = quotes.GetUltimate(7, 14, 28)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 28, results.Count);

        UltimateResult last = results.LastOrDefault();
        Assert.AreEqual(49.5257, NullMath.Round(last.Ultimate, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad short period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetUltimate(quotes, 0));

        // bad middle period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetUltimate(quotes, 7, 6));

        // bad long period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetUltimate(quotes, 7, 14, 11));
    }
}
