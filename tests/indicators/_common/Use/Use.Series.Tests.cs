namespace Tests.Indicators.Series;

[TestClass]
public class UseTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // compose basic data
        List<Reusable> o = Quotes.Use(CandlePart.Open).ToList();
        List<Reusable> h = Quotes.Use(CandlePart.High).ToList();
        List<Reusable> l = Quotes.Use(CandlePart.Low).ToList();
        List<Reusable> c = Quotes.Use(CandlePart.Close).ToList();
        List<Reusable> v = Quotes.Use(CandlePart.Volume).ToList();
        List<Reusable> hl = Quotes.Use(CandlePart.Hl2).ToList();
        List<Reusable> hlc = Quotes.Use(CandlePart.Hlc3).ToList();
        List<Reusable> oc = Quotes.Use(CandlePart.Oc2).ToList();
        List<Reusable> ohl = Quotes.Use(CandlePart.Ohl3).ToList();
        List<Reusable> ohlc = Quotes.Use(CandlePart.Ohlc4).ToList();

        // proper quantities
        Assert.AreEqual(502, c.Count);

        // samples
        Reusable ro = o[501];
        Reusable rh = h[501];
        Reusable rl = l[501];
        Reusable rc = c[501];
        Reusable rv = v[501];
        Reusable rhl = hl[501];
        Reusable rhlc = hlc[501];
        Reusable roc = oc[501];
        Reusable rohl = ohl[501];
        Reusable rohlc = ohlc[501];

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
        List<Reusable> results = Quotes
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
}
