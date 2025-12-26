namespace StaticSeries;

[TestClass]
public class Ichimoku : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int tenkanPeriods = 9;
        const int kijunPeriods = 26;
        const int senkouBPeriods = 52;

        IReadOnlyList<IchimokuResult> sut = Quotes
            .ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.TenkanSen != null).Should().HaveCount(494);
        sut.Where(static x => x.KijunSen != null).Should().HaveCount(477);
        sut.Where(static x => x.SenkouSpanA != null).Should().HaveCount(451);
        sut.Where(static x => x.SenkouSpanB != null).Should().HaveCount(425);
        sut.Where(static x => x.ChikouSpan != null).Should().HaveCount(476);

        // sample values
        IchimokuResult r1 = sut[51];
        r1.TenkanSen.Should().Be(224.465m);
        r1.KijunSen.Should().Be(221.94m);
        r1.SenkouSpanA.Should().Be(214.8325m);
        r1.SenkouSpanB.Should().BeNull();
        r1.ChikouSpan.Should().Be(226.35m);

        IchimokuResult r2 = sut[249];
        r2.TenkanSen.Should().Be(257.15m);
        r2.KijunSen.Should().Be(253.085m);
        r2.SenkouSpanA.Should().Be(246.3125m);
        r2.SenkouSpanB.Should().Be(241.685m);
        r2.ChikouSpan.Should().Be(259.21m);

        IchimokuResult r3 = sut[475];
        r3.TenkanSen.Should().Be(265.575m);
        r3.KijunSen.Should().Be(263.965m);
        r3.SenkouSpanA.Should().Be(274.9475m);
        r3.SenkouSpanB.Should().Be(274.95m);
        r3.ChikouSpan.Should().Be(245.28m);

        IchimokuResult r4 = sut[501];
        r4.TenkanSen.Should().Be(241.26m);
        r4.KijunSen.Should().Be(251.505m);
        r4.SenkouSpanA.Should().Be(264.77m);
        r4.SenkouSpanB.Should().Be(269.82m);
        r4.ChikouSpan.Should().BeNull();
    }

    [TestMethod]
    public void Extended()
    {
        IReadOnlyList<IchimokuResult> sut = Quotes
            .ToIchimoku(3, 13, 40, 0, 0);

        sut.Should().HaveCount(502);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<IchimokuResult> r = BadQuotes
            .ToIchimoku();

        r.Should().HaveCount(502);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<IchimokuResult> r0 = Noquotes
            .ToIchimoku();

        r0.Should().BeEmpty();

        IReadOnlyList<IchimokuResult> r1 = Onequote
            .ToIchimoku();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<IchimokuResult> sut = Quotes
            .ToIchimoku()
            .Condense();

        sut.Should().HaveCount(502);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToIchimoku(0));

        // bad short span period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToIchimoku(9, 0));

        // bad long span period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToIchimoku(9, 26, 26));

        // invalid offsets
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToIchimoku(9, 26, 52, -1));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToIchimoku(9, 26, 52, -1, 12));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToIchimoku(9, 26, 52, 12, -1));
    }
}
