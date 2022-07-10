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
        Assert.AreEqual(502, results.Count(x => x.Tr != null));

        // sample values
        TrResult r1 = results[12];
        Assert.AreEqual(1.32, NullMath.Round(r1.Tr, 8));

        TrResult r2 = results[13];
        Assert.AreEqual(1.45, NullMath.Round(r2.Tr, 8));

        TrResult r3 = results[24];
        Assert.AreEqual(0.88, NullMath.Round(r3.Tr, 8));

        TrResult r4 = results[249];
        Assert.AreEqual(0.58, NullMath.Round(r4.Tr, 8));

        TrResult r5 = results[501];
        Assert.AreEqual(2.67, NullMath.Round(r5.Tr, 8));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmmaResult> results = quotes
            .GetTr()
            .GetSmma(14)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.Smma != null));

        // sample values - same as ATR
        SmmaResult r1 = results[12];
        Assert.AreEqual(null, r1.Smma);

        SmmaResult r2 = results[13];
        Assert.AreEqual(1.3371, NullMath.Round(r2.Smma, 4));

        SmmaResult r3 = results[24];
        Assert.AreEqual(1.3201, NullMath.Round(r3.Smma, 4));

        SmmaResult r4 = results[249];
        Assert.AreEqual(1.3381, NullMath.Round(r4.Smma, 4));

        SmmaResult r5 = results[501];
        Assert.AreEqual(6.1497, NullMath.Round(r5.Smma, 4));
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
