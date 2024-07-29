namespace StaticSeries;

[TestClass]
public class Ultimate : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<UltimateResult> results = Quotes
            .GetUltimate();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Ultimate != null));

        // sample values
        UltimateResult r1 = results[74];
        Assert.AreEqual(51.7770, r1.Ultimate.Round(4));

        UltimateResult r2 = results[249];
        Assert.AreEqual(45.3121, r2.Ultimate.Round(4));

        UltimateResult r3 = results[501];
        Assert.AreEqual(49.5257, r3.Ultimate.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetUltimate()
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(465, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<UltimateResult> r = BadQuotes
            .GetUltimate(1, 2, 3);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Ultimate is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<UltimateResult> r0 = Noquotes
            .GetUltimate();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<UltimateResult> r1 = Onequote
            .GetUltimate();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<UltimateResult> results = Quotes
            .GetUltimate()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 28, results.Count);

        UltimateResult last = results[^1];
        Assert.AreEqual(49.5257, last.Ultimate.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad short period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetUltimate(0));

        // bad middle period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetUltimate(7, 6));

        // bad long period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetUltimate(7, 14, 11));
    }
}
