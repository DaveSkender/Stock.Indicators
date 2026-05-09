namespace StaticSeries;

[TestClass]
public class Kama : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int erPeriods = 10;
        const int fastPeriods = 2;
        const int slowPeriods = 30;

        IReadOnlyList<KamaResult> sut = Quotes
            .ToKama(erPeriods, fastPeriods, slowPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Er != null).Should().HaveCount(492);
        sut.Where(static x => x.Kama != null).Should().HaveCount(493);

        // sample values
        KamaResult r1 = sut[8];
        r1.Er.Should().BeNull();
        r1.Kama.Should().BeNull();

        KamaResult r2 = sut[9];
        r2.Er.Should().BeNull();
        r2.Kama.Should().BeApproximately(213.7500, Money4);

        KamaResult r3 = sut[10];
        r3.Er.Should().BeApproximately(0.2465, Money4);
        r3.Kama.Should().BeApproximately(213.7713, Money4);

        KamaResult r4 = sut[24];
        r4.Er.Should().BeApproximately(0.2136, Money4);
        r4.Kama.Should().BeApproximately(214.7423, Money4);

        KamaResult r5 = sut[149];
        r5.Er.Should().BeApproximately(0.3165, Money4);
        r5.Kama.Should().BeApproximately(235.5510, Money4);

        KamaResult r6 = sut[249];
        r6.Er.Should().BeApproximately(0.3182, Money4);
        r6.Kama.Should().BeApproximately(256.0898, Money4);

        KamaResult r7 = sut[501];
        r7.Er.Should().BeApproximately(0.2214, Money4);
        r7.Kama.Should().BeApproximately(240.1138, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<KamaResult> sut = Quotes.ToKama(10, 2, 30);
        sut.IsBetween(static x => x.Er, 0, 1);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<KamaResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToKama();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Kama != null).Should().HaveCount(493);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<KamaResult> sut = Quotes
            .ToSma(2)
            .ToKama();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Kama != null).Should().HaveCount(492);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToKama()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(484);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<KamaResult> r = BadQuotes
            .ToKama();

        r.Should().HaveCount(502);
        r.Where(static x => x.Kama is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<KamaResult> r0 = Noquotes
            .ToKama();

        r0.Should().BeEmpty();

        IReadOnlyList<KamaResult> r1 = Onequote
            .ToKama();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const int erPeriods = 10;
        const int fastPeriods = 2;
        const int slowPeriods = 30;

        IReadOnlyList<KamaResult> sut = Quotes
            .ToKama(erPeriods, fastPeriods, slowPeriods)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - Math.Max(erPeriods + 100, erPeriods * 10), sut);

        KamaResult last = sut[^1];
        last.Er.Should().BeApproximately(0.2214, Money4);
        last.Kama.Should().BeApproximately(240.1138, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad ER period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKama(0));

        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKama(10, 0));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKama(10, 5, 5));
    }
}
