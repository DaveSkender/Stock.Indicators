using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class WilliamsRTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<WilliamsResult> results = quotes
            .GetWilliamsR(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.WilliamsR != null));

        // sample values
        WilliamsResult r1 = results[343];
        Assert.AreEqual(-19.8211, r1.WilliamsR.Round(4));

        WilliamsResult r2 = results[501];
        Assert.AreEqual(-52.0121, r2.WilliamsR.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetWilliamsR()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<WilliamsResult> r = badQuotes
            .GetWilliamsR(20)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.WilliamsR is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<WilliamsResult> r0 = noquotes
            .GetWilliamsR()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<WilliamsResult> r1 = onequote
            .GetWilliamsR()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<WilliamsResult> results = quotes
            .GetWilliamsR(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        WilliamsResult last = results.LastOrDefault();
        Assert.AreEqual(-52.0121, last.WilliamsR.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetWilliamsR(0));
}
