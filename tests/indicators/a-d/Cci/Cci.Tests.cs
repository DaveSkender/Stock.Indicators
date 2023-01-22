using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class CciTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CciResult> results = quotes
            .GetCci(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Cci != null));

        // sample value
        CciResult r = results[501];
        Assert.AreEqual(-52.9946, r.Cci.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetCci(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<CciResult> r = badQuotes
            .GetCci(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Cci is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<CciResult> r0 = noquotes
            .GetCci()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<CciResult> r1 = onequote
            .GetCci()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<CciResult> results = quotes
            .GetCci(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CciResult last = results.LastOrDefault();
        Assert.AreEqual(-52.9946, last.Cci.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetCci(0));
}
