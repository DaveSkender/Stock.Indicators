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
        Assert.AreEqual(475, results.Count(x => x.Adx != null));
        Assert.AreEqual(462, results.Count(x => x.Adxr != null));

        // sample values
        AdxResult r19 = results[19];
        Assert.AreEqual(21.0361, NullMath.Round(r19.Pdi, 4));
        Assert.AreEqual(25.0124, NullMath.Round(r19.Mdi, 4));
        Assert.AreEqual(null, r19.Adx);

        AdxResult r29 = results[29];
        Assert.AreEqual(37.9719, NullMath.Round(r29.Pdi, 4));
        Assert.AreEqual(14.1658, NullMath.Round(r29.Mdi, 4));
        Assert.AreEqual(19.7949, NullMath.Round(r29.Adx, 4));

        AdxResult r39 = results[39];
        Assert.IsNull(r29.Adxr);

        AdxResult r40 = results[40];
        Assert.AreEqual(29.1062, NullMath.Round(r40.Adxr, 4));

        AdxResult r248 = results[248];
        Assert.AreEqual(32.3167, NullMath.Round(r248.Pdi, 4));
        Assert.AreEqual(18.2471, NullMath.Round(r248.Mdi, 4));
        Assert.AreEqual(30.5903, NullMath.Round(r248.Adx, 4));
        Assert.AreEqual(29.1252, NullMath.Round(r248.Adxr, 4));

        AdxResult r501 = results[501];
        Assert.AreEqual(17.7565, NullMath.Round(r501.Pdi, 4));
        Assert.AreEqual(31.1510, NullMath.Round(r501.Mdi, 4));
        Assert.AreEqual(34.2987, NullMath.Round(r501.Adx, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetAdx(14)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(466, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AdxResult> r = badQuotes.GetAdx(20);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Adx is double and double.NaN));
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
        Assert.AreEqual(17.7565, NullMath.Round(last.Pdi, 4));
        Assert.AreEqual(31.1510, NullMath.Round(last.Mdi, 4));
        Assert.AreEqual(34.2987, NullMath.Round(last.Adx, 4));
    }

    [TestMethod]
    public void Exceptions() =>

        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAdx(quotes, 1));
}
