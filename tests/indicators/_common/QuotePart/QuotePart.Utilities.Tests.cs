namespace Utilities;

[TestClass]
public class QuoteParts : TestBaseWithPrecision
{
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
        c.Timestamp.Should().Be(lastDate);

        // last values should be correct
        c.Value.Should().Be(245.28);
        o.Value.Should().Be(244.92);
        h.Value.Should().Be(245.54);
        l.Value.Should().Be(242.87);
        c.Value.Should().Be(245.28);
        v.Value.Should().Be(147031456);
        hl.Value.Should().Be(244.205);
        hlc.Value.Should().BeApproximately(244.5633, Money4);
        oc.Value.Should().Be(245.1);
        ohl.Value.Should().BeApproximately(244.4433, Money4);
        ohlc.Value.Should().Be(244.6525);
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
        c.Should().HaveCount(502);

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
        rc.Timestamp.Should().Be(lastDate);

        // last values should be correct
        rc.Value.Should().Be(245.28);
        ro.Value.Should().Be(244.92);
        rh.Value.Should().Be(245.54);
        rl.Value.Should().Be(242.87);
        rc.Value.Should().Be(245.28);
        rv.Value.Should().Be(147031456);
        rhl.Value.Should().Be(244.205);
        rhlc.Value.Should().BeApproximately(244.5633, Money4);
        roc.Value.Should().Be(245.1);
        rohl.Value.Should().BeApproximately(244.4433, Money4);
        rohlc.Value.Should().Be(244.6525);
    }
}
