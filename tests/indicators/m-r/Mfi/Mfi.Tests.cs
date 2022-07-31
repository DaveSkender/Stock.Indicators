using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Mfi : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 14;
        List<MfiResult> results = quotes.GetMfi(lookbackPeriods).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[439];
        Assert.AreEqual(69.0622, NullMath.Round(r1.Mfi, 4));

        MfiResult r2 = results[501];
        Assert.AreEqual(39.9494, NullMath.Round(r2.Mfi, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetMfi()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(479, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void SmallLookback()
    {
        int lookbackPeriods = 4;

        List<MfiResult> results = Indicator.GetMfi(quotes, lookbackPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(498, results.Count(x => x.Mfi != null));

        // sample values
        MfiResult r1 = results[31];
        Assert.AreEqual(100, NullMath.Round(r1.Mfi, 4));

        MfiResult r2 = results[43];
        Assert.AreEqual(0, NullMath.Round(r2.Mfi, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<MfiResult> r = Indicator.GetMfi(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Mfi is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<MfiResult> r0 = noquotes.GetMfi();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<MfiResult> r1 = onequote.GetMfi();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 14;
        List<MfiResult> results = quotes.GetMfi(lookbackPeriods)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        MfiResult last = results.LastOrDefault();
        Assert.AreEqual(39.9494, NullMath.Round(last.Mfi, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetMfi(quotes, 1));
}
