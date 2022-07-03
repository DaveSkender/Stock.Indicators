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
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
        Assert.AreEqual(false, results.Any(x => x.RocSma != null));

        // sample values
        RocResult r1 = results[249];
        Assert.AreEqual(2.4827, NullMath.Round(r1.Roc, 4));
        Assert.AreEqual(null, r1.RocSma);

        RocResult r2 = results[501];
        Assert.AreEqual(-8.2482, NullMath.Round(r2.Roc, 4));
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
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
        Assert.AreEqual(478, results.Count(x => x.RocSma != null));

        // sample values
        RocResult r1 = results[29];
        Assert.AreEqual(3.2936, NullMath.Round(r1.Roc, 4));
        Assert.AreEqual(2.1558, NullMath.Round(r1.RocSma, 4));

        RocResult r2 = results[501];
        Assert.AreEqual(-8.2482, NullMath.Round(r2.Roc, 4));
        Assert.AreEqual(-8.4828, NullMath.Round(r2.RocSma, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<RocResult> results = quotes
            .Use(CandlePart.Close)
            .GetRoc(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<RocResult> r = tupleNanny.GetRoc(6);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Roc is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<RocResult> results = quotes
            .GetSma(2)
            .GetRoc(20);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(481, results.Count(x => x.Roc != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetRoc(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<RocResult> r = Indicator.GetRoc(badQuotes, 35, 2);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Roc is double and double.NaN));
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
        Assert.AreEqual(-8.2482, NullMath.Round(last.Roc, 4));
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
