using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Dynamic : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<DynamicResult> results = quotes.GetDynamic(14).ToList();

        // assertions
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Dynamic != null));

        // sample values
        DynamicResult r1 = results[1];
        Assert.AreEqual(212.9465, NullMath.Round(r1.Dynamic, 4));

        DynamicResult r25 = results[25];
        Assert.AreEqual(215.4801, NullMath.Round(r25.Dynamic, 4));

        DynamicResult r250 = results[250];
        Assert.AreEqual(256.0554, NullMath.Round(r250.Dynamic, 4));

        DynamicResult r501 = results[501];
        Assert.AreEqual(245.7356, NullMath.Round(r501.Dynamic, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetDynamic(14)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(492, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<DynamicResult> r = badQuotes.GetDynamic(15);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Dynamic is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<DynamicResult> r0 = noquotes.GetDynamic(14);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<DynamicResult> r1 = onequote.GetDynamic(14);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<DynamicResult> results = quotes.GetDynamic(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(501, results.Count);

        DynamicResult last = results.LastOrDefault();
        Assert.AreEqual(245.7356, NullMath.Round(last.Dynamic, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetDynamic(quotes, 0));
}
