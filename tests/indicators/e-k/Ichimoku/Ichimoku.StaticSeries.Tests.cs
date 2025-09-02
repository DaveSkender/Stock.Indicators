namespace StaticSeries;

[TestClass]
public class Ichimoku : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int tenkanPeriods = 9;
        int kijunPeriods = 26;
        int senkouBPeriods = 52;

        IReadOnlyList<IchimokuResult> results = Quotes
            .ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(494, results.Where(x => x.TenkanSen != null));
        Assert.HasCount(477, results.Where(x => x.KijunSen != null));
        Assert.HasCount(451, results.Where(x => x.SenkouSpanA != null));
        Assert.HasCount(425, results.Where(x => x.SenkouSpanB != null));
        Assert.HasCount(476, results.Where(x => x.ChikouSpan != null));

        // sample values
        IchimokuResult r1 = results[51];
        Assert.AreEqual(224.465m, r1.TenkanSen);
        Assert.AreEqual(221.94m, r1.KijunSen);
        Assert.AreEqual(214.8325m, r1.SenkouSpanA);
        Assert.IsNull(r1.SenkouSpanB);
        Assert.AreEqual(226.35m, r1.ChikouSpan);

        IchimokuResult r2 = results[249];
        Assert.AreEqual(257.15m, r2.TenkanSen);
        Assert.AreEqual(253.085m, r2.KijunSen);
        Assert.AreEqual(246.3125m, r2.SenkouSpanA);
        Assert.AreEqual(241.685m, r2.SenkouSpanB);
        Assert.AreEqual(259.21m, r2.ChikouSpan);

        IchimokuResult r3 = results[475];
        Assert.AreEqual(265.575m, r3.TenkanSen);
        Assert.AreEqual(263.965m, r3.KijunSen);
        Assert.AreEqual(274.9475m, r3.SenkouSpanA);
        Assert.AreEqual(274.95m, r3.SenkouSpanB);
        Assert.AreEqual(245.28m, r3.ChikouSpan);

        IchimokuResult r4 = results[501];
        Assert.AreEqual(241.26m, r4.TenkanSen);
        Assert.AreEqual(251.505m, r4.KijunSen);
        Assert.AreEqual(264.77m, r4.SenkouSpanA);
        Assert.AreEqual(269.82m, r4.SenkouSpanB);
        Assert.IsNull(r4.ChikouSpan);
    }

    [TestMethod]
    public void Extended()
    {
        IReadOnlyList<IchimokuResult> results = Quotes
            .ToIchimoku(3, 13, 40, 0, 0);

        Assert.HasCount(502, results);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<IchimokuResult> r = BadQuotes
            .ToIchimoku();

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<IchimokuResult> r0 = Noquotes
            .ToIchimoku();

        Assert.IsEmpty(r0);

        IReadOnlyList<IchimokuResult> r1 = Onequote
            .ToIchimoku();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<IchimokuResult> results = Quotes
            .ToIchimoku()
            .Condense();

        Assert.HasCount(502, results);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToIchimoku(0));

        // bad short span period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToIchimoku(9, 0));

        // bad long span period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToIchimoku(9, 26, 26));

        // invalid offsets
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToIchimoku(9, 26, 52, -1));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToIchimoku(9, 26, 52, -1, 12));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToIchimoku(9, 26, 52, 12, -1));
    }
}
