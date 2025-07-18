namespace StaticSeries;

[TestClass]
public class HeikinAshi : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<HeikinAshiResult> results = Quotes
            .ToHeikinAshi();

        // proper quantities
        Assert.AreEqual(502, results.Count);

        // sample value
        HeikinAshiResult r = results[501];
        Assert.AreEqual(241.3018m, r.Open.Round(4));
        Assert.AreEqual(245.54m, r.High.Round(4));
        Assert.AreEqual(241.3018m, r.Low.Round(4));
        Assert.AreEqual(244.6525m, r.Close.Round(4));
        Assert.AreEqual(147031456m, r.Volume);
    }

    [TestMethod]
    public void UseAsQuotes()
    {
        IReadOnlyList<HeikinAshiResult> haQuotes = Quotes.ToHeikinAshi();
        IReadOnlyList<SmaResult> haSma = haQuotes.ToSma(5);
        Assert.AreEqual(498, haSma.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<HeikinAshiResult> r = BadQuotes
            .ToHeikinAshi();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<HeikinAshiResult> r0 = Noquotes
            .ToHeikinAshi();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<HeikinAshiResult> r1 = Onequote
            .ToHeikinAshi();

        Assert.AreEqual(1, r1.Count);
    }
}
