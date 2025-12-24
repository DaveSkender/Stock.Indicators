namespace StaticSeries;

[TestClass]
public class Awesome : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<AwesomeResult> sut = Quotes
            .ToAwesome();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Oscillator != null).Should().HaveCount(469);

        // sample values
        AwesomeResult r1 = sut[32];
        r1.Oscillator.Should().BeNull();
        r1.Normalized.Should().BeNull();

        AwesomeResult r2 = sut[33];
        r2.Oscillator.Should().BeApproximately(5.4756, Money4);
        r2.Normalized.Should().BeApproximately(2.4548, Money4);

        AwesomeResult r3 = sut[249];
        r3.Oscillator.Should().BeApproximately(5.0618, Money4);
        r3.Normalized.Should().BeApproximately(1.9634, Money4);

        AwesomeResult r4 = sut[501];
        r4.Oscillator.Should().BeApproximately(-17.7692, Money4);
        r4.Normalized.Should().BeApproximately(-7.2763, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<AwesomeResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToAwesome();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Oscillator != null).Should().HaveCount(469);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<AwesomeResult> sut = Quotes
            .ToSma(2)
            .ToAwesome();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Oscillator != null).Should().HaveCount(468);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToAwesome()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(460);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<AwesomeResult> r = BadQuotes
            .ToAwesome();

        r.Should().HaveCount(502);
        r.Where(static x => x.Oscillator is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<AwesomeResult> r0 = Noquotes
            .ToAwesome();

        r0.Should().BeEmpty();

        IReadOnlyList<AwesomeResult> r1 = Onequote
            .ToAwesome();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AwesomeResult> sut = Quotes
            .ToAwesome()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 33);

        AwesomeResult last = sut[^1];
        last.Oscillator.Should().BeApproximately(-17.7692, Money4);
        last.Normalized.Should().BeApproximately(-7.2763, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAwesome(0, 34));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAwesome(25, 25));
    }
}
