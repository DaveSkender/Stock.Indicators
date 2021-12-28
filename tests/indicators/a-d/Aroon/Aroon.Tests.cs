using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Aroon : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AroonResult> results = quotes.GetAroon(25).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(477, results.Where(x => x.AroonUp != null).Count());
        Assert.AreEqual(477, results.Where(x => x.AroonDown != null).Count());
        Assert.AreEqual(477, results.Where(x => x.Oscillator != null).Count());

        // sample values
        AroonResult r1 = results[210];
        Assert.AreEqual(100m, r1.AroonUp);
        Assert.AreEqual(000m, r1.AroonDown);
        Assert.AreEqual(100m, r1.Oscillator);

        AroonResult r2 = results[293];
        Assert.AreEqual(0m, r2.AroonUp);
        Assert.AreEqual(40m, r2.AroonDown);
        Assert.AreEqual(-40m, r2.Oscillator);

        AroonResult r3 = results[298];
        Assert.AreEqual(0m, r3.AroonUp);
        Assert.AreEqual(20m, r3.AroonDown);
        Assert.AreEqual(-20m, r3.Oscillator);

        AroonResult r4 = results[458];
        Assert.AreEqual(0m, r4.AroonUp);
        Assert.AreEqual(100m, r4.AroonDown);
        Assert.AreEqual(-100m, r4.Oscillator);

        AroonResult r5 = results[501];
        Assert.AreEqual(28m, r5.AroonUp);
        Assert.AreEqual(88m, r5.AroonDown);
        Assert.AreEqual(-60m, r5.Oscillator);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AroonResult> r = Indicator.GetAroon(badQuotes, 20);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<AroonResult> results = quotes.GetAroon(25)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 25, results.Count);

        AroonResult last = results.LastOrDefault();
        Assert.AreEqual(28m, last.AroonUp);
        Assert.AreEqual(88m, last.AroonDown);
        Assert.AreEqual(-60m, last.Oscillator);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAroon(quotes, 0));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetAroon(TestData.GetDefault(29), 30));
    }
}
