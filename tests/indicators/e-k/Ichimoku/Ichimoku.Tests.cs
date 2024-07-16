namespace Series;

[TestClass]
public class IchimokuTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int tenkanPeriods = 9;
        int kijunPeriods = 26;
        int senkouBPeriods = 52;

        List<IchimokuResult> results = Quotes
            .GetIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(494, results.Count(x => x.TenkanSen != null));
        Assert.AreEqual(477, results.Count(x => x.KijunSen != null));
        Assert.AreEqual(451, results.Count(x => x.SenkouSpanA != null));
        Assert.AreEqual(425, results.Count(x => x.SenkouSpanB != null));
        Assert.AreEqual(476, results.Count(x => x.ChikouSpan != null));

        // sample values
        IchimokuResult r1 = results[51];
        Assert.AreEqual(224.465m, r1.TenkanSen);
        Assert.AreEqual(221.94m, r1.KijunSen);
        Assert.AreEqual(214.8325m, r1.SenkouSpanA);
        Assert.AreEqual(null, r1.SenkouSpanB);
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
        Assert.AreEqual(null, r4.ChikouSpan);
    }

    [TestMethod]
    public void Extended()
    {
        List<IchimokuResult> r = Quotes
            .GetIchimoku(3, 13, 40, 0, 0)
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void BadData()
    {
        List<IchimokuResult> r = BadQuotes
            .GetIchimoku()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<IchimokuResult> r0 = Noquotes
            .GetIchimoku()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<IchimokuResult> r1 = Onequote
            .GetIchimoku()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<IchimokuResult> r = Quotes
            .GetIchimoku()
            .Condense()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetIchimoku(0));

        // bad short span period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetIchimoku(9, 0));

        // bad long span period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetIchimoku(9, 26, 26));

        // invalid offsets
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetIchimoku(9, 26, 52, -1));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetIchimoku(9, 26, 52, -1, 12));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetIchimoku(9, 26, 52, 12, -1));
    }
}
