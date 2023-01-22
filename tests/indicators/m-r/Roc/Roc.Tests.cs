using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class RocTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<RocResult> results = quotes
            .GetRoc(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Momentum != null));
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
        Assert.AreEqual(false, results.Any(x => x.RocSma != null));

        // sample values
        RocResult r49 = results[49];
        Assert.AreEqual(4.96, r49.Momentum.Round(4));
        Assert.AreEqual(2.2465, r49.Roc.Round(4));
        Assert.AreEqual(null, r49.RocSma);

        RocResult r249 = results[249];
        Assert.AreEqual(6.25, r249.Momentum.Round(4));
        Assert.AreEqual(2.4827, r249.Roc.Round(4));
        Assert.AreEqual(null, r249.RocSma);

        RocResult r501 = results[501];
        Assert.AreEqual(-22.05, r501.Momentum.Round(4));
        Assert.AreEqual(-8.2482, r501.Roc.Round(4));
        Assert.AreEqual(null, r501.RocSma);
    }

    [TestMethod]
    public void WithSma()
    {
        int lookbackPeriods = 20;
        int smaPeriods = 5;

        List<RocResult> results = quotes
            .GetRoc(lookbackPeriods, smaPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
        Assert.AreEqual(478, results.Count(x => x.RocSma != null));

        // sample values
        RocResult r1 = results[29];
        Assert.AreEqual(3.2936, r1.Roc.Round(4));
        Assert.AreEqual(2.1558, r1.RocSma.Round(4));

        RocResult r2 = results[501];
        Assert.AreEqual(-8.2482, r2.Roc.Round(4));
        Assert.AreEqual(-8.4828, r2.RocSma.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<RocResult> results = quotes
            .Use(CandlePart.Close)
            .GetRoc(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<RocResult> r = tupleNanny
            .GetRoc(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Roc is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<RocResult> results = quotes
            .GetSma(2)
            .GetRoc(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetRoc(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<RocResult> r = badQuotes
            .GetRoc(35, 2)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Roc is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<RocResult> r0 = noquotes
            .GetRoc(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<RocResult> r1 = onequote
            .GetRoc(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<RocResult> results = quotes
            .GetRoc(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 20, results.Count);

        RocResult last = results.LastOrDefault();
        Assert.AreEqual(-8.2482, last.Roc.Round(4));
        Assert.AreEqual(null, last.RocSma);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetRoc(0));

        // bad SMA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetRoc(14, 0));
    }
}
