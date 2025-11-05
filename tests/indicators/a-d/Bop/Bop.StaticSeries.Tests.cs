namespace StaticSeries;

[TestClass]
public class Bop : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<BopResult> results = Quotes
            .ToBop();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(489, results.Where(static x => x.Bop != null));

        // sample values
        BopResult r1 = results[12];
        Assert.IsNull(r1.Bop);

        BopResult r2 = results[13];
        Assert.AreEqual(0.081822, r2.Bop.Round(6));

        BopResult r3 = results[149];
        Assert.AreEqual(-0.016203, r3.Bop.Round(6));

        BopResult r4 = results[249];
        Assert.AreEqual(-0.058682, r4.Bop.Round(6));

        BopResult r5 = results[501];
        Assert.AreEqual(-0.292788, r5.Bop.Round(6));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToBop()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(480, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<BopResult> r = Data.GetBtcUsdNan()
            .ToBop(50);

        Assert.IsEmpty(r.Where(static x => x.Bop is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<BopResult> r = BadQuotes
            .ToBop();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Bop is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<BopResult> r0 = Noquotes
            .ToBop();
        Assert.IsEmpty(r0);

        IReadOnlyList<BopResult> r1 = Onequote
            .ToBop();
        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<BopResult> results = Quotes
            .ToBop()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 13, results);

        BopResult last = results[^1];
        Assert.AreEqual(-0.292788, last.Bop.Round(6));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToBop(0));
}
