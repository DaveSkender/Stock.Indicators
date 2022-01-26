using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class VolatilityStop : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VolatilityStopResult> results =
            quotes.GetVolatilityStop(14, 3)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(448, results.Where(x => x.Sar != null).Count());

        // sample values
        VolatilityStopResult r53 = results[53];
        Assert.IsNull(r53.Sar);
        Assert.IsNull(r53.IsStop);
        Assert.IsNull(r53.LowerBand);
        Assert.IsNull(r53.UpperBand);

        VolatilityStopResult r54 = results[54];
        Assert.AreEqual(226.2177m, Math.Round((decimal)r54.Sar, 4));
        Assert.AreEqual(false, r54.IsStop);
        Assert.AreEqual(226.2177m, Math.Round((decimal)r54.UpperBand, 4));
        Assert.IsNull(r54.LowerBand);

        VolatilityStopResult r55 = results[55];
        Assert.AreEqual(226.2178m, Math.Round((decimal)r55.Sar, 4));
        Assert.AreEqual(false, r55.IsStop);
        Assert.AreEqual(226.2178m, Math.Round((decimal)r55.UpperBand, 4));
        Assert.IsNull(r55.LowerBand);

        VolatilityStopResult r168 = results[168];
        Assert.IsTrue(r168.IsStop);

        VolatilityStopResult r282 = results[282];
        Assert.AreEqual(261.8687m, Math.Round((decimal)r282.Sar, 4));
        Assert.AreEqual(true, r282.IsStop);
        Assert.AreEqual(261.8687m, Math.Round((decimal)r282.UpperBand, 4));
        Assert.IsNull(r282.LowerBand);

        VolatilityStopResult r283 = results[283];
        Assert.AreEqual(249.3219m, Math.Round((decimal)r283.Sar, 4));
        Assert.AreEqual(false, r283.IsStop);
        Assert.AreEqual(249.3219m, Math.Round((decimal)r283.LowerBand, 4));
        Assert.IsNull(r283.UpperBand);

        VolatilityStopResult r284 = results[284];
        Assert.AreEqual(249.7460m, Math.Round((decimal)r284.Sar, 4));
        Assert.AreEqual(false, r284.IsStop);
        Assert.AreEqual(249.7460m, Math.Round((decimal)r284.LowerBand, 4));
        Assert.IsNull(r284.UpperBand);

        VolatilityStopResult last = results.LastOrDefault();
        Assert.AreEqual(249.2423m, Math.Round((decimal)last.Sar, 4));
        Assert.AreEqual(false, last.IsStop);
        Assert.AreEqual(249.2423m, Math.Round((decimal)last.UpperBand, 4));
        Assert.IsNull(last.LowerBand);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<VolatilityStopResult> r = Indicator.GetVolatilityStop(badQuotes);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<VolatilityStopResult> r0 = noquotes.GetVolatilityStop();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<VolatilityStopResult> r1 = onequote.GetVolatilityStop();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<VolatilityStopResult> results =
            quotes.GetVolatilityStop(14, 3)
                .RemoveWarmupPeriods()
                .ToList();

        // assertions
        Assert.AreEqual(402, results.Count);

        VolatilityStopResult last = results.LastOrDefault();
        Assert.AreEqual(249.2423m, Math.Round((decimal)last.Sar, 4));
        Assert.AreEqual(false, last.IsStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetVolatilityStop(1));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetVolatilityStop(quotes, 20, 0));
    }
}
