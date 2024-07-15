namespace Tests.Indicators.Series;

[TestClass]
public class UseTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // compose basic data
        List<QuotePart> o = Quotes.Use(CandlePart.Open).ToList();
        List<QuotePart> h = Quotes.Use(CandlePart.High).ToList();
        List<QuotePart> l = Quotes.Use(CandlePart.Low).ToList();
        List<QuotePart> c = Quotes.Use(CandlePart.Close).ToList();
        List<QuotePart> v = Quotes.Use(CandlePart.Volume).ToList();
        List<QuotePart> hl = Quotes.Use(CandlePart.HL2).ToList();
        List<QuotePart> hlc = Quotes.Use(CandlePart.HLC3).ToList();
        List<QuotePart> oc = Quotes.Use(CandlePart.OC2).ToList();
        List<QuotePart> ohl = Quotes.Use(CandlePart.OHL3).ToList();
        List<QuotePart> ohlc = Quotes.Use(CandlePart.OHLC4).ToList();

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
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, rc.Timestamp);

        // last values should be correct
        Assert.AreEqual(245.28, rc.Value);
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
        List<QuotePart> results = Quotes
            .Use(CandlePart.Close)
            .ToList();

        Assert.AreEqual(502, results.Count);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<QuotePart> r = BadQuotes
            .Use(CandlePart.Close)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Value is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<QuotePart> r0 = Noquotes
            .Use(CandlePart.Close)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<QuotePart> r1 = Onequote
            .Use(CandlePart.Close)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }
}
