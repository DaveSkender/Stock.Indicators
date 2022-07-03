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
        Assert.AreEqual(477, results.Count(x => x.AroonUp != null));
        Assert.AreEqual(477, results.Count(x => x.AroonDown != null));
        Assert.AreEqual(477, results.Count(x => x.Oscillator != null));

        // sample values
        AroonResult r1 = results[210];
        Assert.AreEqual(100, r1.AroonUp);
        Assert.AreEqual(000, r1.AroonDown);
        Assert.AreEqual(100, r1.Oscillator);

        AroonResult r2 = results[293];
        Assert.AreEqual(0, r2.AroonUp);
        Assert.AreEqual(40, r2.AroonDown);
        Assert.AreEqual(-40, r2.Oscillator);

        AroonResult r3 = results[298];
        Assert.AreEqual(0, r3.AroonUp);
        Assert.AreEqual(20, r3.AroonDown);
        Assert.AreEqual(-20, r3.Oscillator);

        AroonResult r4 = results[458];
        Assert.AreEqual(0, r4.AroonUp);
        Assert.AreEqual(100, r4.AroonDown);
        Assert.AreEqual(-100, r4.Oscillator);

        AroonResult r5 = results[501];
        Assert.AreEqual(28, r5.AroonUp);
        Assert.AreEqual(88, r5.AroonDown);
        Assert.AreEqual(-60, r5.Oscillator);
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetAroon(25)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(468, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AroonResult> r = Indicator.GetAroon(badQuotes, 20);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AroonResult> r0 = noquotes.GetAroon();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AroonResult> r1 = onequote.GetAroon();
        Assert.AreEqual(1, r1.Count());
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
        Assert.AreEqual(28, last.AroonUp);
        Assert.AreEqual(88, last.AroonDown);
        Assert.AreEqual(-60, last.Oscillator);
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetAroon(quotes, 0));
}
