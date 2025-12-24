namespace StaticSeries;

[TestClass]
public class Prs : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int lookbackPeriods = 30;

        IReadOnlyList<PrsResult> sut = OtherQuotes
            .ToPrs(Quotes, lookbackPeriods);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Prs != null).Should().HaveCount(502);

        // sample values
        PrsResult r1 = sut[8];
        r1.Prs.Should().BeApproximately(1.108340, Money6);
        r1.PrsPercent.Should().BeNull();

        PrsResult r2 = sut[249];
        r2.Prs.Should().BeApproximately(1.222373, Money6);
        r2.PrsPercent.Should().BeApproximately(-0.023089, Money6);

        PrsResult r3 = sut[501];
        r3.Prs.Should().BeApproximately(1.356817, Money6);
        r3.PrsPercent.Should().BeApproximately(0.037082, Money6);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<PrsResult> sut = OtherQuotes
            .Use(CandlePart.Close)
            .ToPrs(Quotes.Use(CandlePart.Close), 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Prs != null).Should().HaveCount(502);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = OtherQuotes
            .ToPrs(Quotes, 20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(493);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<PrsResult> sut = Quotes
            .ToSma(2)
            .ToPrs(OtherQuotes.ToSma(2), 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Prs != null).Should().HaveCount(501);
        sut.Where(static x => x.Prs is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<PrsResult> r = BadQuotes
            .ToPrs(BadQuotes, 15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Prs is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<PrsResult> r0 = Noquotes
            .ToPrs(Noquotes);

        r0.Should().BeEmpty();

        IReadOnlyList<PrsResult> r1 = Onequote
            .ToPrs(Onequote);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => OtherQuotes.ToPrs(Quotes, 0));

        // insufficient quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => Data.GetCompare(13).ToPrs(Quotes, 14));

        // insufficient eval quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => Data.GetCompare(300).ToPrs(Quotes, 14));

        // mismatch quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => OtherQuotes.ToPrs(MismatchQuotes, 14));
    }
}
