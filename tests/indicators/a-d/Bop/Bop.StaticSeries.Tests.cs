namespace StaticSeries;

[TestClass]
public class Bop : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<BopResult> sut = Quotes
            .ToBop();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Bop != null).Should().HaveCount(489);

        // sample values
        BopResult r1 = sut[12];
        r1.Bop.Should().BeNull();

        BopResult r2 = sut[13];
        r2.Bop.Should().BeApproximately(0.081822, Money6);

        BopResult r3 = sut[149];
        r3.Bop.Should().BeApproximately(-0.016203, Money6);

        BopResult r4 = sut[249];
        r4.Bop.Should().BeApproximately(-0.058682, Money6);

        BopResult r5 = sut[501];
        r5.Bop.Should().BeApproximately(-0.292788, Money6);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<BopResult> sut = Quotes.ToBop(14);
        sut.IsBetween(static x => x.Bop, -1, 1);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToBop()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(480);
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<BopResult> r = Data.GetBtcUsdNan()
            .ToBop(50);

        r.Where(static x => x.Bop is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<BopResult> r = BadQuotes
            .ToBop();

        r.Should().HaveCount(502);
        r.Where(static x => x.Bop is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<BopResult> r0 = Noquotes
            .ToBop();
        r0.Should().BeEmpty();

        IReadOnlyList<BopResult> r1 = Onequote
            .ToBop();
        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<BopResult> sut = Quotes
            .ToBop()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 13);

        BopResult last = sut[^1];
        last.Bop.Should().BeApproximately(-0.292788, Money6);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToBop(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
