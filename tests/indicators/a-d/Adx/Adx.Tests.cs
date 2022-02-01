using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Adx : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 14;
        List<AdxResult> results = quotes.GetAdx(lookbackPeriods).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(475, results.Where(x => x.Adx != null).Count());

        // sample values
        AdxResult r19 = results[19];
        Assert.AreEqual(21.0361, Math.Round((double)r19.Pdi, 4));
        Assert.AreEqual(25.0124, Math.Round((double)r19.Mdi, 4));
        Assert.AreEqual(null, r19.Adx);

        AdxResult r29 = results[29];
        Assert.AreEqual(37.9719, Math.Round((double)r29.Pdi, 4));
        Assert.AreEqual(14.1658, Math.Round((double)r29.Mdi, 4));
        Assert.AreEqual(19.7949, Math.Round((double)r29.Adx, 4));

        AdxResult r39 = results[39];
        Assert.IsNull(r29.Adxr);

        AdxResult r40 = results[40];
        Assert.AreEqual(29.1062, Math.Round((double)r40.Adxr, 4));

        AdxResult r248 = results[248];
        Assert.AreEqual(32.3167, Math.Round((double)r248.Pdi, 4));
        Assert.AreEqual(18.2471, Math.Round((double)r248.Mdi, 4));
        Assert.AreEqual(30.5903, Math.Round((double)r248.Adx, 4));
        Assert.AreEqual(29.1252, Math.Round((double)r248.Adxr, 4));

        AdxResult r501 = results[501];
        Assert.AreEqual(17.7565, Math.Round((double)r501.Pdi, 4));
        Assert.AreEqual(31.1510, Math.Round((double)r501.Mdi, 4));
        Assert.AreEqual(34.2987, Math.Round((double)r501.Adx, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AdxResult> r = badQuotes.GetAdx(20);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void BigData()
    {
        IEnumerable<AdxResult> r = bigQuotes.GetAdx(200);
        Assert.AreEqual(1246, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AdxResult> r0 = noquotes.GetAdx(5);
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AdxResult> r1 = onequote.GetAdx(5);
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        IEnumerable<AdxResult> r = quotes.GetAdx(14)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - ((2 * 14) + 100), r.Count());

        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, Math.Round((double)last.Pdi, 4));
        Assert.AreEqual(31.1510, Math.Round((double)last.Mdi, 4));
        Assert.AreEqual(34.2987, Math.Round((double)last.Adx, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAdx(quotes, 1));
    }
}
