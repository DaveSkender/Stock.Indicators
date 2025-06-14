namespace StaticSeries;

[TestClass]
public class Bop : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<BopResult> results = Quotes
            .ToBop();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(489, results.Count(x => x.Bop != null));

        // sample values
        BopResult r1 = results[12];
        Assert.AreEqual(null, r1.Bop);

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
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToBop()
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<BopResult> r = Data.GetBtcUsdNan()
            .ToBop(50);

        Assert.AreEqual(0, r.Count(x => x.Bop is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<BopResult> r = BadQuotes
            .ToBop();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Bop is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<BopResult> r0 = Noquotes
            .ToBop();
        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<BopResult> r1 = Onequote
            .ToBop();
        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<BopResult> results = Quotes
            .ToBop()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 13, results.Count);

        BopResult last = results[^1];
        Assert.AreEqual(-0.292788, last.Bop.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToBop(0));
}
