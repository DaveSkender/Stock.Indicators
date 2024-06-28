namespace Tests.Indicators.Series;

[TestClass]
public class UseTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // compose basic data
        List<Reusable> o = quotes.Use(CandlePart.Open).ToList();
        List<Reusable> h = quotes.Use(CandlePart.High).ToList();
        List<Reusable> l = quotes.Use(CandlePart.Low).ToList();
        List<Reusable> c = quotes.Use(CandlePart.Close).ToList();
        List<Reusable> v = quotes.Use(CandlePart.Volume).ToList();
        List<Reusable> hl = quotes.Use(CandlePart.HL2).ToList();
        List<Reusable> hlc = quotes.Use(CandlePart.HLC3).ToList();
        List<Reusable> oc = quotes.Use(CandlePart.OC2).ToList();
        List<Reusable> ohl = quotes.Use(CandlePart.OHL3).ToList();
        List<Reusable> ohlc = quotes.Use(CandlePart.OHLC4).ToList();

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
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
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
        List<Reusable> results = quotes
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
