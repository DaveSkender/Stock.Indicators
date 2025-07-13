namespace Utilities;

[TestClass]
public class QuoteParts : TestBase
{
    [TestMethod]
    public void Instantiation()
    {
        Quote q = Quotes[1];

        QuotePart sut0 = new(q.Timestamp, (double)q.Close);
        QuotePart sut1 = new(q);

        sut1.Should().Be(sut0);
    }

    [TestMethod]
    public void ConvertQuote()
    {
        // compose basic data
        QuotePart o = Quotes[501].ToQuotePart(CandlePart.Open);
        QuotePart h = Quotes[501].ToQuotePart(CandlePart.High);
        QuotePart l = Quotes[501].ToQuotePart(CandlePart.Low);
        QuotePart c = Quotes[501].ToQuotePart(CandlePart.Close);
        QuotePart v = Quotes[501].ToQuotePart(CandlePart.Volume);
        QuotePart hl = Quotes[501].ToQuotePart(CandlePart.HL2);
        QuotePart hlc = Quotes[501].ToQuotePart(CandlePart.HLC3);
        QuotePart oc = Quotes[501].ToQuotePart(CandlePart.OC2);
        QuotePart ohl = Quotes[501].ToQuotePart(CandlePart.OHL3);
        QuotePart ohlc = Quotes[501].ToQuotePart(CandlePart.OHLC4);

        // proper last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
        Assert.AreEqual(lastDate, c.Timestamp);

        // last values should be correct
        Assert.AreEqual(245.28, c.Value);
        Assert.AreEqual(244.92, o.Value);
        Assert.AreEqual(245.54, h.Value);
        Assert.AreEqual(242.87, l.Value);
        Assert.AreEqual(245.28, c.Value);
        Assert.AreEqual(147031456, v.Value);
        Assert.AreEqual(244.205, hl.Value);
        Assert.AreEqual(244.5633, hlc.Value.Round(4));
        Assert.AreEqual(245.1, oc.Value);
        Assert.AreEqual(244.4433, ohl.Value.Round(4));
        Assert.AreEqual(244.6525, ohlc.Value);
    }

    [TestMethod]
    public void ConvertList()
    {
        // compose data
        IReadOnlyList<QuotePart> o = Quotes.ToQuotePart(CandlePart.Open);
        IReadOnlyList<QuotePart> h = Quotes.ToQuotePart(CandlePart.High);
        IReadOnlyList<QuotePart> l = Quotes.ToQuotePart(CandlePart.Low);
        IReadOnlyList<QuotePart> c = Quotes.ToQuotePart(CandlePart.Close);
        IReadOnlyList<QuotePart> v = Quotes.ToQuotePart(CandlePart.Volume);
        IReadOnlyList<QuotePart> hl = Quotes.ToQuotePart(CandlePart.HL2);
        IReadOnlyList<QuotePart> hlc = Quotes.ToQuotePart(CandlePart.HLC3);
        IReadOnlyList<QuotePart> oc = Quotes.ToQuotePart(CandlePart.OC2);
        IReadOnlyList<QuotePart> ohl = Quotes.ToQuotePart(CandlePart.OHL3);
        IReadOnlyList<QuotePart> ohlc = Quotes.ToQuotePart(CandlePart.OHLC4);

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
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
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
}
