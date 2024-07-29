namespace StaticSeries;

[TestClass]
public class ElderRayTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<ElderRayResult> results = Quotes
            .GetElderRay();

        // proper quantities
        Assert.AreEqual(502, results.Count);
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
        IReadOnlyList<SmaResult> results = Quotes
            .GetElderRay()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ElderRayResult> r = BadQuotes
            .GetElderRay();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.BullPower is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ElderRayResult> r0 = Noquotes
            .GetElderRay();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ElderRayResult> r1 = Onequote
            .GetElderRay();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ElderRayResult> results = Quotes
            .GetElderRay()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (100 + 13), results.Count);

        ElderRayResult last = results[^1];
        Assert.AreEqual(246.0129, last.Ema.Round(4));
        Assert.AreEqual(-0.4729, last.BullPower.Round(4));
        Assert.AreEqual(-3.1429, last.BearPower.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetElderRay(0));
}
