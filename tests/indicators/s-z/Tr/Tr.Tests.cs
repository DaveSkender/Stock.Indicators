namespace Tests.Indicators;

[TestClass]
public class TrTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TrResult> results = quotes
            .GetTr()
            .ToList();

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
    public void Chainor()
    {
        // same as ATR
        List<SmmaResult> results = quotes
            .GetTr()
            .GetSmma(14)
            .ToList();

        List<AtrResult> atrResults = quotes
            .GetAtr(14)
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            SmmaResult r = results[i];
            AtrResult a = atrResults[i];

            Assert.AreEqual(a.Date, r.Date);
            Assert.AreEqual(a.Atr, r.Smma);
        }
    }

    [TestMethod]
    public void BadData()
    {
        List<TrResult> r = badQuotes
            .GetTr()
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Tr is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<TrResult> r0 = noquotes
            .GetTr()
            .ToList();

        Assert.IsEmpty(r0);

        List<TrResult> r1 = onequote
            .GetTr()
            .ToList();

        Assert.HasCount(1, r1);
    }
}
