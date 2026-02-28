namespace StaticSeries;

[TestClass]
public class Hurst : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<HurstResult> sut = LongestQuotes
            .ToHurst(LongestQuotes.Count - 1);

        // assertions

        // proper quantities
        sut.Should().HaveCount(15821);
        sut.Where(static x => x.HurstExponent != null).Should().HaveCount(1);

        // sample value
        HurstResult r15820 = sut[15820];
        r15820.HurstExponent.Should().BeApproximately(0.483563, Money6);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<HurstResult> sut = Quotes.ToHurst(100);
        sut.IsBetween(static x => x.HurstExponent, 0, 1);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<HurstResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToHurst();

        sut.Should().HaveCount(502);
        Assert.AreEqual(402, sut.Count(static x => x.HurstExponent != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToHurst()
            .ToSma(10);

        sut.Should().HaveCount(502);
        Assert.AreEqual(393, sut.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<HurstResult> sut = Quotes
            .ToSma(10)
            .ToHurst();

        sut.Should().HaveCount(502);
        Assert.AreEqual(393, sut.Count(static x => x.HurstExponent != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<HurstResult> r = BadQuotes
            .ToHurst(150);

        r.Should().HaveCount(502);
        r.Where(static x => x.HurstExponent is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<HurstResult> r0 = Noquotes
            .ToHurst();

        r0.Should().BeEmpty();

        IReadOnlyList<HurstResult> r1 = Onequote
            .ToHurst();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<HurstResult> sut = LongestQuotes.ToHurst(LongestQuotes.Count - 1)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(1);

        HurstResult last = sut[^1];
        last.HurstExponent.Should().BeApproximately(0.483563, Money6);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToHurst(19))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
