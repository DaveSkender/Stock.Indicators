namespace StaticSeries;

[TestClass]
public class TrTests : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<TrResult> results = Quotes
            .GetTr();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Tr != null));

        // sample values
        TrResult r0 = results[0];
        Assert.AreEqual(null, r0.Tr);

        TrResult r1 = results[1];
        Assert.AreEqual(1.42, r1.Tr.Round(8));

        TrResult r12 = results[12];
        Assert.AreEqual(1.32, r12.Tr.Round(8));

        TrResult r13 = results[13];
        Assert.AreEqual(1.45, r13.Tr.Round(8));

        TrResult r24 = results[24];
        Assert.AreEqual(0.88, r24.Tr.Round(8));

        TrResult r249 = results[249];
        Assert.AreEqual(0.58, r249.Tr.Round(8));

        TrResult r501 = results[501];
        Assert.AreEqual(2.67, r501.Tr.Round(8));
    }

    [TestMethod]
    public void Chainor()
    {
        // same as ATR
        IReadOnlyList<SmmaResult> results = Quotes
            .GetTr()
            .GetSmma(14);

        IReadOnlyList<AtrResult> atrResults = Quotes
            .GetAtr();

        for (int i = 0; i < results.Count; i++)
        {
            SmmaResult r = results[i];
            AtrResult a = atrResults[i];

            Assert.AreEqual(a.Timestamp, r.Timestamp);
            Assert.AreEqual(a.Atr, r.Smma);
        }
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<TrResult> r = BadQuotes
            .GetTr();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Tr is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<TrResult> r0 = Noquotes
            .GetTr();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<TrResult> r1 = Onequote
            .GetTr();

        Assert.AreEqual(1, r1.Count);
    }
}
