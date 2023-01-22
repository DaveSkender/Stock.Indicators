using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class BopTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<BopResult> results = quotes
            .GetBop(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.Bop != null));

        // sample values
        BopResult r1 = results[12];
        Assert.AreEqual(null, r1.Bop);

        BopResult r2 = results[13];
        Assert.AreEqual(0.081822, r2.Bop.Round(6));

        BopResult r3 = results[149];
        Assert.AreEqual(-0.016203, r3.Bop.Round(6));

        BopResult r4 = results[249];
        Assert.AreEqual(-0.058682, r4.Bop.Round(6));

        BopResult r5 = results[501];
        Assert.AreEqual(-0.292788, r5.Bop.Round(6));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetBop(14)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IEnumerable<BopResult> r = TestData.GetBtcUsdNan()
            .GetBop(50);

        Assert.AreEqual(0, r.Count(x => x.Bop is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        List<BopResult> r = badQuotes
            .GetBop()
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Bop is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<BopResult> r0 = noquotes
            .GetBop()
            .ToList();
        Assert.AreEqual(0, r0.Count);

        List<BopResult> r1 = onequote
            .GetBop()
            .ToList();
        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<BopResult> results = quotes
            .GetBop(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        BopResult last = results.LastOrDefault();
        Assert.AreEqual(-0.292788, last.Bop.Round(6));
    }

    // bad smoothing period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetBop(0));
}
