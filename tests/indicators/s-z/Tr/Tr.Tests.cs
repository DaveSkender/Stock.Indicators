using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Tr : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<TrResult> results = quotes.GetTr().ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(501, results.Count(x => x.Tr != null));

        // sample values
        TrResult r0 = results[0];
        Assert.AreEqual(null, r0.Tr);

        TrResult r1 = results[1];
        Assert.AreEqual(1.42, NullMath.Round(r1.Tr, 8));

        TrResult r12 = results[12];
        Assert.AreEqual(1.32, NullMath.Round(r12.Tr, 8));

        TrResult r13 = results[13];
        Assert.AreEqual(1.45, NullMath.Round(r13.Tr, 8));

        TrResult r24 = results[24];
        Assert.AreEqual(0.88, NullMath.Round(r24.Tr, 8));

        TrResult r249 = results[249];
        Assert.AreEqual(0.58, NullMath.Round(r249.Tr, 8));

        TrResult r501 = results[501];
        Assert.AreEqual(2.67, NullMath.Round(r501.Tr, 8));
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
        IEnumerable<TrResult> r = badQuotes.GetTr();
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Tr is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<TrResult> r0 = noquotes.GetTr();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<TrResult> r1 = onequote.GetTr();
        Assert.AreEqual(1, r1.Count());
    }
}
