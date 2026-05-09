namespace StaticSeries;

[TestClass]
public class Kvo : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<KvoResult> sut =
            Quotes.ToKvo();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Oscillator != null).Should().HaveCount(446);
        sut.Where(static x => x.Signal != null).Should().HaveCount(434);

        // sample values
        KvoResult r55 = sut[55];
        r55.Oscillator.Should().BeNull();
        r55.Signal.Should().BeNull();

        KvoResult r56 = sut[56];
        (r56.Oscillator.Value).Should().BeApproximately(-2138454001, 0.5);
        r56.Signal.Should().BeNull();

        KvoResult r57 = sut[57];
        (r57.Oscillator.Value).Should().BeApproximately(-2265495450, 0.5);
        r57.Signal.Should().BeNull();

        KvoResult r68 = sut[68];
        (r68.Oscillator.Value).Should().BeApproximately(-1241548491, 0.5);
        (r68.Signal.Value).Should().BeApproximately(-1489659254, 0.5);

        KvoResult r149 = sut[149];
        (r149.Oscillator.Value).Should().BeApproximately(-62800843, 0.5);
        (r149.Signal.Value).Should().BeApproximately(-18678832, 0.5);

        KvoResult r249 = sut[249];
        (r249.Oscillator.Value).Should().BeApproximately(-51541005, 0.5);
        (r249.Signal.Value).Should().BeApproximately(135207969, 0.5);

        KvoResult r501 = sut[501];
        (r501.Oscillator.Value).Should().BeApproximately(-539224047, 0.5);
        (r501.Signal.Value).Should().BeApproximately(-1548306127, 0.5);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToKvo()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(437);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<KvoResult> r = BadQuotes
            .ToKvo();

        r.Should().HaveCount(502);
        r.Where(static x => x.Oscillator is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<KvoResult> r0 = Noquotes
            .ToKvo();

        r0.Should().BeEmpty();

        IReadOnlyList<KvoResult> r1 = Onequote
            .ToKvo();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<KvoResult> sut = Quotes
            .ToKvo()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (55 + 150));

        KvoResult last = sut[^1];
        (last.Oscillator.Value).Should().BeApproximately(-539224047, 0.5);
        (last.Signal.Value).Should().BeApproximately(-1548306127, 0.5);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKvo(2));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKvo(20, 20));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKvo(34, 55, 0));
    }
}
