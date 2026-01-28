namespace StaticSeries;

[TestClass]
public class T3 : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<T3Result> sut = Quotes
            .ToT3();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.T3 != null).Should().HaveCount(502);

        // sample values
        T3Result r5 = sut[5];
        r5.T3.Should().BeApproximately(213.9654, Money4);

        T3Result r24 = sut[24];
        r24.T3.Should().BeApproximately(215.9481, Money4);

        T3Result r44 = sut[44];
        r44.T3.Should().BeApproximately(224.9412, Money4);

        T3Result r149 = sut[149];
        r149.T3.Should().BeApproximately(235.8851, Money4);

        T3Result r249 = sut[249];
        r249.T3.Should().BeApproximately(257.8735, Money4);

        T3Result r501 = sut[501];
        r501.T3.Should().BeApproximately(238.9308, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<T3Result> sut = Quotes
            .Use(CandlePart.Close)
            .ToT3();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.T3 != null).Should().HaveCount(502);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<T3Result> sut = Quotes
            .ToSma(2)
            .ToT3();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.T3 != null).Should().HaveCount(501);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToT3()
            .ToSma(10);

        sut.Should().HaveCount(502);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<T3Result> r = BadQuotes
            .ToT3();

        r.Should().HaveCount(502);
        r.Where(static x => x.T3 is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<T3Result> r0 = Noquotes
            .ToT3();

        r0.Should().BeEmpty();

        IReadOnlyList<T3Result> r1 = Onequote
            .ToT3();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToT3(0));

        // bad volume factor
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToT3(25, 0));
    }
}
