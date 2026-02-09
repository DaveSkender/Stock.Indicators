namespace StaticSeries;

[TestClass]
public class StochRsi : StaticSeriesTestBase
{
    /// <summary>
    /// Fast RSI
    /// </summary>
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;

        IReadOnlyList<StochRsiResult> sut =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        sut.Should().HaveCount(502);
        Assert.HasCount(475, sut.Where(static x => x.StochRsi != null));
        Assert.HasCount(473, sut.Where(static x => x.Signal != null));

        // sample values
        StochRsiResult r1 = sut[31];
        r1.StochRsi.Should().BeApproximately(93.3333, Money4);
        r1.Signal.Should().BeApproximately(97.7778, Money4);

        StochRsiResult r2 = sut[152];
        r2.StochRsi.Should().Be(0);
        r2.Signal.Should().Be(0);

        StochRsiResult r3 = sut[249];
        r3.StochRsi.Should().BeApproximately(36.5517, Money4);
        r3.Signal.Should().BeApproximately(27.3094, Money4);

        StochRsiResult r4 = sut[501];
        r4.StochRsi.Should().BeApproximately(97.5244, Money4);
        r4.Signal.Should().BeApproximately(89.8385, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes.ToStochRsi(14, 14, 3, 1);
        sut.IsBetween(static x => x.StochRsi, 0, 100);
        sut.IsBetween(static x => x.Signal, 0, 100);
    }

    [TestMethod]
    public void SlowRsi()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> sut =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        sut.Should().HaveCount(502);
        Assert.HasCount(473, sut.Where(static x => x.StochRsi != null));
        Assert.HasCount(471, sut.Where(static x => x.Signal != null));

        // sample values
        StochRsiResult r1 = sut[31];
        r1.StochRsi.Should().BeApproximately(97.7778, Money4);
        r1.Signal.Should().BeApproximately(99.2593, Money4);

        StochRsiResult r2 = sut[152];
        r2.StochRsi.Should().Be(0);
        r2.Signal.Should().BeApproximately(20.0263, Money4);

        StochRsiResult r3 = sut[249];
        r3.StochRsi.Should().BeApproximately(27.3094, Money4);
        r3.Signal.Should().BeApproximately(33.2716, Money4);

        StochRsiResult r4 = sut[501];
        r4.StochRsi.Should().BeApproximately(89.8385, Money4);
        r4.Signal.Should().BeApproximately(73.4176, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToStochRsi(14, 14, 3);

        sut.Should().HaveCount(502);
        Assert.HasCount(475, sut.Where(static x => x.StochRsi != null));
        sut.Where(static x => x.StochRsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes
            .ToSma(2)
            .ToStochRsi(14, 14, 3);

        sut.Should().HaveCount(502);
        Assert.HasCount(474, sut.Where(static x => x.StochRsi != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToStochRsi(14, 14, 3, 3)
            .ToSma(10);

        sut.Should().HaveCount(502);
        Assert.HasCount(464, sut.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<StochRsiResult> r = BadQuotes
            .ToStochRsi(15, 20, 3, 2);

        r.Should().HaveCount(502);
        r.Where(static x => x.StochRsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<StochRsiResult> r0 = Noquotes
            .ToStochRsi(10, 20, 3);

        r0.Should().BeEmpty();

        IReadOnlyList<StochRsiResult> r1 = Onequote
            .ToStochRsi(8, 13, 2);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> sut = Quotes
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .RemoveWarmupPeriods();

        // assertions
        const int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
        sut.Should().HaveCount(502 - removeQty);

        StochRsiResult last = sut[^1];
        last.StochRsi.Should().BeApproximately(89.8385, Money4);
        last.Signal.Should().BeApproximately(73.4176, Money4);
    }

    [TestMethod]
    public void AutoHealing_HandlesRsiWarmupPeriodsCorrectly()
    {
        // Regression test for T204: Verify that StochRsi auto-healing works correctly
        // without explicit Remove() call on RSI results. CalcStoch handles NaN values
        // gracefully, making Remove() unnecessary and avoiding extra list allocation.
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;

        // Get results using current auto-healing implementation
        IReadOnlyList<StochRsiResult> results =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // Verify correct behavior
        results.Should().HaveCount(502);
        Assert.HasCount(475, results.Where(static x => x.StochRsi != null));
        Assert.HasCount(473, results.Where(static x => x.Signal != null));

        // Verify specific values match expected
        StochRsiResult r1 = results[31];
        r1.StochRsi.Should().BeApproximately(93.3333, Money4);
        r1.Signal.Should().BeApproximately(97.7778, Money4);

        StochRsiResult r2 = results[501];
        r2.StochRsi.Should().BeApproximately(97.5244, Money4);
        r2.Signal.Should().BeApproximately(89.8385, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(0, 14, 3));

        // bad STO period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 0, 3, 3));

        // bad STO signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 14, 0));

        // bad STO smoothing period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 14, 3, 0));
    }
}
