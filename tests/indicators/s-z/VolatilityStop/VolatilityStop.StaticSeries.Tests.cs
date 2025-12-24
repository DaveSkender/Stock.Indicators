namespace StaticSeries;

[TestClass]
public class VolatilityStop : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<VolatilityStopResult> sut =
            Quotes.ToVolatilityStop(14);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sar != null).Should().HaveCount(448);

        // sample values
        VolatilityStopResult r53 = sut[53];
        r53.Sar.Should().BeNull();
        r53.IsStop.Should().BeNull();
        r53.LowerBand.Should().BeNull();
        r53.UpperBand.Should().BeNull();

        VolatilityStopResult r54 = sut[54];
        r54.Sar.Should().BeApproximately(226.2118, Money4);
        Assert.IsFalse(r54.IsStop);
        r54.UpperBand.Should().BeApproximately(226.2118, Money4);
        r54.LowerBand.Should().BeNull();

        VolatilityStopResult r55 = sut[55];
        r55.Sar.Should().BeApproximately(226.2124, Money4);
        Assert.IsFalse(r55.IsStop);
        r55.UpperBand.Should().BeApproximately(226.2124, Money4);
        r55.LowerBand.Should().BeNull();

        VolatilityStopResult r168 = sut[168];
        Assert.IsTrue(r168.IsStop);

        VolatilityStopResult r282 = sut[282];
        r282.Sar.Should().BeApproximately(261.8687, Money4);
        Assert.IsTrue(r282.IsStop);
        r282.UpperBand.Should().BeApproximately(261.8687, Money4);
        r282.LowerBand.Should().BeNull();

        VolatilityStopResult r283 = sut[283];
        r283.Sar.Should().BeApproximately(249.3219, Money4);
        Assert.IsFalse(r283.IsStop);
        r283.LowerBand.Should().BeApproximately(249.3219, Money4);
        r283.UpperBand.Should().BeNull();

        VolatilityStopResult r284 = sut[284];
        r284.Sar.Should().BeApproximately(249.7460, Money4);
        Assert.IsFalse(r284.IsStop);
        r284.LowerBand.Should().BeApproximately(249.7460, Money4);
        r284.UpperBand.Should().BeNull();

        VolatilityStopResult last = sut[^1];
        last.Sar.Should().BeApproximately(249.2423, Money4);
        Assert.IsFalse(last.IsStop);
        last.UpperBand.Should().BeApproximately(249.2423, Money4);
        last.LowerBand.Should().BeNull();
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToVolatilityStop()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(439);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<VolatilityStopResult> r = BadQuotes
            .ToVolatilityStop();

        r.Should().HaveCount(502);
        Assert.IsEmpty(r.Where(static x => x.Sar is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<VolatilityStopResult> r0 = Noquotes
            .ToVolatilityStop();

        r0.Should().BeEmpty();

        IReadOnlyList<VolatilityStopResult> r1 = Onequote
            .ToVolatilityStop();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<VolatilityStopResult> sut = Quotes
            .ToVolatilityStop(14)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(402);

        VolatilityStopResult last = sut[^1];
        last.Sar.Should().BeApproximately(249.2423, Money4);
        Assert.IsFalse(last.IsStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToVolatilityStop(1));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToVolatilityStop(20, 0));
    }
}
