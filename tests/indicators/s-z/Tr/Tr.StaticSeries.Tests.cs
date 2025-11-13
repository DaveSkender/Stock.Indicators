namespace StaticSeries;

[TestClass]
public class Tr : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<TrResult> results = Quotes
            .ToTr();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Tr != null));

        // sample values
        TrResult r0 = results[0];
        Assert.IsNull(r0.Tr);

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
    public void ChainingFromResults_WorksAsExpected()
    {
        // same as ATR
        IReadOnlyList<SmmaResult> results = Quotes
            .ToTr()
            .ToSmma(14);

        IReadOnlyList<AtrResult> atrResults = Quotes
            .ToAtr();

        for (int i = 0; i < results.Count; i++)
        {
            SmmaResult r = results[i];
            AtrResult a = atrResults[i];

            Assert.AreEqual(a.Timestamp, r.Timestamp);
            Assert.AreEqual(a.Atr, r.Smma);
        }
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<TrResult> r = BadQuotes
            .ToTr();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Tr is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<TrResult> r0 = Noquotes
            .ToTr();

        Assert.IsEmpty(r0);

        IReadOnlyList<TrResult> r1 = Onequote
            .ToTr();

        Assert.HasCount(1, r1);
    }
}
