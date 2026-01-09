namespace StaticSeries;

[TestClass]
public class Cmo : StaticSeriesTestBase
{
    // TODO: test for CMO isUp works as expected
    // when there’s no price change

    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CmoResult> sut = Quotes
            .ToCmo(14);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Cmo != null).Should().HaveCount(488);

        // sample values
        CmoResult r13 = sut[13];
        r13.Cmo.Should().BeNull();

        CmoResult r14 = sut[14];
        r14.Cmo.Should().BeApproximately(24.1081, Money4);

        CmoResult r249 = sut[249];
        r249.Cmo.Should().BeApproximately(48.9614, Money4);

        CmoResult r501 = sut[501];
        r501.Cmo.Should().BeApproximately(-26.7502, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<CmoResult> sut = Quotes.ToCmo(14);
        sut.IsBetween(static x => x.Cmo, -100, 100);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<CmoResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToCmo(14);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Cmo != null).Should().HaveCount(488);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<CmoResult> sut = Quotes
            .ToSma(2)
            .ToCmo(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Cmo != null).Should().HaveCount(481);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToCmo(20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(473);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CmoResult> r = BadQuotes
            .ToCmo(35);

        r.Should().HaveCount(502);
        r.Where(static x => x.Cmo is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CmoResult> r0 = Noquotes
            .ToCmo(5);

        r0.Should().BeEmpty();

        IReadOnlyList<CmoResult> r1 = Onequote
            .ToCmo(5);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CmoResult> sut = Quotes
            .ToCmo(14)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(488);

        CmoResult last = sut[^1];
        last.Cmo.Should().BeApproximately(-26.7502, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToCmo(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
