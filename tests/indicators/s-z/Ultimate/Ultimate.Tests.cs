namespace Series;

[TestClass]
public class UltimateTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<UltimateResult> results = Quotes
            .GetUltimate()
            .ToList();

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
        List<SmaResult> results = Quotes
            .GetUltimate()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(465, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<UltimateResult> r = BadQuotes
            .GetUltimate(1, 2, 3)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Ultimate is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<UltimateResult> r0 = Noquotes
            .GetUltimate()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<UltimateResult> r1 = Onequote
            .GetUltimate()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<UltimateResult> results = Quotes
            .GetUltimate()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - 28, results.Count);

        UltimateResult last = results.LastOrDefault();
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
