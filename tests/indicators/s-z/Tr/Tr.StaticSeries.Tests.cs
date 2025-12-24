namespace StaticSeries;

[TestClass]
public class Tr : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<TrResult> sut = Quotes
            .ToTr();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Tr != null).Should().HaveCount(501);

        // sample values
        TrResult r0 = sut[0];
        r0.Tr.Should().BeNull();

        TrResult r1 = sut[1];
        Assert.AreEqual(1.42, r1.Tr.Round(8));

        TrResult r12 = sut[12];
        Assert.AreEqual(1.32, r12.Tr.Round(8));

        TrResult r13 = sut[13];
        Assert.AreEqual(1.45, r13.Tr.Round(8));

        TrResult r24 = sut[24];
        Assert.AreEqual(0.88, r24.Tr.Round(8));

        TrResult r249 = sut[249];
        Assert.AreEqual(0.58, r249.Tr.Round(8));

        TrResult r501 = sut[501];
        Assert.AreEqual(2.67, r501.Tr.Round(8));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        // same as ATR
        IReadOnlyList<SmmaResult> sut = Quotes
            .ToTr()
            .ToSmma(14);

        IReadOnlyList<AtrResult> atrResults = Quotes
            .ToAtr();

        for (int i = 0; i < sut.Count; i++)
        {
            SmmaResult r = sut[i];
            AtrResult a = atrResults[i];

            r.Timestamp.Should().Be(a.Timestamp);
            r.Smma.Should().Be(a.Atr);
        }
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<TrResult> r = BadQuotes
            .ToTr();

        r.Should().HaveCount(502);
        Assert.IsEmpty(r.Where(static x => x.Tr is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<TrResult> r0 = Noquotes
            .ToTr();

        r0.Should().BeEmpty();

        IReadOnlyList<TrResult> r1 = Onequote
            .ToTr();

        r1.Should().HaveCount(1);
    }
}
