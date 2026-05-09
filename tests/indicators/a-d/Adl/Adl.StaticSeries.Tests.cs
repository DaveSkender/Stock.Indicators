namespace StaticSeries;

[TestClass]
public class Adl : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<AdlResult> sut = Quotes
            .ToAdl();

        // proper quantities
        sut.Should().HaveCount(502);

        // sample values
        AdlResult r1 = sut[249];
        r1.MoneyFlowMultiplier.Should().BeApproximately(0.7778, Money4);
        r1.MoneyFlowVolume.Should().BeApproximately(36433792.89, Money2);
        r1.Adl.Should().BeApproximately(3266400865.74, Money2);

        AdlResult r2 = sut[501];
        r2.MoneyFlowMultiplier.Should().BeApproximately(0.8052, Money4);
        r2.MoneyFlowVolume.Should().BeApproximately(118396116.25, Money2);
        r2.Adl.Should().BeApproximately(3439986548.42, Money2);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToAdl()
            .ToSma(10);

        // assertions

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(493);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<AdlResult> r = BadQuotes
            .ToAdl();

        r.Should().HaveCount(502);
        r.Where(static x => double.IsNaN(x.Adl)).Should().BeEmpty();
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<AdlResult> r = BigQuotes
            .ToAdl();

        r.Should().HaveCount(1246);
    }

    [TestMethod]
    public void RandomQuotes_WorksAsExpected()
    {
        IReadOnlyList<AdlResult> r = RandomQuotes
            .ToAdl();

        r.Should().HaveCount(1000);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<AdlResult> r0 = Noquotes
            .ToAdl();

        r0.Should().BeEmpty();

        IReadOnlyList<AdlResult> r1 = Onequote
            .ToAdl();

        r1.Should().HaveCount(1);
    }
}
