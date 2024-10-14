namespace StaticSeries;

[TestClass]
public class QuoteParts : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // compose data
        IReadOnlyList<QuotePart> o = Quotes.Use(CandlePart.Open);
        IReadOnlyList<QuotePart> h = Quotes.Use(CandlePart.High);
        IReadOnlyList<QuotePart> l = Quotes.Use(CandlePart.Low);
        IReadOnlyList<QuotePart> c = Quotes.Use(CandlePart.Close);
        IReadOnlyList<QuotePart> v = Quotes.Use(CandlePart.Volume);
        IReadOnlyList<QuotePart> hl = Quotes.Use(CandlePart.HL2);
        IReadOnlyList<QuotePart> hlc = Quotes.Use(CandlePart.HLC3);
        IReadOnlyList<QuotePart> oc = Quotes.Use(CandlePart.OC2);
        IReadOnlyList<QuotePart> ohl = Quotes.Use(CandlePart.OHL3);
        IReadOnlyList<QuotePart> ohlc = Quotes.Use(CandlePart.OHLC4);

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
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<QuotePart> r = BadQuotes
            .Use(CandlePart.Close);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Value is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<QuotePart> r0 = Noquotes
            .Use(CandlePart.Close);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<QuotePart> r1 = Onequote
            .Use(CandlePart.Close);

        Assert.AreEqual(1, r1.Count);
    }
}
