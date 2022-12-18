using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Vwma : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VwmaResult> results = quotes.GetVwma(10)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Vwma != null));

        // sample values
        VwmaResult r8 = results[8];
        Assert.IsNull(r8.Vwma);

        Assert.AreEqual(213.981942, NullMath.Round(results[9].Vwma, 6));
        Assert.AreEqual(215.899211, NullMath.Round(results[24].Vwma, 6));
        Assert.AreEqual(226.302760, NullMath.Round(results[99].Vwma, 6));
        Assert.AreEqual(257.053654, NullMath.Round(results[249].Vwma, 6));
        Assert.AreEqual(242.101548, NullMath.Round(results[501].Vwma, 6));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetVwma(10)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<VwmaResult> r = badQuotes.GetVwma(15).ToList();
        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Vwma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<VwmaResult> r0 = noquotes.GetVwma(4).ToList();
        Assert.AreEqual(0, r0.Count);

        List<VwmaResult> r1 = onequote.GetVwma(4).ToList();
        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<VwmaResult> results = quotes.GetVwma(10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        VwmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.101548, NullMath.Round(last.Vwma, 6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetVwma(0));
}
