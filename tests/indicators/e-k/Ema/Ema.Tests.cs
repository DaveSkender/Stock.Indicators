using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Ema : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<EmaResult> results = quotes.GetEma(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Ema != null).Count());

        // sample values
        EmaResult r29 = results[29];
        Assert.AreEqual(216.6228m, Math.Round((decimal)r29.Ema, 4));

        EmaResult r249 = results[249];
        Assert.AreEqual(255.3873m, Math.Round((decimal)r249.Ema, 4));

        EmaResult r501 = results[501];
        Assert.AreEqual(249.3519m, Math.Round((decimal)r501.Ema, 4));
    }

    [TestMethod]
    public void Custom()
    {
        List<EmaResult> results = quotes.GetEma(20, CandlePart.Open)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Ema != null).Count());

        // sample values
        EmaResult r29 = results[29];
        Assert.AreEqual(216.2643m, Math.Round((decimal)r29.Ema, 4));

        EmaResult r249 = results[249];
        Assert.AreEqual(255.4875m, Math.Round((decimal)r249.Ema, 4));

        EmaResult r501 = results[501];
        Assert.AreEqual(249.9157m, Math.Round((decimal)r501.Ema, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<EmaResult> r = Indicator.GetEma(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<EmaResult> r0 = noquotes.GetEma(10);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<EmaResult> r1 = onequote.GetEma(10);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<EmaResult> results = quotes.GetEma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (20 + 100), results.Count);

        EmaResult last = results.LastOrDefault();
        Assert.AreEqual(249.3519m, Math.Round((decimal)last.Ema, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetEma(quotes, 0));
    }
}
