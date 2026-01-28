namespace StaticSeries;

[TestClass]
public class QuoteParts : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        // compose data
        IReadOnlyList<TimeValue> o = Quotes.Use(CandlePart.Open);
        IReadOnlyList<TimeValue> h = Quotes.Use(CandlePart.High);
        IReadOnlyList<TimeValue> l = Quotes.Use(CandlePart.Low);
        IReadOnlyList<TimeValue> c = Quotes.Use(CandlePart.Close);
        IReadOnlyList<TimeValue> v = Quotes.Use(CandlePart.Volume);
        IReadOnlyList<TimeValue> hl = Quotes.Use(CandlePart.HL2);
        IReadOnlyList<TimeValue> hlc = Quotes.Use(CandlePart.HLC3);
        IReadOnlyList<TimeValue> oc = Quotes.Use(CandlePart.OC2);
        IReadOnlyList<TimeValue> ohl = Quotes.Use(CandlePart.OHL3);
        IReadOnlyList<TimeValue> ohlc = Quotes.Use(CandlePart.OHLC4);

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

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(493);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<TimeValue> r = BadQuotes
            .Use(CandlePart.Close);

        r.Should().HaveCount(502);
        r.Where(static x => x.Value is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<TimeValue> r0 = Noquotes
            .Use(CandlePart.Close);

        r0.Should().BeEmpty();

        IReadOnlyList<TimeValue> r1 = Onequote
            .Use(CandlePart.Close);

        r1.Should().HaveCount(1);
    }
}
