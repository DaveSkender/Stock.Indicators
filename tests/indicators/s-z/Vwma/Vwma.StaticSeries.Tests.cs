namespace StaticSeries;

[TestClass]
public class Vwma : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<VwmaResult> results = Quotes
            .ToVwma(10);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(x => x.Vwma != null));

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
        IReadOnlyList<SmaResult> results = Quotes
            .ToVwma(10)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<VwmaResult> r = BadQuotes
            .ToVwma(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Vwma is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<VwmaResult> r0 = Noquotes
            .ToVwma(4);

        Assert.IsEmpty(r0);

        IReadOnlyList<VwmaResult> r1 = Onequote
            .ToVwma(4);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<VwmaResult> results = Quotes
            .ToVwma(10)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 9, results);

        VwmaResult last = results[^1];
        Assert.AreEqual(242.101548, last.Vwma.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToVwma(0));
}
