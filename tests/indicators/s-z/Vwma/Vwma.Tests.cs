namespace Tests.Indicators;

[TestClass]
public class VwmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<VwmaResult> results = quotes
            .GetVwma(10)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(493, results.Count(x => x.Vwma != null));

        // sample values
        VwmaResult r8 = results[8];
        Assert.IsNull(r8.Vwma);

        Assert.AreEqual(213.981942, results[9].Vwma.Round(6));
        Assert.AreEqual(215.899211, results[24].Vwma.Round(6));
        Assert.AreEqual(226.302760, results[99].Vwma.Round(6));
        Assert.AreEqual(257.053654, results[249].Vwma.Round(6));
        Assert.AreEqual(242.101548, results[501].Vwma.Round(6));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetVwma(10)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<VwmaResult> r = badQuotes
            .GetVwma(15)
            .ToList();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Vwma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<VwmaResult> r0 = noquotes
            .GetVwma(4)
            .ToList();

        Assert.IsEmpty(r0);

        List<VwmaResult> r1 = onequote
            .GetVwma(4)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        List<VwmaResult> results = quotes
            .GetVwma(10)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - 9, results);

        VwmaResult last = results.LastOrDefault();
        Assert.AreEqual(242.101548, last.Vwma.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetVwma(0));
}
