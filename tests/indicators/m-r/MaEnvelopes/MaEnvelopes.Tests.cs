using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class MaEnvelopes : TestBase
{
    [TestMethod]
    public void Alma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(10, 2.5, MaType.ALMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(216.0619m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(221.4635m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(210.6604m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(257.5787m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(264.0182m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(251.1393m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(242.1871m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(248.2418m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(236.1324m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Dema()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.DEMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[38];
        Assert.AreEqual(223.4594m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(229.0459m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(217.8730m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.4452m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(264.9064m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(251.9841m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(241.1677m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(247.1969m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(235.1385m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Epma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.EPMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(216.2859m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(221.6930m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(210.8787m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.5179m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(264.9808m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(252.0549m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(235.8131m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(241.7085m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(229.9178m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Ema()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.EMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(215.0920m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(220.4693m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(209.7147m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(255.3873m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(261.7719m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(249.0026m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(249.3519m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(255.5857m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(243.1181m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Hma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.HMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r2 = results[149];
        Assert.AreEqual(236.0835m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(241.9856m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(230.1814m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(235.6972m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(241.5897m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(229.8048m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Sma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.SMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(215.0310m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(220.4068m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(209.6552m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(255.5500m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(261.9388m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(249.16125m, NullMath.Round(r2.LowerEnvelope, 5));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(251.8600m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(258.1565m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(245.5635m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Smma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.SMMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(214.8433m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(220.2144m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(209.4722m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(252.5574m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(258.8714m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(246.2435m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(255.6746m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(262.0665m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(249.2828m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Tema()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.TEMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[57];
        Assert.AreEqual(222.6349m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(228.2008m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(217.0690m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.6208m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(265.0863m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(252.1553m, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(238.7690m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(244.7382m, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(232.7998m, NullMath.Round(r3.LowerEnvelope, 4));
    }

    [TestMethod]
    public void Wma()
    {
        List<MaEnvelopeResult> results =
            quotes.GetMaEnvelopes(20, 2.5, MaType.WMA)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

        // sample values
        MaEnvelopeResult r1 = results[149];
        Assert.AreEqual(235.5253m, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(241.4135m, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(229.6372m, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[501];
        Assert.AreEqual(246.5110m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(252.6738m, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(240.3483m, NullMath.Round(r2.LowerEnvelope, 4));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<MaEnvelopeResult> a = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.ALMA);
        Assert.AreEqual(502, a.Count());

        IEnumerable<MaEnvelopeResult> d = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.DEMA);
        Assert.AreEqual(502, d.Count());

        IEnumerable<MaEnvelopeResult> p = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.EPMA);
        Assert.AreEqual(502, p.Count());

        IEnumerable<MaEnvelopeResult> e = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.EMA);
        Assert.AreEqual(502, e.Count());

        IEnumerable<MaEnvelopeResult> h = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.HMA);
        Assert.AreEqual(502, h.Count());

        IEnumerable<MaEnvelopeResult> s = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.SMA);
        Assert.AreEqual(502, s.Count());

        IEnumerable<MaEnvelopeResult> t = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.TEMA);
        Assert.AreEqual(502, t.Count());

        IEnumerable<MaEnvelopeResult> w = Indicator.GetMaEnvelopes(badQuotes, 5, 2.5, MaType.WMA);
        Assert.AreEqual(502, w.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad offset period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMaEnvelopes(quotes, 14, 0));

        // bad MA period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetMaEnvelopes(quotes, 14, 5, MaType.KAMA));

        // note: insufficient quotes is tested elsewhere
    }
}
