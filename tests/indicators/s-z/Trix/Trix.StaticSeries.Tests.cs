namespace StaticSeries;

[TestClass]
public class Trix : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<TrixResult> sut = Quotes
            .ToTrix(20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Ema3 != null).Should().HaveCount(482);
        sut.Where(static x => x.Trix != null).Should().HaveCount(482);

        // sample values
        TrixResult r24 = sut[24];
        r24.Ema3.Should().BeApproximately(214.5486, Money4);
        r24.Trix.Should().BeApproximately(0.005047, Money6);

        TrixResult r67 = sut[67];
        r67.Ema3.Should().BeApproximately(221.7837, Money4);
        r67.Trix.Should().BeApproximately(0.050030, Money6);

        TrixResult r249 = sut[249];
        r249.Ema3.Should().BeApproximately(249.4469, Money4);
        r249.Trix.Should().BeApproximately(0.121781, Money6);

        TrixResult r501 = sut[501];
        r501.Ema3.Should().BeApproximately(263.3216, Money4);
        r501.Trix.Should().BeApproximately(-0.230742, Money6);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<TrixResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToTrix(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Trix != null).Should().HaveCount(482);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<TrixResult> sut = Quotes
            .ToSma(2)
            .ToTrix(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Trix != null).Should().HaveCount(481);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToTrix(20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(473);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<TrixResult> r = BadQuotes
            .ToTrix(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Trix is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<TrixResult> r0 = Noquotes
            .ToTrix(5);

        r0.Should().BeEmpty();

        IReadOnlyList<TrixResult> r1 = Onequote
            .ToTrix(5);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<TrixResult> sut = Quotes
            .ToTrix(20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - ((3 * 20) + 100));

        TrixResult last = sut[^1];
        last.Ema3.Should().BeApproximately(263.3216, Money4);
        last.Trix.Should().BeApproximately(-0.230742, Money6);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToTrix(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
