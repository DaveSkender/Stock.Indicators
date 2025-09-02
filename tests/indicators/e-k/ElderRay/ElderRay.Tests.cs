namespace Tests.Indicators;

[TestClass]
public class ElderRayTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<ElderRayResult> results = quotes
            .GetElderRay(13)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(490, results.Count(x => x.BullPower != null));
        Assert.AreEqual(490, results.Count(x => x.BearPower != null));

        // sample values
        ElderRayResult r1 = results[11];
        Assert.IsNull(r1.Ema);
        Assert.IsNull(r1.BullPower);
        Assert.IsNull(r1.BearPower);

        ElderRayResult r2 = results[12];
        Assert.AreEqual(214.0000, r2.Ema.Round(4));
        Assert.AreEqual(0.7500, r2.BullPower.Round(4));
        Assert.AreEqual(-0.5100, r2.BearPower.Round(4));

        ElderRayResult r3 = results[24];
        Assert.AreEqual(215.5426, r3.Ema.Round(4));
        Assert.AreEqual(1.4274, r3.BullPower.Round(4));
        Assert.AreEqual(0.5474, r3.BearPower.Round(4));

        ElderRayResult r4 = results[149];
        Assert.AreEqual(235.3970, r4.Ema.Round(4));
        Assert.AreEqual(0.9430, r4.BullPower.Round(4));
        Assert.AreEqual(0.4730, r4.BearPower.Round(4));

        ElderRayResult r5 = results[249];
        Assert.AreEqual(256.5206, r5.Ema.Round(4));
        Assert.AreEqual(1.5194, r5.BullPower.Round(4));
        Assert.AreEqual(1.0694, r5.BearPower.Round(4));

        ElderRayResult r6 = results[501];
        Assert.AreEqual(246.0129, r6.Ema.Round(4));
        Assert.AreEqual(-0.4729, r6.BullPower.Round(4));
        Assert.AreEqual(-3.1429, r6.BearPower.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetElderRay(13)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(481, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ElderRayResult> r = badQuotes
            .GetElderRay()
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.BullPower is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ElderRayResult> r0 = noquotes
            .GetElderRay()
            .ToList();

        Assert.IsEmpty(r0);

        List<ElderRayResult> r1 = onequote
            .GetElderRay()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<ElderRayResult> results = quotes
            .GetElderRay(13)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - (100 + 13), results);

        ElderRayResult last = results.LastOrDefault();
        Assert.AreEqual(246.0129, last.Ema.Round(4));
        Assert.AreEqual(-0.4729, last.BullPower.Round(4));
        Assert.AreEqual(-3.1429, last.BearPower.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetElderRay(0));
}
