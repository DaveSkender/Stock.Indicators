namespace Utilities;

[TestClass]
public class QuoteParts : TestBaseWithPrecision
{
    [TestMethod]
    public void ConvertQuote()
    {
        // compose basic data
        TimeValue o = Quotes[501].ToQuotePart(CandlePart.Open);
        TimeValue h = Quotes[501].ToQuotePart(CandlePart.High);
        TimeValue l = Quotes[501].ToQuotePart(CandlePart.Low);
        TimeValue c = Quotes[501].ToQuotePart(CandlePart.Close);
        TimeValue v = Quotes[501].ToQuotePart(CandlePart.Volume);
        TimeValue hl = Quotes[501].ToQuotePart(CandlePart.HL2);
        TimeValue hlc = Quotes[501].ToQuotePart(CandlePart.HLC3);
        TimeValue oc = Quotes[501].ToQuotePart(CandlePart.OC2);
        TimeValue ohl = Quotes[501].ToQuotePart(CandlePart.OHL3);
        TimeValue ohlc = Quotes[501].ToQuotePart(CandlePart.OHLC4);

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
        IReadOnlyList<TimeValue> o = Quotes.ToQuotePart(CandlePart.Open);
        IReadOnlyList<TimeValue> h = Quotes.ToQuotePart(CandlePart.High);
        IReadOnlyList<TimeValue> l = Quotes.ToQuotePart(CandlePart.Low);
        IReadOnlyList<TimeValue> c = Quotes.ToQuotePart(CandlePart.Close);
        IReadOnlyList<TimeValue> v = Quotes.ToQuotePart(CandlePart.Volume);
        IReadOnlyList<TimeValue> hl = Quotes.ToQuotePart(CandlePart.HL2);
        IReadOnlyList<TimeValue> hlc = Quotes.ToQuotePart(CandlePart.HLC3);
        IReadOnlyList<TimeValue> oc = Quotes.ToQuotePart(CandlePart.OC2);
        IReadOnlyList<TimeValue> ohl = Quotes.ToQuotePart(CandlePart.OHL3);
        IReadOnlyList<TimeValue> ohlc = Quotes.ToQuotePart(CandlePart.OHLC4);

        // proper quantities
        c.Should().HaveCount(502);

        // samples
        TimeValue ro = o[501];
        TimeValue rh = h[501];
        TimeValue rl = l[501];
        TimeValue rc = c[501];
        TimeValue rv = v[501];
        TimeValue rhl = hl[501];
        TimeValue rhlc = hlc[501];
        TimeValue roc = oc[501];
        TimeValue rohl = ohl[501];
        TimeValue rohlc = ohlc[501];

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
