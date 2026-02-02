namespace StaticSeries;

[TestClass]
public class Ultimate : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<UltimateResult> sut = Quotes
            .ToUltimate();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Ultimate != null).Should().HaveCount(474);

        // sample values
        UltimateResult r1 = sut[74];
        r1.Ultimate.Should().BeApproximately(51.7770, Money4);

        UltimateResult r2 = sut[249];
        r2.Ultimate.Should().BeApproximately(45.3121, Money4);

        UltimateResult r3 = sut[501];
        r3.Ultimate.Should().BeApproximately(49.5257, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<UltimateResult> sut = Quotes.ToUltimate(7, 14, 28);
        sut.IsBetween(static x => x.Ultimate, 0, 100);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToUltimate()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(465);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<UltimateResult> r = BadQuotes
            .ToUltimate(1, 2, 3);

        r.Should().HaveCount(502);
        r.Where(static x => x.Ultimate is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<UltimateResult> r0 = Noquotes
            .ToUltimate();

        r0.Should().BeEmpty();

        IReadOnlyList<UltimateResult> r1 = Onequote
            .ToUltimate();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UltimateResult> sut = Quotes
            .ToUltimate()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 28);

        UltimateResult last = sut[^1];
        last.Ultimate.Should().BeApproximately(49.5257, Money4);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad short period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUltimate(0));

        // bad middle period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUltimate(7, 6));

        // bad long period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUltimate(7, 14, 11));
    }
}
