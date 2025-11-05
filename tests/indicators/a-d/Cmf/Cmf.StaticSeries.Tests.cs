namespace StaticSeries;

[TestClass]
public class Cmf : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<CmfResult> results = Quotes
            .ToCmf();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Cmf != null));

        // sample values
        CmfResult r1 = results[49];
        Assert.AreEqual(0.5468, r1.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(55609259, r1.MoneyFlowVolume.Round(2));
        Assert.AreEqual(0.350596, r1.Cmf.Round(6));

        CmfResult r2 = results[249];
        Assert.AreEqual(0.7778, r2.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(36433792.89, r2.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-0.040226, r2.Cmf.Round(6));

        CmfResult r3 = results[501];
        Assert.AreEqual(0.8052, r3.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r3.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-0.123754, r3.Cmf.Round(6));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToCmf()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<CmfResult> r = BadQuotes
            .ToCmf(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Cmf is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<CmfResult> r = BigQuotes
            .ToCmf(150);

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<CmfResult> r0 = Noquotes
            .ToCmf();

        Assert.IsEmpty(r0);

        IReadOnlyList<CmfResult> r1 = Onequote
            .ToCmf();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<CmfResult> results = Quotes
            .ToCmf()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 19, results);

        CmfResult last = results[^1];
        Assert.AreEqual(0.8052, last.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, last.MoneyFlowVolume.Round(2));
        Assert.AreEqual(-0.123754, last.Cmf.Round(6));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToCmf(0));
}
