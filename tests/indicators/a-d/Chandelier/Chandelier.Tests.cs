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
        Assert.AreEqual(481, longResult.Count(x => x.ChandelierExit != null));

        // sample values (long)
        ChandelierResult a = longResult[501];
        Assert.AreEqual(256.5860, NullMath.Round(a.ChandelierExit, 4));

        ChandelierResult b = longResult[492];
        Assert.AreEqual(259.0480, NullMath.Round(b.ChandelierExit, 4));

        // short
        List<ChandelierResult> shortResult =
            Indicator.GetChandelier(quotes, lookbackPeriods, 3, ChandelierType.Short)
            .ToList();

        ChandelierResult c = shortResult[501];
        Assert.AreEqual(246.4240, NullMath.Round(c.ChandelierExit, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetChandelier(22)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(472, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ChandelierResult> r = Indicator.GetChandelier(badQuotes, 15, 2);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.ChandelierExit is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ChandelierResult> r0 = noquotes.GetChandelier();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ChandelierResult> r1 = onequote.GetChandelier();
        Assert.AreEqual(1, r1.Count());
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
        Assert.AreEqual(256.5860, NullMath.Round(last.ChandelierExit, 4));
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
    }
}
