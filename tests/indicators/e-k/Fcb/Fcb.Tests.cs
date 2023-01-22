using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class FcbTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<FcbResult> results = quotes
            .GetFcb(2)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(497, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(493, results.Count(x => x.LowerBand != null));

        // sample values
        FcbResult r1 = results[4];
        Assert.AreEqual(null, r1.UpperBand);
        Assert.AreEqual(null, r1.LowerBand);

        FcbResult r2 = results[10];
        Assert.AreEqual(214.84m, r2.UpperBand);
        Assert.AreEqual(212.53m, r2.LowerBand);

        FcbResult r3 = results[120];
        Assert.AreEqual(233.35m, r3.UpperBand);
        Assert.AreEqual(231.14m, r3.LowerBand);

        FcbResult r4 = results[180];
        Assert.AreEqual(236.78m, r4.UpperBand);
        Assert.AreEqual(233.56m, r4.LowerBand);

        FcbResult r5 = results[250];
        Assert.AreEqual(258.70m, r5.UpperBand);
        Assert.AreEqual(257.04m, r5.LowerBand);

        FcbResult r6 = results[501];
        Assert.AreEqual(262.47m, r6.UpperBand);
        Assert.AreEqual(229.42m, r6.LowerBand);
    }

    [TestMethod]
    public void BadData()
    {
        List<FcbResult> r = badQuotes
            .GetFcb()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<FcbResult> r0 = noquotes
            .GetFcb()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<FcbResult> r1 = onequote
            .GetFcb()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<FcbResult> results = quotes
            .GetFcb(2)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 5, results.Count);

        FcbResult last = results.LastOrDefault();
        Assert.AreEqual(262.47m, last.UpperBand);
        Assert.AreEqual(229.42m, last.LowerBand);
    }

    [TestMethod]
    public void Removed()
    {
        List<FcbResult> results = quotes
            .GetFcb(2)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 5, results.Count);

        FcbResult last = results.LastOrDefault();
        Assert.AreEqual(262.47m, last.UpperBand);
        Assert.AreEqual(229.42m, last.LowerBand);
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetFcb(1));
}
