namespace Tests.Indicators;

[TestClass]
public class UseTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // compose basic data
        List<QuotePart> o = quotes.Use(CandlePart.Open).ToList();
        List<QuotePart> h = quotes.Use(CandlePart.High).ToList();
        List<QuotePart> l = quotes.Use(CandlePart.Low).ToList();
        List<QuotePart> c = quotes.Use(CandlePart.Close).ToList();
        List<QuotePart> v = quotes.Use(CandlePart.Volume).ToList();
        List<QuotePart> hl = quotes.Use(CandlePart.HL2).ToList();
        List<QuotePart> hlc = quotes.Use(CandlePart.HLC3).ToList();
        List<QuotePart> oc = quotes.Use(CandlePart.OC2).ToList();
        List<QuotePart> ohl = quotes.Use(CandlePart.OHL3).ToList();
        List<QuotePart> ohlc = quotes.Use(CandlePart.OHLC4).ToList();

        // proper quantities
        Assert.AreEqual(502, c.Count);

        // samples
        QuotePart ro = o[501];
        QuotePart rh = h[501];
        QuotePart rl = l[501];
        QuotePart rc = c[501];
        QuotePart rv = v[501];
        QuotePart rhl = hl[501];
        QuotePart rhlc = hlc[501];
        QuotePart roc = oc[501];
        QuotePart rohl = ohl[501];
        QuotePart rohlc = ohlc[501];

        // proper last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, rc.Timestamp);

        // last values should be correct
        Assert.AreEqual(244.92, ro.Value);
        Assert.AreEqual(245.54, rh.Value);
        Assert.AreEqual(242.87, rl.Value);
        Assert.AreEqual(245.28, rc.Value);
        Assert.AreEqual(147031456, rv.Value);
        Assert.AreEqual(244.205, rhl.Value);
        Assert.AreEqual(244.5633, rhlc.Value.Round(4));
        Assert.AreEqual(245.1, roc.Value);
        Assert.AreEqual(244.4433, rohl.Value.Round(4));
        Assert.AreEqual(244.6525, rohlc.Value);
    }

    [TestMethod]
    public void Use()
    {
        List<QuotePart> results = quotes
            .Use(CandlePart.Close)
            .ToList();

        Assert.AreEqual(502, results.Count);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }
}
