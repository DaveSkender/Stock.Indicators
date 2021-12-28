using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Chandeleir : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 22;

        List<ChandelierResult> longResult =
            quotes.GetChandelier(lookbackPeriods, 3)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, longResult.Count);
        Assert.AreEqual(481, longResult.Where(x => x.ChandelierExit != null).Count());

        // sample values (long)
        ChandelierResult a = longResult[501];
        Assert.AreEqual(256.5860m, Math.Round((decimal)a.ChandelierExit, 4));

        ChandelierResult b = longResult[492];
        Assert.AreEqual(259.0480m, Math.Round((decimal)b.ChandelierExit, 4));

        // short
        List<ChandelierResult> shortResult =
            Indicator.GetChandelier(quotes, lookbackPeriods, 3, ChandelierType.Short)
            .ToList();

        ChandelierResult c = shortResult[501];
        Assert.AreEqual(246.4240m, Math.Round((decimal)c.ChandelierExit, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ChandelierResult> r = Indicator.GetChandelier(badQuotes, 15, 2);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<ChandelierResult> longResult =
            quotes.GetChandelier(22, 3)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(502 - 21, longResult.Count);

        ChandelierResult last = longResult.LastOrDefault();
        Assert.AreEqual(256.5860m, Math.Round((decimal)last.ChandelierExit, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetChandelier(quotes, 0));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetChandelier(quotes, 25, 0));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetChandelier(TestData.GetDefault(30), 30));
    }
}
