using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class MfiTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<MfiResult> results = quotes
            .GetMfi(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[439];
        Assert.AreEqual(69.0622, r1.Mfi.Round(4));

        MfiResult r2 = results[501];
        Assert.AreEqual(39.9494, r2.Mfi.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetMfi()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 4;

        List<MfiResult> results = quotes
            .GetMfi(lookbackPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(498, results.Count(x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[31];
        Assert.AreEqual(100, r1.Mfi.Round(4));

        MfiResult r2 = results[43];
        Assert.AreEqual(0, r2.Mfi.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<MfiResult> r = badQuotes
            .GetMfi(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Mfi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<MfiResult> r0 = noquotes
            .GetMfi()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<MfiResult> r1 = onequote
            .GetMfi()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;

        List<MfiResult> results = quotes
            .GetMfi(lookbackPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        MfiResult last = results.LastOrDefault();
        Assert.AreEqual(39.9494, last.Mfi.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetMfi(1));
}
