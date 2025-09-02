namespace Tests.Indicators;

[TestClass]
public class ChandelierTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 22;

        List<ChandelierResult> longResult =
            quotes.GetChandelier(lookbackPeriods, 3)
            .ToList();

        // proper quantities
        Assert.HasCount(502, longResult);
        Assert.AreEqual(480, longResult.Count(x => x.ChandelierExit != null));

        // sample values (long)
        ChandelierResult a = longResult[501];
        Assert.AreEqual(256.5860, a.ChandelierExit.Round(4));

        ChandelierResult b = longResult[492];
        Assert.AreEqual(259.0480, b.ChandelierExit.Round(4));

        // short
        List<ChandelierResult> shortResult =
            quotes.GetChandelier(lookbackPeriods, 3, ChandelierType.Short)
            .ToList();

        ChandelierResult c = shortResult[501];
        Assert.AreEqual(246.4240, c.ChandelierExit.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetChandelier(22)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(471, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<ChandelierResult> r = badQuotes
            .GetChandelier(15, 2)
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.ChandelierExit is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ChandelierResult> r0 = noquotes
            .GetChandelier()
            .ToList();

        Assert.IsEmpty(r0);

        List<ChandelierResult> r1 = onequote
            .GetChandelier()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<ChandelierResult> longResult = quotes
            .GetChandelier(22, 3)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 22, longResult);

        ChandelierResult last = longResult.LastOrDefault();
        Assert.AreEqual(256.5860, last.ChandelierExit.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetChandelier(0));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetChandelier(25, 0));

        // bad type
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            quotes.GetChandelier(25, 2, (ChandelierType)int.MaxValue));
    }
}
