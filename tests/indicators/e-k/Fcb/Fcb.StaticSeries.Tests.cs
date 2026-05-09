namespace StaticSeries;

[TestClass]
public class Fcb : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<FcbResult> sut = Quotes
            .ToFcb();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.UpperBand != null).Should().HaveCount(497);
        sut.Where(static x => x.LowerBand != null).Should().HaveCount(493);

        // sample values
        FcbResult r1 = sut[4];
        r1.UpperBand.Should().BeNull();
        r1.LowerBand.Should().BeNull();

        FcbResult r2 = sut[10];
        r2.UpperBand.Should().Be(214.84m);
        r2.LowerBand.Should().Be(212.53m);

        FcbResult r3 = sut[120];
        r3.UpperBand.Should().Be(233.35m);
        r3.LowerBand.Should().Be(231.14m);

        FcbResult r4 = sut[180];
        r4.UpperBand.Should().Be(236.78m);
        r4.LowerBand.Should().Be(233.56m);

        FcbResult r5 = sut[250];
        r5.UpperBand.Should().Be(258.70m);
        r5.LowerBand.Should().Be(257.04m);

        FcbResult r6 = sut[501];
        r6.UpperBand.Should().Be(262.47m);
        r6.LowerBand.Should().Be(229.42m);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<FcbResult> r = BadQuotes
            .ToFcb();

        r.Should().HaveCount(502);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<FcbResult> r0 = Noquotes
            .ToFcb();

        r0.Should().BeEmpty();

        IReadOnlyList<FcbResult> r1 = Onequote
            .ToFcb();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<FcbResult> sut = Quotes
            .ToFcb()
            .Condense();

        // assertions
        sut.Should().HaveCount(502 - 5);

        FcbResult last = sut[^1];
        last.UpperBand.Should().Be(262.47m);
        last.LowerBand.Should().Be(229.42m);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<FcbResult> sut = Quotes
            .ToFcb()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 5);

        FcbResult last = sut[^1];
        last.UpperBand.Should().Be(262.47m);
        last.LowerBand.Should().Be(229.42m);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToFcb(1))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
