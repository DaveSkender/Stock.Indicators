using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Atr : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AtrResult> results = quotes
            .GetAtr(14)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(488, results.Count(x => x.Atr != null));

        // sample values
        AtrResult r13 = results[13];
        Assert.AreEqual(1.45, NullMath.Round(r13.Tr, 8));
        Assert.AreEqual(null, r13.Atr);
        Assert.AreEqual(null, r13.Atrp);

        AtrResult r14 = results[14];
        Assert.AreEqual(1.82, NullMath.Round(r14.Tr, 8));
        Assert.AreEqual(1.3364, NullMath.Round(r14.Atr, 4));
        Assert.AreEqual(0.6215, NullMath.Round(r14.Atrp, 4));

        AtrResult r24 = results[24];
        Assert.AreEqual(0.88, NullMath.Round(r24.Tr, 8));
        Assert.AreEqual(1.3034, NullMath.Round(r24.Atr, 4));
        Assert.AreEqual(0.6026, NullMath.Round(r24.Atrp, 4));

        AtrResult r249 = results[249];
        Assert.AreEqual(0.58, NullMath.Round(r249.Tr, 8));
        Assert.AreEqual(1.3381, NullMath.Round(r249.Atr, 4));
        Assert.AreEqual(0.5187, NullMath.Round(r249.Atrp, 4));

        AtrResult r501 = results[501];
        Assert.AreEqual(2.67, NullMath.Round(r501.Tr, 8));
        Assert.AreEqual(6.1497, NullMath.Round(r501.Atr, 4));
        Assert.AreEqual(2.5072, NullMath.Round(r501.Atrp, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetAtr(10)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502 - 19, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<AtrResult> r = badQuotes
            .GetAtr(20)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Atr is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AtrResult> r0 = noquotes
            .GetAtr()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<AtrResult> r1 = onequote
            .GetAtr()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<AtrResult> results = quotes
            .GetAtr(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 14, results.Count);

        AtrResult last = results.LastOrDefault();
        Assert.AreEqual(2.67, NullMath.Round(last.Tr, 8));
        Assert.AreEqual(6.1497, NullMath.Round(last.Atr, 4));
        Assert.AreEqual(2.5072, NullMath.Round(last.Atrp, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAtr(1));
}
