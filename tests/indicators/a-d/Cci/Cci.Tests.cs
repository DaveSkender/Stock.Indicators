using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Cci : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<CciResult> results = quotes.GetCci(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Cci != null));

        // sample value
        CciResult r = results[501];
        Assert.AreEqual(-52.9946, NullMath.Round(r.Cci, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetCci(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<CciResult> r = Indicator.GetCci(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Cci is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<CciResult> r0 = noquotes.GetCci();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<CciResult> r1 = onequote.GetCci();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<CciResult> results = quotes.GetCci(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        CciResult last = results.LastOrDefault();
        Assert.AreEqual(-52.9946, NullMath.Round(last.Cci, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetCci(quotes, 0));
}
