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
        Assert.AreEqual(493, results.Where(x => x.Vwma != null).Count());

        // sample values
        VwmaResult r8 = results[8];
        Assert.IsNull(r8.Vwma);

        Assert.AreEqual(213.981942m, Math.Round(results[9].Vwma.Value, 6));
        Assert.AreEqual(215.899211m, Math.Round(results[24].Vwma.Value, 6));
        Assert.AreEqual(226.302760m, Math.Round(results[99].Vwma.Value, 6));
        Assert.AreEqual(257.053654m, Math.Round(results[249].Vwma.Value, 6));
        Assert.AreEqual(242.101548m, Math.Round(results[501].Vwma.Value, 6));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<VwmaResult> r = badQuotes.GetVwma(15);
        Assert.AreEqual(502, r.Count());
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
        Assert.AreEqual(242.101548m, Math.Round(last.Vwma.Value, 6));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetVwma(0));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetVwma(TestData.GetDefault(9), 10));
    }
}
