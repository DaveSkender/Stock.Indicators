namespace StaticSeries;

[TestClass]
public class Atr : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<AtrResult> results = Quotes
            .ToAtr();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Atr != null));

        // sample values
        AtrResult r13 = results[13];
        Assert.AreEqual(1.45, r13.Tr.Round(8));
        Assert.AreEqual(null, r13.Atr);
        Assert.AreEqual(null, r13.Atrp);

        AtrResult r14 = results[14];
        Assert.AreEqual(1.82, r14.Tr.Round(8));
        Assert.AreEqual(1.3364, r14.Atr.Round(4));
        Assert.AreEqual(0.6215, r14.Atrp.Round(4));

        AtrResult r24 = results[24];
        Assert.AreEqual(0.88, r24.Tr.Round(8));
        Assert.AreEqual(1.3034, r24.Atr.Round(4));
        Assert.AreEqual(0.6026, r24.Atrp.Round(4));

        AtrResult r249 = results[249];
        Assert.AreEqual(0.58, r249.Tr.Round(8));
        Assert.AreEqual(1.3381, r249.Atr.Round(4));
        Assert.AreEqual(0.5187, r249.Atrp.Round(4));

        AtrResult r501 = results[501];
        Assert.AreEqual(2.67, r501.Tr.Round(8));
        Assert.AreEqual(6.1497, r501.Atr.Round(4));
        Assert.AreEqual(2.5072, r501.Atrp.Round(4));
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
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToAtr(10)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502 - 19, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AtrResult> r = BadQuotes
            .ToAtr(20);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Atr is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AtrResult> r0 = Noquotes
            .ToAtr();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<AtrResult> r1 = Onequote
            .ToAtr();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AtrResult> results = Quotes
            .ToAtr()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        AtrResult last = results[^1];
        Assert.AreEqual(2.67, last.Tr.Round(8));
        Assert.AreEqual(6.1497, last.Atr.Round(4));
        Assert.AreEqual(2.5072, last.Atrp.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAtr(1));
}
