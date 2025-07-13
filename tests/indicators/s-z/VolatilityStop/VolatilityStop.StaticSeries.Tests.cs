namespace StaticSeries;

[TestClass]
public class VolatilityStop : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<VolatilityStopResult> results =
            Quotes.ToVolatilityStop(14);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Count(x => x.Sar != null));

        // sample values
        VolatilityStopResult r53 = results[53];
        Assert.IsNull(r53.Sar);
        Assert.IsNull(r53.IsStop);
        Assert.IsNull(r53.LowerBand);
        Assert.IsNull(r53.UpperBand);

        VolatilityStopResult r54 = results[54];
        Assert.AreEqual(226.2118, r54.Sar.Round(4));
        Assert.AreEqual(false, r54.IsStop);
        Assert.AreEqual(226.2118, r54.UpperBand.Round(4));
        Assert.IsNull(r54.LowerBand);

        VolatilityStopResult r55 = results[55];
        Assert.AreEqual(226.2124, r55.Sar.Round(4));
        Assert.AreEqual(false, r55.IsStop);
        Assert.AreEqual(226.2124, r55.UpperBand.Round(4));
        Assert.IsNull(r55.LowerBand);

        VolatilityStopResult r168 = results[168];
        Assert.IsTrue(r168.IsStop);

        VolatilityStopResult r282 = results[282];
        Assert.AreEqual(261.8687, r282.Sar.Round(4));
        Assert.AreEqual(true, r282.IsStop);
        Assert.AreEqual(261.8687, r282.UpperBand.Round(4));
        Assert.IsNull(r282.LowerBand);

        VolatilityStopResult r283 = results[283];
        Assert.AreEqual(249.3219, r283.Sar.Round(4));
        Assert.AreEqual(false, r283.IsStop);
        Assert.AreEqual(249.3219, r283.LowerBand.Round(4));
        Assert.IsNull(r283.UpperBand);

        VolatilityStopResult r284 = results[284];
        Assert.AreEqual(249.7460, r284.Sar.Round(4));
        Assert.AreEqual(false, r284.IsStop);
        Assert.AreEqual(249.7460, r284.LowerBand.Round(4));
        Assert.IsNull(r284.UpperBand);

        VolatilityStopResult last = results[^1];
        Assert.AreEqual(249.2423, last.Sar.Round(4));
        Assert.AreEqual(false, last.IsStop);
        Assert.AreEqual(249.2423, last.UpperBand.Round(4));
        Assert.IsNull(last.LowerBand);
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToVolatilityStop()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(439, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<VolatilityStopResult> r = BadQuotes
            .ToVolatilityStop();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Sar is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<VolatilityStopResult> r0 = Noquotes
            .ToVolatilityStop();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<VolatilityStopResult> r1 = Onequote
            .ToVolatilityStop();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<VolatilityStopResult> results = Quotes
            .ToVolatilityStop(14)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(402, results.Count);

        VolatilityStopResult last = results[^1];
        Assert.AreEqual(249.2423, last.Sar.Round(4));
        Assert.AreEqual(false, last.IsStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToVolatilityStop(1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToVolatilityStop(20, 0));
    }
}
