using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Vortex : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VortexResult> results = quotes.GetVortex(14).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Pvi != null));

        // sample values
        VortexResult r1 = results[13];
        Assert.IsNull(r1.Pvi);
        Assert.IsNull(r1.Nvi);

        VortexResult r2 = results[14];
        Assert.AreEqual(1.0460, NullMath.Round(r2.Pvi, 4));
        Assert.AreEqual(0.8119, NullMath.Round(r2.Nvi, 4));

        VortexResult r3 = results[29];
        Assert.AreEqual(1.1300, NullMath.Round(r3.Pvi, 4));
        Assert.AreEqual(0.7393, NullMath.Round(r3.Nvi, 4));

        VortexResult r4 = results[249];
        Assert.AreEqual(1.1558, NullMath.Round(r4.Pvi, 4));
        Assert.AreEqual(0.6634, NullMath.Round(r4.Nvi, 4));

        VortexResult r5 = results[501];
        Assert.AreEqual(0.8712, NullMath.Round(r5.Pvi, 4));
        Assert.AreEqual(1.1163, NullMath.Round(r5.Nvi, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<VortexResult> r = Indicator.GetVortex(badQuotes, 20);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Pvi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<VortexResult> r0 = noquotes.GetVortex(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<VortexResult> r1 = onequote.GetVortex(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        List<VortexResult> results = quotes.GetVortex(14)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        VortexResult last = results.LastOrDefault();
        Assert.AreEqual(0.8712, NullMath.Round(last.Pvi, 4));
        Assert.AreEqual(1.1163, NullMath.Round(last.Nvi, 4));
    }

    [TestMethod]
    public void Removed()
    {
        List<VortexResult> results = quotes.GetVortex(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        VortexResult last = results.LastOrDefault();
        Assert.AreEqual(0.8712, NullMath.Round(last.Pvi, 4));
        Assert.AreEqual(1.1163, NullMath.Round(last.Nvi, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetVortex(quotes, 1));
}
