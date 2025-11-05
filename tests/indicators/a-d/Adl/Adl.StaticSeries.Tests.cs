namespace StaticSeries;

[TestClass]
public class Adl : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<AdlResult> results = Quotes
            .ToAdl();

        // proper quantities
        Assert.HasCount(502, results);

        // sample values
        AdlResult r1 = results[249];
        Assert.AreEqual(0.7778, r1.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(36433792.89, r1.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3266400865.74, r1.Adl.Round(2));

        AdlResult r2 = results[501];
        Assert.AreEqual(0.8052, r2.MoneyFlowMultiplier.Round(4));
        Assert.AreEqual(118396116.25, r2.MoneyFlowVolume.Round(2));
        Assert.AreEqual(3439986548.42, r2.Adl.Round(2));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToAdl()
            .ToSma(10);

        // assertions

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<AdlResult> r = BadQuotes
            .ToAdl();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => double.IsNaN(x.Adl)));
    }

    [TestMethod]
    public void BigQuoteValues_DoesNotFail()
    {
        IReadOnlyList<AdlResult> r = BigQuotes
            .ToAdl();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void RandomQuotes_WorksAsExpected()
    {
        IReadOnlyList<AdlResult> r = RandomQuotes
            .ToAdl();

        Assert.HasCount(1000, r);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<AdlResult> r0 = Noquotes
            .ToAdl();

        Assert.IsEmpty(r0);

        IReadOnlyList<AdlResult> r1 = Onequote
            .ToAdl();

        Assert.HasCount(1, r1);
    }
}
