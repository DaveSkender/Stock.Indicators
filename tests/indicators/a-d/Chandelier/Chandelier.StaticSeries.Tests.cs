namespace StaticSeries;

[TestClass]
public class Chandelier : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int lookbackPeriods = 22;

        IReadOnlyList<ChandelierResult> longResult =
            Quotes.GetChandelier(lookbackPeriods);

        // proper quantities
        Assert.AreEqual(502, longResult.Count);
        Assert.AreEqual(480, longResult.Count(x => x.ChandelierExit != null));

        // sample values (long)
        ChandelierResult a = longResult[501];
        Assert.AreEqual(256.5860, a.ChandelierExit.Round(4));

        ChandelierResult b = longResult[492];
        Assert.AreEqual(259.0480, b.ChandelierExit.Round(4));

        // short
        IReadOnlyList<ChandelierResult> shortResult =
            Quotes.GetChandelier(lookbackPeriods, 3, ChandelierType.Short);

        ChandelierResult c = shortResult[501];
        Assert.AreEqual(246.4240, c.ChandelierExit.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetChandelier()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(471, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ChandelierResult> r = BadQuotes
            .GetChandelier(15, 2);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.ChandelierExit is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ChandelierResult> r0 = Noquotes
            .GetChandelier();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ChandelierResult> r1 = Onequote
            .GetChandelier();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ChandelierResult> results = Quotes
            .GetChandelier()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 22, results.Count);

        ChandelierResult last = results[^1];
        Assert.AreEqual(256.5860, last.ChandelierExit.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetChandelier(0));

        // bad multiplier
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetChandelier(25, 0));

        // bad type
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetChandelier(25, 2, (ChandelierType)int.MaxValue));
    }
}
