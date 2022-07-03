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
        Assert.AreEqual(493, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(216.0619, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(221.4635, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(210.6604, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(257.5787, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(264.0182, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(251.1393, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(242.1871, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(248.2418, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(236.1324, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[38];
        Assert.AreEqual(223.4594, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(229.0459, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(217.8730, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.4452, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(264.9064, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(251.9841, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(241.1677, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(247.1969, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(235.1385, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(216.2859, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(221.6930, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(210.8787, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.5179, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(264.9808, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(252.0549, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(235.8131, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(241.7085, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(229.9178, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(215.0920, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(220.4693, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(209.7147, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(255.3873, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(261.7719, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(249.0026, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(249.3519, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(255.5857, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(243.1181, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(480, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r2 = results[149];
        Assert.AreEqual(236.0835, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(241.9856, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(230.1814, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(235.6972, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(241.5897, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(229.8048, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(215.0310, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(220.4068, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(209.6552, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(255.5500, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(261.9388, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(249.16125, NullMath.Round(r2.LowerEnvelope, 5));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(251.8600, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(258.1565, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(245.5635, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[24];
        Assert.AreEqual(214.8433, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(220.2144, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(209.4722, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(252.5574, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(258.8714, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(246.2435, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(255.6746, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(262.0665, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(249.2828, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[57];
        Assert.AreEqual(222.6349, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(228.2008, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(217.0690, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[249];
        Assert.AreEqual(258.6208, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(265.0863, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(252.1553, NullMath.Round(r2.LowerEnvelope, 4));

        MaEnvelopeResult r3 = results[501];
        Assert.AreEqual(238.7690, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(244.7382, NullMath.Round(r3.UpperEnvelope, 4));
        Assert.AreEqual(232.7998, NullMath.Round(r3.LowerEnvelope, 4));
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
        Assert.AreEqual(483, results.Count(x => x.Centerline != null));

        // sample values
        MaEnvelopeResult r1 = results[149];
        Assert.AreEqual(235.5253, NullMath.Round(r1.Centerline, 4));
        Assert.AreEqual(241.4135, NullMath.Round(r1.UpperEnvelope, 4));
        Assert.AreEqual(229.6372, NullMath.Round(r1.LowerEnvelope, 4));

        MaEnvelopeResult r2 = results[501];
        Assert.AreEqual(246.5110, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(252.6738, NullMath.Round(r2.UpperEnvelope, 4));
        Assert.AreEqual(240.3483, NullMath.Round(r2.LowerEnvelope, 4));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<MaEnvelopeResult> results = quotes
            .Use(CandlePart.Close)
            .GetMaEnvelopes(10, 2.5, MaType.SMA);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(493, results.Count(x => x.Centerline != null));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<MaEnvelopeResult> r
            = tupleNanny.GetMaEnvelopes(8, 2.5, MaType.ALMA);

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.UpperEnvelope is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<MaEnvelopeResult> results = quotes
            .GetSma(2)
            .GetMaEnvelopes(10, 2.5, MaType.SMA);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(492, results.Count(x => x.Centerline != null));
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
    public void Condense()
    {
        IEnumerable<MaEnvelopeResult> r = quotes.GetMaEnvelopes(20, 2.5, MaType.SMA)
            .Condense();

        Assert.AreEqual(483, r.Count());
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
