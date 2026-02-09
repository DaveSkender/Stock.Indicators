namespace StaticSeries;

[TestClass]
public class Smi : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<SmiResult> sut = Quotes
            .ToSmi(14, 20, 5);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Smi != null).Should().HaveCount(489);
        sut.Where(static x => x.Signal != null).Should().HaveCount(489);

        // sample values
        SmiResult r12 = sut[12];
        r12.Smi.Should().BeNull();
        r12.Signal.Should().BeNull();

        SmiResult r13 = sut[13];
        r13.Smi.Should().BeApproximately(17.2603, Money4);
        r13.Signal.Should().BeApproximately(17.2603, Money4);

        SmiResult r14 = sut[14];
        r14.Smi.Should().BeApproximately(18.6086, Money4);
        r14.Signal.Should().BeApproximately(17.9344, Money4);

        SmiResult r28 = sut[28];
        r28.Smi.Should().BeApproximately(51.0417, Money4);
        r28.Signal.Should().BeApproximately(47.1207, Money4);

        SmiResult r150 = sut[150];
        r150.Smi.Should().BeApproximately(65.6692, Money4);
        r150.Signal.Should().BeApproximately(66.3292, Money4);

        SmiResult r250 = sut[250];  // also testing aliases here
        r250.Smi.Should().BeApproximately(67.2534, Money4);
        r250.Signal.Should().BeApproximately(67.6261, Money4);

        SmiResult r501 = sut[501];
        r501.Smi.Should().BeApproximately(-52.6560, Money4);
        r501.Signal.Should().BeApproximately(-54.1903, Money4);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToSmi(14, 20, 5)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(480);
    }

    [TestMethod]
    public void NoSignal()
    {
        IReadOnlyList<SmiResult> sut = Quotes
            .ToSmi(5, 20, 20, 1);

        // signal equals oscillator
        SmiResult r1 = sut[487];
        r1.Signal.Should().Be(r1.Smi);

        SmiResult r2 = sut[501];
        r2.Signal.Should().Be(r2.Smi);
    }

    [TestMethod]
    public void SmallPeriods()
    {
        IReadOnlyList<SmiResult> sut = Quotes
            .ToSmi(1, 1, 1, 5);

        // sample values
        SmiResult r51 = sut[51];
        r51.Smi.Should().BeApproximately(-100, Money4);
        r51.Signal.Should().BeApproximately(-20.8709, Money4);

        SmiResult r81 = sut[81];
        r81.Smi.Should().BeApproximately(0, Money4);
        r81.Signal.Should().BeApproximately(-14.7101, Money4);

        SmiResult r88 = sut[88];
        r88.Smi.Should().BeApproximately(100, Money4);
        r88.Signal.Should().BeApproximately(47.2291, Money4);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<SmiResult> r = BadQuotes
            .ToSmi(5, 5, 1, 5);

        r.Should().HaveCount(502);
        r.Where(static x => x.Smi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<SmiResult> r0 = Noquotes
            .ToSmi(5, 5);

        r0.Should().BeEmpty();

        IReadOnlyList<SmiResult> r1 = Onequote
            .ToSmi(5, 3, 3);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmiResult> sut = Quotes
            .ToSmi(14, 20, 5)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(501 - (14 + 100));

        SmiResult last = sut[^1];
        last.Smi.Should().BeApproximately(-52.6560, Money4);
        last.Signal.Should().BeApproximately(-54.1903, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSmi(0, 5, 5, 5));

        // bad first smooth period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSmi(14, 0, 5, 5));

        // bad second smooth period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSmi(14, 3, 0, 5));

        // bad signal
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToSmi(9, 3, 1, 0));
    }
}
