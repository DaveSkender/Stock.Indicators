namespace StaticSeries;

[TestClass]
public class Correlation : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CorrResult> sut = Quotes
            .ToCorrelation(OtherQuotes, 20);

        // proper quantities
        // should always be the same number of sut as there is quotes
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Correlation != null).Should().HaveCount(483);

        // sample values
        CorrResult r18 = sut[18];
        r18.Correlation.Should().BeNull();
        r18.RSquared.Should().BeNull();

        CorrResult r19 = sut[19];
        r19.Correlation.Should().BeApproximately(0.6933, Money4);
        r19.RSquared.Should().BeApproximately(0.4806, Money4);

        CorrResult r257 = sut[257];
        r257.Correlation.Should().BeApproximately(-0.1347, Money4);
        r257.RSquared.Should().BeApproximately(0.0181, Money4);

        CorrResult r501 = sut[501];
        r501.Correlation.Should().BeApproximately(0.8460, Money4);
        r501.RSquared.Should().BeApproximately(0.7157, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<CorrResult> sut = Quotes.ToCorrelation(OtherQuotes, 20);
        sut.IsBetween(static x => x.Correlation, -1, 1);
        sut.IsBetween(static x => x.RSquared, 0, 1);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<CorrResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToCorrelation(OtherQuotes.Use(CandlePart.Close), 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Correlation != null).Should().HaveCount(483);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToCorrelation(OtherQuotes, 20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(474);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<CorrResult> sut = Quotes
            .ToSma(2)
            .ToCorrelation(OtherQuotes.ToSma(2), 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Correlation != null).Should().HaveCount(482);
        sut.Where(static x => x.Correlation is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CorrResult> r = BadQuotes
            .ToCorrelation(BadQuotes, 15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Correlation is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<CorrResult> r = BigQuotes
            .ToCorrelation(BigQuotes, 150);

        r.Should().HaveCount(1246);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CorrResult> r0 = Noquotes
            .ToCorrelation(Noquotes, 10);

        r0.Should().BeEmpty();

        IReadOnlyList<CorrResult> r1 = Onequote
            .ToCorrelation(Onequote, 10);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CorrResult> sut = Quotes
            .ToCorrelation(OtherQuotes, 20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 19);

        CorrResult last = sut[^1];
        last.Correlation.Should().BeApproximately(0.8460, Money4);
        last.RSquared.Should().BeApproximately(0.7157, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToCorrelation(OtherQuotes, 0));

        // bad eval quotes
        IReadOnlyList<Quote> eval = Data.GetCompare(300);
        Assert.ThrowsExactly<InvalidQuotesException>(
            () => Quotes.ToCorrelation(eval, 30));

        // mismatched quotes
        Assert.ThrowsExactly<InvalidQuotesException>(
            () => MismatchQuotes.ToCorrelation(OtherQuotes, 20));
    }
}
