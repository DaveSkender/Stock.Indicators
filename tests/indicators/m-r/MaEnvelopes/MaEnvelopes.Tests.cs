using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class MaEnvelopesTests : TestBase
{
    [TestMethod]
    public void Alma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(10, 2.5, MaType.ALMA)
            .ToList();

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
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.DEMA)
            .ToList();

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
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.EPMA)
            .ToList();

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
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.EMA)
            .ToList();

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
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.HMA)
            .ToList();

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
    public void Sma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.SMA)
            .ToList();

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
    public void Smma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.SMMA)
            .ToList();

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
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.TEMA)
            .ToList();

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
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.WMA)
            .ToList();

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
    public void UseTuple()
    {
        List<MaEnvelopeResult> results = quotes
            .Use(CandlePart.Close)
            .GetMaEnvelopes(10, 2.5, MaType.SMA)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<MaEnvelopeResult> r = tupleNanny
            .GetMaEnvelopes(8, 2.5, MaType.ALMA)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.UpperEnvelope is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<MaEnvelopeResult> results = quotes
            .GetSma(2)
            .GetMaEnvelopes(10, 2.5, MaType.SMA)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<MaEnvelopeResult> a = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.ALMA)
            .ToList();

        Assert.AreEqual(502, a.Count);

        List<MaEnvelopeResult> d = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.DEMA)
            .ToList();

        Assert.AreEqual(502, d.Count);

        List<MaEnvelopeResult> p = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.EPMA)
            .ToList();

        Assert.AreEqual(502, p.Count);

        List<MaEnvelopeResult> e = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.EMA)
            .ToList();

        Assert.AreEqual(502, e.Count);

        List<MaEnvelopeResult> h = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.HMA)
            .ToList();

        Assert.AreEqual(502, h.Count);

        List<MaEnvelopeResult> s = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.SMA)
            .ToList();

        Assert.AreEqual(502, s.Count);

        List<MaEnvelopeResult> t = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.TEMA)
            .ToList();

        Assert.AreEqual(502, t.Count);

        List<MaEnvelopeResult> w = badQuotes
            .GetMaEnvelopes(5, 2.5, MaType.WMA)
            .ToList();

        Assert.AreEqual(502, w.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<MaEnvelopeResult> r = quotes
            .GetMaEnvelopes(20, 2.5, MaType.SMA)
            .Condense()
            .ToList();

        Assert.AreEqual(483, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad offset period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMaEnvelopes(14, 0));

        // bad MA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetMaEnvelopes(14, 5, MaType.KAMA));

        // note: insufficient quotes is tested elsewhere
    }
}
