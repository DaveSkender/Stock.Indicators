namespace Tests.Indicators;

[TestClass]
public class VolatilityStopTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VolatilityStopResult> results =
            quotes.GetVolatilityStop(14, 3)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(448, results.Where(x => x.Sar != null));

        // sample values
        VolatilityStopResult r53 = results[53];
        Assert.IsNull(r53.Sar);
        Assert.IsNull(r53.IsStop);
        Assert.IsNull(r53.LowerBand);
        Assert.IsNull(r53.UpperBand);

        VolatilityStopResult r54 = results[54];
        Assert.AreEqual(226.2118, r54.Sar.Round(4));
        Assert.IsFalse(r54.IsStop);
        Assert.AreEqual(226.2118, r54.UpperBand.Round(4));
        Assert.IsNull(r54.LowerBand);

        VolatilityStopResult r55 = results[55];
        Assert.AreEqual(226.2124, r55.Sar.Round(4));
        Assert.IsFalse(r55.IsStop);
        Assert.AreEqual(226.2124, r55.UpperBand.Round(4));
        Assert.IsNull(r55.LowerBand);

        VolatilityStopResult r168 = results[168];
        Assert.IsTrue(r168.IsStop);

        VolatilityStopResult r282 = results[282];
        Assert.AreEqual(261.8687, r282.Sar.Round(4));
        Assert.IsTrue(r282.IsStop);
        Assert.AreEqual(261.8687, r282.UpperBand.Round(4));
        Assert.IsNull(r282.LowerBand);

        VolatilityStopResult r283 = results[283];
        Assert.AreEqual(249.3219, r283.Sar.Round(4));
        Assert.IsFalse(r283.IsStop);
        Assert.AreEqual(249.3219, r283.LowerBand.Round(4));
        Assert.IsNull(r283.UpperBand);

        VolatilityStopResult r284 = results[284];
        Assert.AreEqual(249.7460, r284.Sar.Round(4));
        Assert.IsFalse(r284.IsStop);
        Assert.AreEqual(249.7460, r284.LowerBand.Round(4));
        Assert.IsNull(r284.UpperBand);

        VolatilityStopResult last = results.LastOrDefault();
        Assert.AreEqual(249.2423, last.Sar.Round(4));
        Assert.IsFalse(last.IsStop);
        Assert.AreEqual(249.2423, last.UpperBand.Round(4));
        Assert.IsNull(last.LowerBand);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetVolatilityStop()
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(439, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<VolatilityStopResult> r = badQuotes
            .GetVolatilityStop()
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Sar is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<VolatilityStopResult> r0 = noquotes
            .GetVolatilityStop()
            .ToList();

        Assert.IsEmpty(r0);

        List<VolatilityStopResult> r1 = onequote
            .GetVolatilityStop()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<VolatilityStopResult> results = quotes
            .GetVolatilityStop(14, 3)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(402, results);

        VolatilityStopResult last = results.LastOrDefault();
        Assert.AreEqual(249.2423, last.Sar.Round(4));
        Assert.IsFalse(last.IsStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetVolatilityStop(1));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetVolatilityStop(20, 0));
    }
}
