using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Cmf : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CmfResult> results = quotes.GetCmf(20).ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Cmf != null));

        // sample values
        CmfResult r1 = results[49];
        Assert.AreEqual(0.5468, NullMath.Round(r1.MoneyFlowMultiplier, 4));
        Assert.AreEqual(55609259, NullMath.Round(r1.MoneyFlowVolume, 2));
        Assert.AreEqual(0.350596, NullMath.Round(r1.Cmf, 6));

        CmfResult r2 = results[249];
        Assert.AreEqual(0.7778, NullMath.Round(r2.MoneyFlowMultiplier, 4));
        Assert.AreEqual(36433792.89, NullMath.Round(r2.MoneyFlowVolume, 2));
        Assert.AreEqual(-0.040226, NullMath.Round(r2.Cmf, 6));

        CmfResult r3 = results[501];
        Assert.AreEqual(0.8052, NullMath.Round(r3.MoneyFlowMultiplier, 4));
        Assert.AreEqual(118396116.25, NullMath.Round(r3.MoneyFlowVolume, 2));
        Assert.AreEqual(-0.123754, NullMath.Round(r3.Cmf, 6));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetCmf(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<CmfResult> r = Indicator.GetCmf(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Cmf is double and double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<CmfResult> r = Indicator.GetCmf(bigQuotes, 150);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<CmfResult> r0 = noquotes.GetCmf();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<CmfResult> r1 = onequote.GetCmf();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<CmfResult> results = quotes.GetCmf(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CmfResult last = results.LastOrDefault();
        Assert.AreEqual(0.8052, NullMath.Round(last.MoneyFlowMultiplier, 4));
        Assert.AreEqual(118396116.25, NullMath.Round(last.MoneyFlowVolume, 2));
        Assert.AreEqual(-0.123754, NullMath.Round(last.Cmf, 6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetCmf(quotes, 0));
}
