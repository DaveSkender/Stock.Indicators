namespace StaticSeries;

[TestClass]
public class MaEnvelopes : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard() // SMA
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(215.0310, r1.Centerline.Round(4));
        Assert.AreEqual(220.4068, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(209.6552, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(255.5500, r2.Centerline.Round(4));
        Assert.AreEqual(261.9388, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(249.16125, r2.LowerEnvelope.Round(5));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(251.8600, r3.Centerline.Round(4));
        Assert.AreEqual(258.1565, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(245.5635, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Alma()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(10, 2.5, MaType.ALMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(216.0619, r1.Centerline.Round(4));
        Assert.AreEqual(221.4635, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(210.6604, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(257.5787, r2.Centerline.Round(4));
        Assert.AreEqual(264.0182, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(251.1393, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(242.1871, r3.Centerline.Round(4));
        Assert.AreEqual(248.2418, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(236.1324, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Dema()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.DEMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[38];
        Assert.AreEqual(223.4594, r1.Centerline.Round(4));
        Assert.AreEqual(229.0459, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(217.8730, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.4452, r2.Centerline.Round(4));
        Assert.AreEqual(264.9064, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(251.9841, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(241.1677, r3.Centerline.Round(4));
        Assert.AreEqual(247.1969, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(235.1385, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Epma()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.EPMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(216.2859, r1.Centerline.Round(4));
        Assert.AreEqual(221.6930, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(210.8787, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.5179, r2.Centerline.Round(4));
        Assert.AreEqual(264.9808, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(252.0549, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(235.8131, r3.Centerline.Round(4));
        Assert.AreEqual(241.7085, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(229.9178, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Ema()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.EMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(215.0920, r1.Centerline.Round(4));
        Assert.AreEqual(220.4693, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(209.7147, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(255.3873, r2.Centerline.Round(4));
        Assert.AreEqual(261.7719, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(249.0026, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(249.3519, r3.Centerline.Round(4));
        Assert.AreEqual(255.5857, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(243.1181, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Hma()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.HMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r2 = results[149];
        Assert.AreEqual(236.0835, r2.Centerline.Round(4));
        Assert.AreEqual(241.9856, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(230.1814, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(235.6972, r3.Centerline.Round(4));
        Assert.AreEqual(241.5897, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(229.8048, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Smma()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.SMMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(214.8433, r1.Centerline.Round(4));
        Assert.AreEqual(220.2144, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(209.4722, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(252.5574, r2.Centerline.Round(4));
        Assert.AreEqual(258.8714, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(246.2435, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(255.6746, r3.Centerline.Round(4));
        Assert.AreEqual(262.0665, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(249.2828, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Tema()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.TEMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[57];
        Assert.AreEqual(222.6349, r1.Centerline.Round(4));
        Assert.AreEqual(228.2008, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(217.0690, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.6208, r2.Centerline.Round(4));
        Assert.AreEqual(265.0863, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(252.1553, r2.LowerEnvelope.Round(4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(238.7690, r3.Centerline.Round(4));
        Assert.AreEqual(244.7382, r3.UpperEnvelope.Round(4));
        Assert.AreEqual(232.7998, r3.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void Wma()
    {
        IReadOnlyList<MaEnvelopeResult> results =
            Quotes.ToMaEnvelopes(20, 2.5, MaType.WMA);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[149];
        Assert.AreEqual(235.5253, r1.Centerline.Round(4));
        Assert.AreEqual(241.4135, r1.UpperEnvelope.Round(4));
        Assert.AreEqual(229.6372, r1.LowerEnvelope.Round(4));

        MaEnvelopeResult r2 = results[501];
        Assert.AreEqual(246.5110, r2.Centerline.Round(4));
        Assert.AreEqual(252.6738, r2.UpperEnvelope.Round(4));
        Assert.AreEqual(240.3483, r2.LowerEnvelope.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<MaEnvelopeResult> results = Quotes
            .Use(CandlePart.Close)
            .ToMaEnvelopes(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<MaEnvelopeResult> results = Quotes
            .ToSma(2)
            .ToMaEnvelopes(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<MaEnvelopeResult> a = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.ALMA);

        Assert.AreEqual(502, a.Count);

        IReadOnlyList<MaEnvelopeResult> d = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.DEMA);

        Assert.AreEqual(502, d.Count);

        IReadOnlyList<MaEnvelopeResult> p = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.EPMA);

        Assert.AreEqual(502, p.Count);

        IReadOnlyList<MaEnvelopeResult> e = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.EMA);

        Assert.AreEqual(502, e.Count);

        IReadOnlyList<MaEnvelopeResult> h = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.HMA);

        Assert.AreEqual(502, h.Count);

        IReadOnlyList<MaEnvelopeResult> s = BadQuotes
            .ToMaEnvelopes(5);

        Assert.AreEqual(502, s.Count);

        IReadOnlyList<MaEnvelopeResult> t = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.TEMA);

        Assert.AreEqual(502, t.Count);

        IReadOnlyList<MaEnvelopeResult> w = BadQuotes
            .ToMaEnvelopes(5, 2.5, MaType.WMA);

        Assert.AreEqual(502, w.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<MaEnvelopeResult> r0 = Noquotes
            .ToMaEnvelopes(10);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<MaEnvelopeResult> r1 = Onequote
            .ToMaEnvelopes(10);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<MaEnvelopeResult> results = Quotes
            .ToMaEnvelopes(20)
            .Condense();

        Assert.AreEqual(483, results.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad offset period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToMaEnvelopes(14, 0));

        // bad MA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToMaEnvelopes(14, 5, MaType.KAMA));

        // note: insufficient quotes is tested elsewhere
    }
}
