using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Adl : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AdlResult> results = quotes.GetAdl().ToList();

        // assertions

        // should always be the same number of results as there is quotes
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
        List<AdlResult> results = Indicator.GetAdl(quotes, 20).ToList();

        // assertions

        // should always be the same number of results as there is quotes
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
        IEnumerable<SmaResult> results = quotes
            .GetAdl()
            .GetSma(10);

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AdlResult> r = badQuotes.GetAdl();
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => double.IsNaN(x.Adl)));
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<AdlResult> r = bigQuotes.GetAdl();
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void RandomData()
    {
        IEnumerable<AdlResult> r = randomQuotes.GetAdl();
        Assert.AreEqual(1000, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AdlResult> r0 = noquotes.GetAdl();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AdlResult> r1 = onequote.GetAdl();
        Assert.AreEqual(1, r1.Count());
    }

    // bad SMA period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetAdl(quotes, 0));
}
