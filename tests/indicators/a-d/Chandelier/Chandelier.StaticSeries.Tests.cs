namespace StaticSeries;

[TestClass]
public class Chandelier : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int lookbackPeriods = 22;

        IReadOnlyList<ChandelierResult> longResult =
            Quotes.ToChandelier(lookbackPeriods);

        // proper quantities
        longResult.Should().HaveCount(502);
        longResult.Where(static x => x.ChandelierExit != null).Should().HaveCount(480);

        // sample values (long)
        ChandelierResult a = longResult[501];
        a.ChandelierExit.Should().BeApproximately(256.5860, Money4);

        ChandelierResult b = longResult[492];
        b.ChandelierExit.Should().BeApproximately(259.0480, Money4);

        // short
        IReadOnlyList<ChandelierResult> shortResult =
            Quotes.ToChandelier(lookbackPeriods, 3, Direction.Short);

        ChandelierResult c = shortResult[501];
        c.ChandelierExit.Should().BeApproximately(246.4240, Money4);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToChandelier()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(471);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ChandelierResult> r = BadQuotes
            .ToChandelier(15, 2);

        r.Should().HaveCount(502);
        r.Where(static x => x.ChandelierExit is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ChandelierResult> r0 = Noquotes
            .ToChandelier();

        r0.Should().BeEmpty();

        IReadOnlyList<ChandelierResult> r1 = Onequote
            .ToChandelier();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<ChandelierResult> sut = Quotes
            .ToChandelier()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 22);

        ChandelierResult last = sut[^1];
        last.ChandelierExit.Should().BeApproximately(256.5860, Money4);
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
