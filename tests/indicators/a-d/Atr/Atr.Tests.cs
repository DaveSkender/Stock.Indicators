using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Atr : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AtrResult> results = quotes.GetAtr(14).ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(502 - 13, results.Count(x => x.Atr != null));

        // sample values
        AtrResult r1 = results[12];
        Assert.AreEqual(1.32, NullMath.Round(r1.Tr, 8));
        Assert.AreEqual(null, r1.Atr);
        Assert.AreEqual(null, r1.Atrp);

        AtrResult r2 = results[13];
        Assert.AreEqual(1.45, NullMath.Round(r2.Tr, 8));
        Assert.AreEqual(1.3371, NullMath.Round(r2.Atr, 4));
        Assert.AreEqual(0.6258, NullMath.Round(r2.Atrp, 4));

        AtrResult r3 = results[24];
        Assert.AreEqual(0.88, NullMath.Round(r3.Tr, 8));
        Assert.AreEqual(1.3201, NullMath.Round(r3.Atr, 4));
        Assert.AreEqual(0.6104, NullMath.Round(r3.Atrp, 4));

        AtrResult r4 = results[249];
        Assert.AreEqual(0.58, NullMath.Round(r4.Tr, 8));
        Assert.AreEqual(1.3381, NullMath.Round(r4.Atr, 4));
        Assert.AreEqual(0.5187, NullMath.Round(r4.Atrp, 4));

        AtrResult r5 = results[501];
        Assert.AreEqual(2.67, NullMath.Round(r5.Tr, 8));
        Assert.AreEqual(6.1497, NullMath.Round(r5.Atr, 4));
        Assert.AreEqual(2.5072, NullMath.Round(r5.Atrp, 4));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetAtr(10)
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(502 - 18, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AtrResult> r = Indicator.GetAtr(badQuotes, 20);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Atr is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AtrResult> r0 = noquotes.GetAtr();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AtrResult> r1 = onequote.GetAtr();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<AtrResult> results = quotes.GetAtr(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        AtrResult last = results.LastOrDefault();
        Assert.AreEqual(2.67, NullMath.Round(last.Tr, 8));
        Assert.AreEqual(6.1497, NullMath.Round(last.Atr, 4));
        Assert.AreEqual(2.5072, NullMath.Round(last.Atrp, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAtr(quotes, 1));
}
