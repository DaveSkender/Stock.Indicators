using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Adl : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AdlResult> results = quotes
            .GetAdl()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502, results.Count(x => x.AdlSma == null));

        // sample values
        AdlResult r1 = results[249];
        Assert.AreEqual(0.7778, NullMath.Round(r1.MoneyFlowMultiplier, 4));
        Assert.AreEqual(36433792.89, NullMath.Round(r1.MoneyFlowVolume, 2));
        Assert.AreEqual(3266400865.74, NullMath.Round(r1.Adl, 2));
        Assert.AreEqual(null, r1.AdlSma);

        AdlResult r2 = results[501];
        Assert.AreEqual(0.8052, NullMath.Round(r2.MoneyFlowMultiplier, 4));
        Assert.AreEqual(118396116.25, NullMath.Round(r2.MoneyFlowVolume, 2));
        Assert.AreEqual(3439986548.42, NullMath.Round(r2.Adl, 2));
        Assert.AreEqual(null, r2.AdlSma);
    }

    [TestMethod]
    public void WithSma()
    {
        List<AdlResult> results = quotes
            .GetAdl(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.AdlSma != null));

        // sample value
        AdlResult r = results[501];
        Assert.AreEqual(0.8052, NullMath.Round(r.MoneyFlowMultiplier, 4));
        Assert.AreEqual(118396116.25, NullMath.Round(r.MoneyFlowVolume, 2));
        Assert.AreEqual(3439986548.42, NullMath.Round(r.Adl, 2));
        Assert.AreEqual(3595352721.16, NullMath.Round(r.AdlSma, 2));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetAdl()
            .GetSma(10)
            .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<AdlResult> r = badQuotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Adl)));
    }

    [TestMethod]
    public void BigData()
    {
        List<AdlResult> r = bigQuotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public void RandomData()
    {
        List<AdlResult> r = randomQuotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(1000, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AdlResult> r0 = noquotes
            .GetAdl()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<AdlResult> r1 = onequote
            .GetAdl()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetAdl(0));
}
