using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Awesome : TestBase
{

    [TestMethod]
    public void Standard()
    {

        List<AwesomeResult> results = quotes.GetAwesome(5, 34)
            .ToList();

        // assertions

        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(469, results.Where(x => x.Oscillator != null).Count());

        // sample values
        AwesomeResult r1 = results[32];
        Assert.AreEqual(null, r1.Oscillator);
        Assert.AreEqual(null, r1.Normalized);

        AwesomeResult r2 = results[33];
        Assert.AreEqual(5.4756, Math.Round((double)r2.Oscillator, 4));
        Assert.AreEqual(2.4548, Math.Round((double)r2.Normalized, 4));

        AwesomeResult r3 = results[249];
        Assert.AreEqual(5.0618, Math.Round((double)r3.Oscillator, 4));
        Assert.AreEqual(1.9634, Math.Round((double)r3.Normalized, 4));

        AwesomeResult r4 = results[501];
        Assert.AreEqual(-17.7692, Math.Round((double)r4.Oscillator, 4));
        Assert.AreEqual(-7.2763, Math.Round((double)r4.Normalized, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AwesomeResult> r = Indicator.GetAwesome(badQuotes);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<AwesomeResult> results = quotes.GetAwesome(5, 34)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 33, results.Count);

        AwesomeResult last = results.LastOrDefault();
        Assert.AreEqual(-17.7692, Math.Round((double)last.Oscillator, 4));
        Assert.AreEqual(-7.2763, Math.Round((double)last.Normalized, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAwesome(quotes, 0, 34));

        // bad slow period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAwesome(quotes, 25, 25));

        // insufficient quotes
        Assert.ThrowsException<BadQuotesException>(() =>
            Indicator.GetAwesome(TestData.GetDefault(33), 5, 34));
    }
}
