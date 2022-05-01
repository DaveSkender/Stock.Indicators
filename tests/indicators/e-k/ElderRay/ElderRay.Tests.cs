using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class ElderRay : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ElderRayResult> results = quotes.GetElderRay(13).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(490, results.Where(x => x.BullPower != null).Count());
        Assert.AreEqual(490, results.Where(x => x.BearPower != null).Count());

        // sample values
        ElderRayResult r1 = results[11];
        Assert.IsNull(r1.Ema);
        Assert.IsNull(r1.BullPower);
        Assert.IsNull(r1.BearPower);

        ElderRayResult r2 = results[12];
        Assert.AreEqual(214.0000m, NullMath.Round(r2.Ema, 4));
        Assert.AreEqual(0.7500m, r2.BullPower);
        Assert.AreEqual(-0.5100m, r2.BearPower);

        ElderRayResult r3 = results[24];
        Assert.AreEqual(215.5426m, NullMath.Round(r3.Ema, 4));
        Assert.AreEqual(1.4274m, NullMath.Round(r3.BullPower, 4));
        Assert.AreEqual(0.5474m, NullMath.Round(r3.BearPower, 4));

        ElderRayResult r4 = results[149];
        Assert.AreEqual(235.3970m, NullMath.Round(r4.Ema, 4));
        Assert.AreEqual(0.9430m, NullMath.Round(r4.BullPower, 4));
        Assert.AreEqual(0.4730m, NullMath.Round(r4.BearPower, 4));

        ElderRayResult r5 = results[249];
        Assert.AreEqual(256.5206m, NullMath.Round(r5.Ema, 4));
        Assert.AreEqual(1.5194m, NullMath.Round(r5.BullPower, 4));
        Assert.AreEqual(1.0694m, NullMath.Round(r5.BearPower, 4));

        ElderRayResult r6 = results[501];
        Assert.AreEqual(246.0129m, NullMath.Round(r6.Ema, 4));
        Assert.AreEqual(-0.4729m, NullMath.Round(r6.BullPower, 4));
        Assert.AreEqual(-3.1429m, NullMath.Round(r6.BearPower, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ElderRayResult> r = Indicator.GetElderRay(badQuotes);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<ElderRayResult> r0 = noquotes.GetElderRay();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<ElderRayResult> r1 = onequote.GetElderRay();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<ElderRayResult> results = quotes.GetElderRay(13)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (100 + 13), results.Count);

        ElderRayResult last = results.LastOrDefault();
        Assert.AreEqual(246.0129m, NullMath.Round(last.Ema, 4));
        Assert.AreEqual(-0.4729m, NullMath.Round(last.BullPower, 4));
        Assert.AreEqual(-3.1429m, NullMath.Round(last.BearPower, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetElderRay(quotes, 0));
    }
}
