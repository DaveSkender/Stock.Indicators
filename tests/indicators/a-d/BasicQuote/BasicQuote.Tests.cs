namespace Tests.Indicators;

[TestClass]
public class BaseQuoteTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        // compose basic data
        List<BasicResult> o = quotes.GetBaseQuote(CandlePart.Open).ToList();
        List<BasicResult> h = quotes.GetBaseQuote(CandlePart.High).ToList();
        List<BasicResult> l = quotes.GetBaseQuote(CandlePart.Low).ToList();
        List<BasicResult> c = quotes.GetBaseQuote(CandlePart.Close).ToList();
        List<BasicResult> v = quotes.GetBaseQuote(CandlePart.Volume).ToList();
        List<BasicResult> hl = quotes.GetBaseQuote(CandlePart.HL2).ToList();
        List<BasicResult> hlc = quotes.GetBaseQuote(CandlePart.HLC3).ToList();
        List<BasicResult> oc = quotes.GetBaseQuote(CandlePart.OC2).ToList();
        List<BasicResult> ohl = quotes.GetBaseQuote(CandlePart.OHL3).ToList();
        List<BasicResult> ohlc = quotes.GetBaseQuote(CandlePart.OHLC4).ToList();

        // proper quantities
        Assert.AreEqual(502, c.Count);

        // samples
        BasicResult ro = o[501];
        BasicResult rh = h[501];
        BasicResult rl = l[501];
        BasicResult rc = c[501];
        BasicResult rv = v[501];
        BasicResult rhl = hl[501];
        BasicResult rhlc = hlc[501];
        BasicResult roc = oc[501];
        BasicResult rohl = ohl[501];
        BasicResult rohlc = ohlc[501];

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
        List<(DateTime Timestamp, double Value)> results = quotes
            .Use(CandlePart.Close)
            .ToList();

        Assert.AreEqual(502, results.Count);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetBaseQuote(CandlePart.Close)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Sma != null));
    }
}
