namespace StaticSeries;

[TestClass]
public class Atr : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<AtrResult> sut = Quotes
            .ToAtr();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Atr != null).Should().HaveCount(488);

        // sample values
        AtrResult r13 = sut[13];
        r13.Tr.Should().BeApproximately(1.45, Money8);
        r13.Atr.Should().BeNull();
        r13.Atrp.Should().BeNull();

        AtrResult r14 = sut[14];
        r14.Tr.Should().BeApproximately(1.82, Money8);
        r14.Atr.Should().BeApproximately(1.3364, Money4);
        r14.Atrp.Should().BeApproximately(0.6215, Money4);

        AtrResult r24 = sut[24];
        r24.Tr.Should().BeApproximately(0.88, Money8);
        r24.Atr.Should().BeApproximately(1.3034, Money4);
        r24.Atrp.Should().BeApproximately(0.6026, Money4);

        AtrResult r249 = sut[249];
        r249.Tr.Should().BeApproximately(0.58, Money8);
        r249.Atr.Should().BeApproximately(1.3381, Money4);
        r249.Atrp.Should().BeApproximately(0.5187, Money4);

        AtrResult r501 = sut[501];
        r501.Tr.Should().BeApproximately(2.67, Money8);
        r501.Atr.Should().BeApproximately(6.1497, Money4);
        r501.Atrp.Should().BeApproximately(2.5072, Money4);
    }

    [TestMethod]
    public void MatchingTrueRange()
    {
        IReadOnlyList<AtrResult> resultsAtr = Quotes
            .ToAtr(14);

        IReadOnlyList<TrResult> resultsTr = Quotes
            .ToTr();

        for (int i = 0; i < Quotes.Count; i++)
        {
            Quote q = Quotes[i];
            TrResult t = resultsTr[i];
            AtrResult r = resultsAtr[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(t.Timestamp);
            r.Tr.Should().Be(t.Tr);
        }
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToAtr(10)
            .ToSma(10);

        sut.Should().HaveCount(502);
        Assert.AreEqual(502 - 19, sut.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<AtrResult> r = BadQuotes
            .ToAtr(20);

        r.Should().HaveCount(502);
        r.Where(static x => x.Atr is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<AtrResult> r0 = Noquotes
            .ToAtr();

        r0.Should().BeEmpty();

        IReadOnlyList<AtrResult> r1 = Onequote
            .ToAtr();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AtrResult> sut = Quotes
            .ToAtr()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 14);

        AtrResult last = sut[^1];
        last.Tr.Should().BeApproximately(2.67, Money8);
        last.Atr.Should().BeApproximately(6.1497, Money4);
        last.Atrp.Should().BeApproximately(2.5072, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToAtr(1))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
