namespace StaticSeries;

[TestClass]
public class Mama : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const double fastLimit = 0.5;
        const double slowLimit = 0.05;

        IReadOnlyList<MamaResult> sut = Quotes
            .ToMama(fastLimit, slowLimit);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Mama != null).Should().HaveCount(497);

        // sample values
        MamaResult r1 = sut[4];
        r1.Mama.Should().BeNull();
        r1.Fama.Should().BeNull();

        MamaResult r2 = sut[5];
        r2.Mama.Should().Be(213.73);
        r2.Fama.Should().Be(213.73);

        MamaResult r3 = sut[6];
        r3.Mama.Should().BeApproximately(213.7850, Money4);
        r3.Fama.Should().BeApproximately(213.7438, Money3); // Money3 needed due to floating point precision

        MamaResult r4 = sut[25];
        r4.Mama.Should().BeApproximately(215.9524, Money4);
        r4.Fama.Should().BeApproximately(215.1407, Money4);

        MamaResult r5 = sut[149];
        r5.Mama.Should().BeApproximately(235.6593, Money4);
        r5.Fama.Should().BeApproximately(234.3660, Money4);

        MamaResult r6 = sut[249];
        r6.Mama.Should().BeApproximately(256.8026, Money4);
        r6.Fama.Should().BeApproximately(254.0605, Money4);

        MamaResult r7 = sut[501];
        r7.Mama.Should().BeApproximately(244.1092, Money4);
        r7.Fama.Should().BeApproximately(252.6139, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<MamaResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToMama();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Mama != null).Should().HaveCount(497);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<MamaResult> sut = Quotes
            .ToSma(2)
            .ToMama();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Mama != null).Should().HaveCount(496);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToMama()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(488);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<MamaResult> r = BadQuotes
            .ToMama();

        r.Should().HaveCount(502);
        r.Where(static x => x.Mama is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<MamaResult> r0 = Noquotes.ToMama();

        r0.Should().BeEmpty();

        IReadOnlyList<MamaResult> r1 = Onequote.ToMama();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const double fastLimit = 0.5;
        const double slowLimit = 0.05;

        IReadOnlyList<MamaResult> sut = Quotes
            .ToMama(fastLimit, slowLimit)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 50);

        MamaResult last = sut[^1];
        last.Mama.Should().BeApproximately(244.1092, Money4);
        last.Fama.Should().BeApproximately(252.6139, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period (same as slow period)
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMama(0.5, 0.5));

        // bad fast period (cannot be 1 or more)
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMama(1, 0.5));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToMama(0.5, 0));
    }
}
