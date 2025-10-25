namespace StaticSeries;

[TestClass]
public class Chandelier : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        const int lookbackPeriods = 22;

        IReadOnlyList<ChandelierResult> longResult =
            Quotes.ToChandelier(lookbackPeriods);

        // proper quantities
        Assert.HasCount(502, longResult);
        Assert.HasCount(480, longResult.Where(static x => x.ChandelierExit != null));

        // sample values (long)
        ChandelierResult a = longResult[501];
        Assert.AreEqual(256.5860, a.ChandelierExit.Round(4));

        ChandelierResult b = longResult[492];
        Assert.AreEqual(259.0480, b.ChandelierExit.Round(4));

        // short
        IReadOnlyList<ChandelierResult> shortResult =
            Quotes.ToChandelier(lookbackPeriods, 3, Direction.Short);

        ChandelierResult c = shortResult[501];
        Assert.AreEqual(246.4240, c.ChandelierExit.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToChandelier()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(471, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ChandelierResult> r = BadQuotes
            .ToChandelier(15, 2);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.ChandelierExit is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ChandelierResult> r0 = Noquotes
            .ToChandelier();

        Assert.IsEmpty(r0);

        IReadOnlyList<ChandelierResult> r1 = Onequote
            .ToChandelier();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ChandelierResult> results = Quotes
            .ToChandelier()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 22, results);

        ChandelierResult last = results[^1];
        Assert.AreEqual(256.5860, last.ChandelierExit.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToChandelier(0));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToChandelier(25, 0));

        // bad type
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToChandelier(25, 2, (Direction)int.MaxValue));
    }
}
