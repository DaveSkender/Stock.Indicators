using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class VortexTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VortexResult> results = quotes
            .GetVortex(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Pvi != null));

        // sample values
        VortexResult r1 = results[13];
        Assert.IsNull(r1.Pvi);
        Assert.IsNull(r1.Nvi);

        VortexResult r2 = results[14];
        Assert.AreEqual(1.0460, r2.Pvi.Round(4));
        Assert.AreEqual(0.8119, r2.Nvi.Round(4));

        VortexResult r3 = results[29];
        Assert.AreEqual(1.1300, r3.Pvi.Round(4));
        Assert.AreEqual(0.7393, r3.Nvi.Round(4));

        VortexResult r4 = results[249];
        Assert.AreEqual(1.1558, r4.Pvi.Round(4));
        Assert.AreEqual(0.6634, r4.Nvi.Round(4));

        VortexResult r5 = results[501];
        Assert.AreEqual(0.8712, r5.Pvi.Round(4));
        Assert.AreEqual(1.1163, r5.Nvi.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<VortexResult> r = badQuotes
            .GetVortex(20)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Pvi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<VortexResult> r0 = noquotes
            .GetVortex(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<VortexResult> r1 = onequote
            .GetVortex(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<VortexResult> results = quotes
            .GetVortex(14)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        VortexResult last = results.LastOrDefault();
        Assert.AreEqual(0.8712, last.Pvi.Round(4));
        Assert.AreEqual(1.1163, last.Nvi.Round(4));
    }

    [TestMethod]
    public void Removed()
    {
        List<VortexResult> results = quotes
            .GetVortex(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        VortexResult last = results.LastOrDefault();
        Assert.AreEqual(0.8712, last.Pvi.Round(4));
        Assert.AreEqual(1.1163, last.Nvi.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetVortex(1));
}
