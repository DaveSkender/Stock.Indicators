using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Dema : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<DemaResult> results = quotes.GetDema(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Dema != null).Count());

        // sample values
        DemaResult r25 = results[25];
        Assert.AreEqual(215.7605m, Math.Round((decimal)r25.Dema, 4));

        DemaResult r51 = results[51];
        Assert.AreEqual(225.8259m, Math.Round((decimal)r51.Dema, 4));

        DemaResult r249 = results[249];
        Assert.AreEqual(258.4452m, Math.Round((decimal)r249.Dema, 4));

        DemaResult r251 = results[501];
        Assert.AreEqual(241.1677m, Math.Round((decimal)r251.Dema, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<DemaResult> r = Indicator.GetDema(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<DemaResult> r0 = noquotes.GetDema(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<DemaResult> r1 = onequote.GetDema(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<DemaResult> results = quotes.GetDema(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (40 + 100), results.Count);

        DemaResult last = results.LastOrDefault();
        Assert.AreEqual(241.1677m, Math.Round((decimal)last.Dema, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetDema(0));
    }
}
