namespace StaticSeries;

[TestClass]
public class Rsi : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<RsiResult> sut = Quotes
            .ToRsi();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Rsi != null).Should().HaveCount(488);

        // sample values
        sut[13].Rsi.Should().BeNull();
        sut[14].Rsi.Should().BeApproximately(62.0541, Money4);
        sut[249].Rsi.Should().BeApproximately(70.9368, Money4);
        sut[501].Rsi.Should().BeApproximately(42.0773, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<RsiResult> sut = Quotes.ToRsi(14);
        sut.IsBetween(static x => x.Rsi, 0, 100);
    }

    [TestMethod]
    public void SmallLookback()
    {
        const int lookbackPeriods = 1;
        IReadOnlyList<RsiResult> sut = Quotes
            .ToRsi(lookbackPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Rsi != null).Should().HaveCount(501);

        // sample values
        sut[28].Rsi.Should().Be(100);
        sut[52].Rsi.Should().Be(0);
    }

    [TestMethod]
    public void CryptoData()
    {
        IReadOnlyList<Quote> btc = Data.GetBitcoin();

        IReadOnlyList<RsiResult> sut = btc
            .ToRsi(1);

        sut.Should().HaveCount(1246);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<RsiResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToRsi();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Rsi != null).Should().HaveCount(488);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<RsiResult> sut = Quotes
            .ToSma(2)
            .ToRsi();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Rsi != null).Should().HaveCount(487);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToRsi()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(479);
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<RsiResult> sut = Data.GetBtcUsdNan()
            .ToRsi();

        sut.Where(static x => x.Rsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<RsiResult> sut = BadQuotes
            .ToRsi(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Rsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<RsiResult> r0 = Noquotes
            .ToRsi();

        r0.Should().BeEmpty();

        IReadOnlyList<RsiResult> r1 = Onequote
            .ToRsi();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<RsiResult> sut = Quotes
            .ToRsi()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (10 * 14));
        sut[^1].Rsi.Should().BeApproximately(42.0773, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToRsi(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
