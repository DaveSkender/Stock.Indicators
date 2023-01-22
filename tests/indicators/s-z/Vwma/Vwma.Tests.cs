using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class VwmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VwmaResult> results = quotes
            .GetVwma(10)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Vwma != null));

        // sample values
        VwmaResult r8 = results[8];
        Assert.IsNull(r8.Vwma);

        Assert.AreEqual(213.981942, results[9].Vwma.Round(6));
        Assert.AreEqual(215.899211, results[24].Vwma.Round(6));
        Assert.AreEqual(226.302760, results[99].Vwma.Round(6));
        Assert.AreEqual(257.053654, results[249].Vwma.Round(6));
        Assert.AreEqual(242.101548, results[501].Vwma.Round(6));
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
        List<VwmaResult> r = badQuotes
            .GetVwma(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Vwma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<VwmaResult> r0 = noquotes
            .GetVwma(4)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<VwmaResult> r1 = onequote
            .GetVwma(4)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<VwmaResult> results = quotes
            .GetVwma(10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        VwmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.101548, last.Vwma.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetVwma(0));
}
