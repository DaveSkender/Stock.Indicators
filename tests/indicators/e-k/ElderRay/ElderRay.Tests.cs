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
        Assert.AreEqual(490, results.Count(x => x.BullPower != null));
        Assert.AreEqual(490, results.Count(x => x.BearPower != null));

        // sample values
        ElderRayResult r1 = results[11];
        Assert.IsNull(r1.Ema);
        Assert.IsNull(r1.BullPower);
        Assert.IsNull(r1.BearPower);

        ElderRayResult r2 = results[12];
        Assert.AreEqual(214.0000, NullMath.Round(r2.Ema, 4));
        Assert.AreEqual(0.7500, NullMath.Round(r2.BullPower, 4));
        Assert.AreEqual(-0.5100, NullMath.Round(r2.BearPower, 4));

        ElderRayResult r3 = results[24];
        Assert.AreEqual(215.5426, NullMath.Round(r3.Ema, 4));
        Assert.AreEqual(1.4274, NullMath.Round(r3.BullPower, 4));
        Assert.AreEqual(0.5474, NullMath.Round(r3.BearPower, 4));

        ElderRayResult r4 = results[149];
        Assert.AreEqual(235.3970, NullMath.Round(r4.Ema, 4));
        Assert.AreEqual(0.9430, NullMath.Round(r4.BullPower, 4));
        Assert.AreEqual(0.4730, NullMath.Round(r4.BearPower, 4));

        ElderRayResult r5 = results[249];
        Assert.AreEqual(256.5206, NullMath.Round(r5.Ema, 4));
        Assert.AreEqual(1.5194, NullMath.Round(r5.BullPower, 4));
        Assert.AreEqual(1.0694, NullMath.Round(r5.BearPower, 4));

        ElderRayResult r6 = results[501];
        Assert.AreEqual(246.0129, NullMath.Round(r6.Ema, 4));
        Assert.AreEqual(-0.4729, NullMath.Round(r6.BullPower, 4));
        Assert.AreEqual(-3.1429, NullMath.Round(r6.BearPower, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetElderRay(13)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(481, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<ElderRayResult> r = Indicator.GetElderRay(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.BullPower is double and double.NaN));
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
        Assert.AreEqual(246.0129, NullMath.Round(last.Ema, 4));
        Assert.AreEqual(-0.4729, NullMath.Round(last.BullPower, 4));
        Assert.AreEqual(-3.1429, NullMath.Round(last.BearPower, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetElderRay(quotes, 0));
}
