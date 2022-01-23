using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Roc : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<RocResult> results = quotes.GetRoc(20).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Where(x => x.Roc != null).Count());
        Assert.AreEqual(false, results.Any(x => x.RocSma != null));

        // sample values
        RocResult r1 = results[249];
        Assert.AreEqual(2.4827, Math.Round((double)r1.Roc, 4));
        Assert.AreEqual(null, r1.RocSma);

        RocResult r2 = results[501];
        Assert.AreEqual(-8.2482, Math.Round((double)r2.Roc, 4));
        Assert.AreEqual(null, r2.RocSma);
    }

    [TestMethod]
    public void WithSma()
    {
        int lookbackPeriods = 20;
        int smaPeriods = 5;

        List<RocResult> results = Indicator.GetRoc(quotes, lookbackPeriods, smaPeriods)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Where(x => x.Roc != null).Count());
        Assert.AreEqual(478, results.Where(x => x.RocSma != null).Count());

        // sample values
        RocResult r1 = results[29];
        Assert.AreEqual(3.2936, Math.Round((double)r1.Roc, 4));
        Assert.AreEqual(2.1558, Math.Round((double)r1.RocSma, 4));

        RocResult r2 = results[501];
        Assert.AreEqual(-8.2482, Math.Round((double)r2.Roc, 4));
        Assert.AreEqual(-8.4828, Math.Round((double)r2.RocSma, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<RocResult> r = Indicator.GetRoc(badQuotes, 35, 2);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<RocResult> r0 = noquotes.GetRoc(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<RocResult> r1 = onequote.GetRoc(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<RocResult> results = quotes.GetRoc(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        RocResult last = results.LastOrDefault();
        Assert.AreEqual(-8.2482, Math.Round((double)last.Roc, 4));
        Assert.AreEqual(null, last.RocSma);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRoc(quotes, 0));

        // bad SMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetRoc(quotes, 14, 0));
    }
}
