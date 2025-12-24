namespace StaticSeries;

[TestClass]
public class McGinleyDynamic : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<DynamicResult> sut = Quotes
            .ToDynamic(14);

        // assertions
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Dynamic != null).Should().HaveCount(501);

        // sample values
        DynamicResult r1 = sut[1];
        r1.Dynamic.Should().BeApproximately(212.9465, Money4);

        DynamicResult r25 = sut[25];
        r25.Dynamic.Should().BeApproximately(215.4801, Money4);

        DynamicResult r250 = sut[250];
        r250.Dynamic.Should().BeApproximately(256.0554, Money4);

        DynamicResult r501 = sut[501];
        r501.Dynamic.Should().BeApproximately(245.7356, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<DynamicResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToDynamic(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Dynamic != null).Should().HaveCount(501);
        sut.Where(static x => x.Dynamic is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<DynamicResult> sut = Quotes
            .ToSma(10)
            .ToDynamic(14);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Dynamic != null).Should().HaveCount(492);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToDynamic(14)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(492);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<DynamicResult> r = BadQuotes
            .ToDynamic(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Dynamic is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<DynamicResult> r0 = Noquotes
            .ToDynamic(14);

        r0.Should().BeEmpty();

        IReadOnlyList<DynamicResult> r1 = Onequote
            .ToDynamic(14);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToDynamic(0));

        // bad k-factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToDynamic(14, 0));
    }
}
