namespace Tests.Indicators;

[TestClass]
public class AtrTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AtrResult> results = quotes
            .GetAtr(14)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(488, results.Count(static x => x.Atr != null));

        // sample values
        AtrResult r13 = results[13];
        Assert.AreEqual(1.45, r13.Tr.Round(8));
        Assert.IsNull(r13.Atr);
        Assert.IsNull(r13.Atrp);

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
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetAtr(10)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(502 - 19, results.Count(static x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<AtrResult> r = badQuotes
            .GetAtr(20)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Atr is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AtrResult> r0 = noquotes
            .GetAtr()
            .ToList();

        Assert.IsEmpty(r0);

        List<AtrResult> r1 = onequote
            .GetAtr()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<AtrResult> results = quotes
            .GetAtr(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 14, results);

        AtrResult last = results.LastOrDefault();
        Assert.AreEqual(2.67, last.Tr.Round(8));
        Assert.AreEqual(6.1497, last.Atr.Round(4));
        Assert.AreEqual(2.5072, last.Atrp.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetAtr(1));
}
