using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Marubozu : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<MarubozuResult> results = quotes.GetMarubozu(0.95).ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(6, results.Where(x => x.Marubozu != null).Count());

        // sample values
        MarubozuResult r31 = results[31];
        Assert.AreEqual(null, r31.Marubozu);
        Assert.AreEqual(false, r31.IsBullish);

        MarubozuResult r32 = results[32];
        Assert.AreEqual(222.10m, r32.Marubozu);
        Assert.AreEqual(true, r32.IsBullish);

        MarubozuResult r277 = results[277];
        Assert.AreEqual(248.13m, r277.Marubozu);
        Assert.AreEqual(false, r277.IsBullish);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<MarubozuResult> r = Indicator.GetMarubozu(badQuotes);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<MarubozuResult> r0 = noquotes.GetMarubozu();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<MarubozuResult> r1 = onequote.GetMarubozu();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad minimum body percent values
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMarubozu(quotes, 0.799));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMarubozu(quotes, 1.001));
    }
}
