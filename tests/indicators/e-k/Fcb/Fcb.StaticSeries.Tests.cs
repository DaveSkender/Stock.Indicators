namespace StaticSeries;

[TestClass]
public class Fcb : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<FcbResult> results = Quotes
            .ToFcb();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(497, results.Where(static x => x.UpperBand != null));
        Assert.HasCount(493, results.Where(static x => x.LowerBand != null));

        // sample values
        FcbResult r1 = results[4];
        Assert.IsNull(r1.UpperBand);
        Assert.IsNull(r1.LowerBand);

        FcbResult r2 = results[10];
        Assert.AreEqual(214.84m, r2.UpperBand);
        Assert.AreEqual(212.53m, r2.LowerBand);

        FcbResult r3 = results[120];
        Assert.AreEqual(233.35m, r3.UpperBand);
        Assert.AreEqual(231.14m, r3.LowerBand);

        FcbResult r4 = results[180];
        Assert.AreEqual(236.78m, r4.UpperBand);
        Assert.AreEqual(233.56m, r4.LowerBand);

        FcbResult r5 = results[250];
        Assert.AreEqual(258.70m, r5.UpperBand);
        Assert.AreEqual(257.04m, r5.LowerBand);

        FcbResult r6 = results[501];
        Assert.AreEqual(262.47m, r6.UpperBand);
        Assert.AreEqual(229.42m, r6.LowerBand);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<FcbResult> r = BadQuotes
            .ToFcb();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<FcbResult> r0 = Noquotes
            .ToFcb();

        Assert.IsEmpty(r0);

        IReadOnlyList<FcbResult> r1 = Onequote
            .ToFcb();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<FcbResult> results = Quotes
            .ToFcb()
            .Condense();

        // assertions
        Assert.HasCount(502 - 5, results);

        FcbResult last = results[^1];
        Assert.AreEqual(262.47m, last.UpperBand);
        Assert.AreEqual(229.42m, last.LowerBand);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<FcbResult> results = Quotes
            .ToFcb()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 5, results);

        FcbResult last = results[^1];
        Assert.AreEqual(262.47m, last.UpperBand);
        Assert.AreEqual(229.42m, last.LowerBand);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToFcb(1));
}
