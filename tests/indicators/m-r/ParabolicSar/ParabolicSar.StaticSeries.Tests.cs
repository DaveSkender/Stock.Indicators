namespace StaticSeries;

[TestClass]
public class ParabolicSar : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;

        List<ParabolicSarResult> sut =
            Quotes.ToParabolicSar(acclerationStep, maxAccelerationFactor)
                .ToList();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sar != null).Should().HaveCount(488);

        // sample values
        ParabolicSarResult r14 = sut[14];
        r14.Sar.Should().Be(212.83);
        Assert.IsTrue(r14.IsReversal);

        ParabolicSarResult r16 = sut[16];
        r16.Sar.Should().BeApproximately(212.9924, Money4);
        Assert.IsFalse(r16.IsReversal);

        ParabolicSarResult r94 = sut[94];
        r94.Sar.Should().BeApproximately(228.3600, Money4);
        Assert.IsFalse(r94.IsReversal);

        ParabolicSarResult r501 = sut[501];
        r501.Sar.Should().BeApproximately(229.7662, Money4);
        Assert.IsFalse(r501.IsReversal);
    }

    [TestMethod]
    public void Extended()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;
        const double initialStep = 0.01;

        List<ParabolicSarResult> sut =
            Quotes.ToParabolicSar(
                acclerationStep, maxAccelerationFactor, initialStep)
                .ToList();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sar != null).Should().HaveCount(488);

        // sample values
        ParabolicSarResult r14 = sut[14];
        r14.Sar.Should().Be(212.83);
        Assert.IsTrue(r14.IsReversal);

        ParabolicSarResult r16 = sut[16];
        r16.Sar.Should().BeApproximately(212.9518, Money4);
        Assert.IsFalse(r16.IsReversal);

        ParabolicSarResult r94 = sut[94];
        r94.Sar.Should().Be(228.36);
        Assert.IsFalse(r94.IsReversal);

        ParabolicSarResult r486 = sut[486];
        r486.Sar.Should().BeApproximately(273.4148, Money4);
        Assert.IsFalse(r486.IsReversal);

        ParabolicSarResult r501 = sut[501];
        r501.Sar.Should().Be(246.73);
        Assert.IsFalse(r501.IsReversal);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToParabolicSar()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(479);
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;

        List<Quote> insufficientQuotes = Data.GetDefault()
            .OrderBy(static x => x.Timestamp)
            .Take(10)
            .ToList();

        IReadOnlyList<ParabolicSarResult> sut =
            insufficientQuotes
                .ToParabolicSar(acclerationStep, maxAccelerationFactor);

        // assertions

        // proper quantities
        sut.Should().HaveCount(10);
        Assert.IsEmpty(sut.Where(static x => x.Sar != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ParabolicSarResult> r = BadQuotes
            .ToParabolicSar(0.2, 0.2, 0.2);

        r.Should().HaveCount(502);
        Assert.IsEmpty(r.Where(static x => x.Sar is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ParabolicSarResult> r0 = Noquotes
            .ToParabolicSar();

        r0.Should().BeEmpty();

        IReadOnlyList<ParabolicSarResult> r1 = Onequote
            .ToParabolicSar();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const double acclerationStep = 0.02;
        const double maxAccelerationFactor = 0.2;

        IReadOnlyList<ParabolicSarResult> sut = Quotes
            .ToParabolicSar(acclerationStep, maxAccelerationFactor)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(488);

        ParabolicSarResult last = sut[^1];
        last.Sar.Should().BeApproximately(229.7662, Money4);
        Assert.IsFalse(last.IsReversal);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad acceleration step
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(0, 1));

        // insufficient acceleration step
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(0.02, 0));

        // step larger than factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(6, 2));

        // insufficient initial factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToParabolicSar(0.02, 0.5, 0));
    }
}
