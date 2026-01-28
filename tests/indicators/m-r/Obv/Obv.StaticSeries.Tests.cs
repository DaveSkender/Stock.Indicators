namespace StaticSeries;

[TestClass]
public class Obv : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<ObvResult> sut = Quotes
            .ToObv();

        // proper quantities
        sut.Should().HaveCount(502);

        // sample values
        ObvResult r1 = sut[249];
        r1.Obv.Should().Be(1780918888);

        ObvResult r2 = sut[501];
        r2.Obv.Should().Be(539843504);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToObv()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(493);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<ObvResult> r = BadQuotes
            .ToObv();

        r.Should().HaveCount(502);
        r.Where(static x => double.IsNaN(x.Obv)).Should().BeEmpty();
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<ObvResult> r = BigQuotes
            .ToObv();

        r.Should().HaveCount(1246);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<ObvResult> r0 = Noquotes
            .ToObv();

        r0.Should().BeEmpty();

        IReadOnlyList<ObvResult> r1 = Onequote
            .ToObv();

        r1.Should().HaveCount(1);
    }
}
