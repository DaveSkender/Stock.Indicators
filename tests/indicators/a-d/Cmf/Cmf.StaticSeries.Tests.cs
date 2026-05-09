namespace StaticSeries;

[TestClass]
public class Cmf : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CmfResult> sut = Quotes
            .ToCmf();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Cmf != null).Should().HaveCount(483);

        // sample values
        CmfResult r1 = sut[49];
        r1.MoneyFlowMultiplier.Should().BeApproximately(0.5468, Money4);
        r1.MoneyFlowVolume.Should().BeApproximately(55609259, 0.005);
        r1.Cmf.Should().BeApproximately(0.350596, Money6);

        CmfResult r2 = sut[249];
        r2.MoneyFlowMultiplier.Should().BeApproximately(0.7778, Money4);
        r2.MoneyFlowVolume.Should().BeApproximately(36433792.89, 0.005);
        r2.Cmf.Should().BeApproximately(-0.040226, Money6);

        CmfResult r3 = sut[501];
        r3.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        r3.MoneyFlowVolume.Should().BeApproximately(118396116.25, 0.005);
        r3.Cmf.Should().BeApproximately(-0.123754, Money6);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<CmfResult> sut = Quotes.ToCmf(20);
        sut.IsBetween(static x => x.Cmf, -1, 1);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToCmf()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(474);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CmfResult> r = BadQuotes
            .ToCmf(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Cmf is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<CmfResult> r = BigQuotes
            .ToCmf(150);

        r.Should().HaveCount(1246);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CmfResult> r0 = Noquotes
            .ToCmf();

        r0.Should().BeEmpty();

        IReadOnlyList<CmfResult> r1 = Onequote
            .ToCmf();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CmfResult> sut = Quotes
            .ToCmf()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 19);

        CmfResult last = sut[^1];
        last.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        last.MoneyFlowVolume.Should().BeApproximately(118396116.25, 0.005);
        last.Cmf.Should().BeApproximately(-0.123754, Money6);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToCmf(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
