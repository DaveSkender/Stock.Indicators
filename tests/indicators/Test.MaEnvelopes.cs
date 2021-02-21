using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class MaEnvelopes : TestBase
    {

        [TestMethod]
        public void Alma()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 10, 2.5, MaType.ALMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(493, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[24];
            Assert.AreEqual(216.0619m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(221.4635m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(210.6604m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[249];
            Assert.AreEqual(257.5787m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(264.0182m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(251.1393m, Math.Round((decimal)r2.LowerEnvelope, 4));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(242.1871m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(248.2418m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(236.1324m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Dema()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.DEMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(464, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[38];
            Assert.AreEqual(224.1033m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(229.7059m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(218.5008m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[249];
            Assert.AreEqual(258.4452m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(264.9064m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(251.9841m, Math.Round((decimal)r2.LowerEnvelope, 4));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(241.1677m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(247.1969m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(235.1385m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Epma()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.EPMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[24];
            Assert.AreEqual(216.2859m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(221.6930m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(210.8787m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[249];
            Assert.AreEqual(258.5179m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(264.9808m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(252.0549m, Math.Round((decimal)r2.LowerEnvelope, 4));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(235.8131m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(241.7085m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(229.9178m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Ema()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.EMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[24];
            Assert.AreEqual(215.0920m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(220.4693m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(209.7147m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[249];
            Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(261.7719m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(249.0026m, Math.Round((decimal)r2.LowerEnvelope, 4));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(249.3519m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(255.5857m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(243.1181m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Hma()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.HMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(480, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r2 = results[149];
            Assert.AreEqual(236.0835m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(241.9856m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(230.1814m, Math.Round((decimal)r2.LowerEnvelope, 4));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(235.6972m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(241.5897m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(229.8048m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Sma()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.SMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[24];
            Assert.AreEqual(215.0310m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(220.4068m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(209.6552m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[249];
            Assert.AreEqual(255.5500m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(261.9388m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(249.16125m, Math.Round((decimal)r2.LowerEnvelope, 5));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(251.8600m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(258.1565m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(245.5635m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Tema()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.TEMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(445, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[57];
            Assert.AreEqual(222.6022m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(228.1673m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(217.0372m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[249];
            Assert.AreEqual(258.6208m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(265.0863m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(252.1553m, Math.Round((decimal)r2.LowerEnvelope, 4));

            MaEnvelopeResult r3 = results[501];
            Assert.AreEqual(238.7690m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(244.7382m, Math.Round((decimal)r3.UpperEnvelope, 4));
            Assert.AreEqual(232.7998m, Math.Round((decimal)r3.LowerEnvelope, 4));
        }

        [TestMethod]
        public void Wma()
        {

            List<MaEnvelopeResult> results =
                Indicator.GetMaEnvelopes(history, 20, 2.5, MaType.WMA)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());

            // sample values
            MaEnvelopeResult r1 = results[149];
            Assert.AreEqual(235.5253m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(241.4135m, Math.Round((decimal)r1.UpperEnvelope, 4));
            Assert.AreEqual(229.6372m, Math.Round((decimal)r1.LowerEnvelope, 4));

            MaEnvelopeResult r2 = results[501];
            Assert.AreEqual(246.5110m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(252.6738m, Math.Round((decimal)r2.UpperEnvelope, 4));
            Assert.AreEqual(240.3483m, Math.Round((decimal)r2.LowerEnvelope, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<MaEnvelopeResult> a = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.ALMA);
            Assert.AreEqual(502, a.Count());

            IEnumerable<MaEnvelopeResult> d = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.DEMA);
            Assert.AreEqual(502, d.Count());

            IEnumerable<MaEnvelopeResult> p = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.EPMA);
            Assert.AreEqual(502, p.Count());

            IEnumerable<MaEnvelopeResult> e = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.EMA);
            Assert.AreEqual(502, e.Count());

            IEnumerable<MaEnvelopeResult> h = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.HMA);
            Assert.AreEqual(502, h.Count());

            IEnumerable<MaEnvelopeResult> s = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.SMA);
            Assert.AreEqual(502, s.Count());

            IEnumerable<MaEnvelopeResult> t = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.TEMA);
            Assert.AreEqual(502, t.Count());

            IEnumerable<MaEnvelopeResult> w = Indicator.GetMaEnvelopes(historyBad, 5, 2.5, MaType.WMA);
            Assert.AreEqual(502, w.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad offset period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMaEnvelopes(history, 14, 0));

            // bad MA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMaEnvelopes(history, 14, 5, MaType.KAMA));

            // note: insufficient history is tested elsewhere
        }
    }
}
