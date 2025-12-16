namespace StaticSeries;

[TestClass]
public class Ultimate : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<UltimateResult> results = Quotes
            .ToUltimate();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Ultimate != null));

        // sample values
        UltimateResult r1 = results[74];
        Assert.AreEqual(51.7770, r1.Ultimate.Round(4));

        UltimateResult r2 = results[249];
        Assert.AreEqual(45.3121, r2.Ultimate.Round(4));

        UltimateResult r3 = results[501];
        Assert.AreEqual(49.5257, r3.Ultimate.Round(4));
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<UltimateResult> results = Quotes.ToUltimate(7, 14, 28);
        TestAsserts.AlwaysBounded(results, x => x.Ultimate, 0, 100);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToUltimate()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(465, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<UltimateResult> r = BadQuotes
            .ToUltimate(1, 2, 3);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Ultimate is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<UltimateResult> r0 = Noquotes
            .ToUltimate();

        Assert.IsEmpty(r0);

        IReadOnlyList<UltimateResult> r1 = Onequote
            .ToUltimate();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UltimateResult> results = Quotes
            .ToUltimate()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 28, results);

        UltimateResult last = results[^1];
        Assert.AreEqual(49.5257, last.Ultimate.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad short period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUltimate(0));

        // bad middle period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUltimate(7, 6));

        // bad long period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToUltimate(7, 14, 11));
    }
}
