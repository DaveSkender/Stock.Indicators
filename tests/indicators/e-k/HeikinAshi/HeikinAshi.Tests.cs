namespace Series;

[TestClass]
public class HeikinAshiTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<HeikinAshiResult> results = Quotes
            .GetHeikinAshi()
            .ToList();

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
        IEnumerable<HeikinAshiResult> haQuotes = Quotes.GetHeikinAshi();
        IEnumerable<SmaResult> haSma = haQuotes.GetSma(5);
        Assert.AreEqual(498, haSma.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void ToQuotes()
    {
        List<HeikinAshiResult> results = Quotes
            .GetHeikinAshi()
            .ToList();

        List<Quote> haQuotes = results
            .ToQuotes()
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            HeikinAshiResult r = results[i];
            Quote q = haQuotes[i];

            Assert.AreEqual(r.Timestamp, q.Timestamp);
            Assert.AreEqual(r.Open, q.Open);
            Assert.AreEqual(r.High, q.High);
            Assert.AreEqual(r.Low, q.Low);
            Assert.AreEqual(r.Close, q.Close);
            Assert.AreEqual(r.Volume, q.Volume);
        }
    }

    [TestMethod]
    public override void BadData()
    {
        List<HeikinAshiResult> r = BadQuotes
            .GetHeikinAshi()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<HeikinAshiResult> r0 = Noquotes
            .GetHeikinAshi()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<HeikinAshiResult> r1 = Onequote
            .GetHeikinAshi()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }
}
