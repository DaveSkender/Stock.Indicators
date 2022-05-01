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
        Assert.AreEqual(502 - 13, results.Where(x => x.Atr != null).Count());

        // sample values
        AtrResult r1 = results[12];
        Assert.AreEqual(1.32m, r1.Tr);
        Assert.AreEqual(null, r1.Atr);
        Assert.AreEqual(null, r1.Atrp);

        AtrResult r2 = results[13];
        Assert.AreEqual(1.45m, r2.Tr);
        Assert.AreEqual(1.3371m, NullMath.Round(r2.Atr, 4));
        Assert.AreEqual(0.6258m, NullMath.Round(r2.Atrp, 4));

        AtrResult r3 = results[24];
        Assert.AreEqual(0.88m, r3.Tr);
        Assert.AreEqual(1.3201m, NullMath.Round(r3.Atr, 4));
        Assert.AreEqual(0.6104m, NullMath.Round(r3.Atrp, 4));

        AtrResult r4 = results[249];
        Assert.AreEqual(0.58m, r4.Tr);
        Assert.AreEqual(1.3381m, NullMath.Round(r4.Atr, 4));
        Assert.AreEqual(0.5187m, NullMath.Round(r4.Atrp, 4));

        AtrResult r5 = results[501];
        Assert.AreEqual(2.67m, r5.Tr);
        Assert.AreEqual(6.1497m, NullMath.Round(r5.Atr, 4));
        Assert.AreEqual(2.5072m, NullMath.Round(r5.Atrp, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AtrResult> r = Indicator.GetAtr(badQuotes, 20);
        Assert.AreEqual(502, r.Count());
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
        Assert.AreEqual(2.67m, last.Tr);
        Assert.AreEqual(6.1497m, NullMath.Round(last.Atr, 4));
        Assert.AreEqual(2.5072m, NullMath.Round(last.Atrp, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetAtr(quotes, 1));
    }
}
