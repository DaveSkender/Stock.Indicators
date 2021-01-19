using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class PivotPoints : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            PeriodSize periodSize = PeriodSize.Month;
            PivotPointType pointType = PivotPointType.Standard;

            List<PivotPointsResult> results = Indicator.GetPivotPoints(history, periodSize, pointType)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.PP != null).Count());

            // sample values
            PivotPointsResult r1 = results[19];
            Assert.AreEqual(null, r1.R4);
            Assert.AreEqual(null, r1.R3);
            Assert.AreEqual(null, r1.PP);
            Assert.AreEqual(null, r1.S1);
            Assert.AreEqual(null, r1.S2);
            Assert.AreEqual(null, r1.R1);
            Assert.AreEqual(null, r1.R2);
            Assert.AreEqual(null, r1.S3);
            Assert.AreEqual(null, r1.S4);

            PivotPointsResult r2 = results[20];
            Assert.AreEqual(null, r2.R4);
            Assert.AreEqual(null, r2.R3);
            Assert.AreEqual(214.5000m, Math.Round((decimal)r2.PP, 4));
            Assert.AreEqual(211.9800m, Math.Round((decimal)r2.S1, 4));
            Assert.AreEqual(209.0000m, Math.Round((decimal)r2.S2, 4));
            Assert.AreEqual(217.4800m, Math.Round((decimal)r2.R1, 4));
            Assert.AreEqual(220.0000m, Math.Round((decimal)r2.R2, 4));
            Assert.AreEqual(null, r2.S3);
            Assert.AreEqual(null, r2.S4);

            PivotPointsResult r3 = results[149];
            Assert.AreEqual(null, r3.R4);
            Assert.AreEqual(null, r3.R3);
            Assert.AreEqual(233.6400m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(230.8100m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(226.3300m, Math.Round((decimal)r3.S2, 4));
            Assert.AreEqual(238.1200m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(240.9500m, Math.Round((decimal)r3.R2, 4));
            Assert.AreEqual(null, r3.S3);
            Assert.AreEqual(null, r3.S4);

            PivotPointsResult r4 = results[250];
            Assert.AreEqual(null, r4.R4);
            Assert.AreEqual(null, r4.R3);
            Assert.AreEqual(251.2767m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(247.6133m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(241.2867m, Math.Round((decimal)r4.S2, 4));
            Assert.AreEqual(257.6033m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(261.2667m, Math.Round((decimal)r4.R2, 4));
            Assert.AreEqual(null, r4.S3);
            Assert.AreEqual(null, r4.S4);

            PivotPointsResult r5 = results[251];
            Assert.AreEqual(null, r5.R4);
            Assert.AreEqual(null, r5.R3);
            Assert.AreEqual(255.1967m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(251.6933m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(246.3667m, Math.Round((decimal)r5.S2, 4));
            Assert.AreEqual(260.5233m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(264.0267m, Math.Round((decimal)r5.R2, 4));
            Assert.AreEqual(null, r5.S3);
            Assert.AreEqual(null, r5.S4);

            PivotPointsResult r6 = results[501];
            Assert.AreEqual(null, r6.R4);
            Assert.AreEqual(null, r6.R3);
            Assert.AreEqual(266.6767m, Math.Round((decimal)r6.PP, 4));
            Assert.AreEqual(258.9633m, Math.Round((decimal)r6.S1, 4));
            Assert.AreEqual(248.9667m, Math.Round((decimal)r6.S2, 4));
            Assert.AreEqual(276.6733m, Math.Round((decimal)r6.R1, 4));
            Assert.AreEqual(284.3867m, Math.Round((decimal)r6.R2, 4));
            Assert.AreEqual(null, r6.S3);
            Assert.AreEqual(null, r6.S4);
        }

        [TestMethod]
        public void Camarilla()
        {
            PeriodSize periodSize = PeriodSize.Week;
            PivotPointType pointType = PivotPointType.Camarilla;

            IEnumerable<Quote> h = HistoryTestData.Get(38);
            List<PivotPointsResult> results = Indicator.GetPivotPoints(h, periodSize, pointType)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(38, results.Count);
            Assert.AreEqual(33, results.Where(x => x.PP != null).Count());

            // sample values
            PivotPointsResult r2 = results[4];
            Assert.AreEqual(null, r2.R4);
            Assert.AreEqual(null, r2.R3);
            Assert.AreEqual(null, r2.PP);
            Assert.AreEqual(null, r2.S1);
            Assert.AreEqual(null, r2.S2);
            Assert.AreEqual(null, r2.R1);
            Assert.AreEqual(null, r2.R2);
            Assert.AreEqual(null, r2.S3);
            Assert.AreEqual(null, r2.S4);

            PivotPointsResult r3 = results[5];
            Assert.AreEqual(271.0200m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(270.13725m, Math.Round((decimal)r3.S1, 5));
            Assert.AreEqual(269.2545m, Math.Round((decimal)r3.S2, 4));
            Assert.AreEqual(268.3718m, Math.Round((decimal)r3.S3, 4));
            Assert.AreEqual(265.7235m, Math.Round((decimal)r3.S4, 4));
            Assert.AreEqual(271.9028m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(272.7855m, Math.Round((decimal)r3.R2, 4));
            Assert.AreEqual(273.66825m, Math.Round((decimal)r3.R3, 5));
            Assert.AreEqual(276.3165m, Math.Round((decimal)r3.R4, 4));

            PivotPointsResult r4 = results[22];
            Assert.AreEqual(268.9600m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(267.9819m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(267.0038m, Math.Round((decimal)r4.S2, 4));
            Assert.AreEqual(266.0258m, Math.Round((decimal)r4.S3, 4));
            Assert.AreEqual(263.0915m, Math.Round((decimal)r4.S4, 4));
            Assert.AreEqual(269.9381m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(270.9162m, Math.Round((decimal)r4.R2, 4));
            Assert.AreEqual(271.89425m, Math.Round((decimal)r4.R3, 5));
            Assert.AreEqual(274.8285m, Math.Round((decimal)r4.R4, 4));

            PivotPointsResult r5 = results[23];
            Assert.AreEqual(257.1700m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(255.5640m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(253.9580m, Math.Round((decimal)r5.S2, 4));
            Assert.AreEqual(252.3520m, Math.Round((decimal)r5.S3, 4));
            Assert.AreEqual(247.5340m, Math.Round((decimal)r5.S4, 4));
            Assert.AreEqual(258.7760m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(260.3820m, Math.Round((decimal)r5.R2, 4));
            Assert.AreEqual(261.9880m, Math.Round((decimal)r5.R3, 4));
            Assert.AreEqual(266.8060m, Math.Round((decimal)r5.R4, 4));

            PivotPointsResult r6 = results[37];
            Assert.AreEqual(243.1500m, Math.Round((decimal)r6.PP, 4));
            Assert.AreEqual(241.56325m, Math.Round((decimal)r6.S1, 5));
            Assert.AreEqual(239.9765m, Math.Round((decimal)r6.S2, 4));
            Assert.AreEqual(238.3898m, Math.Round((decimal)r6.S3, 4));
            Assert.AreEqual(233.6295m, Math.Round((decimal)r6.S4, 4));
            Assert.AreEqual(244.7368m, Math.Round((decimal)r6.R1, 4));
            Assert.AreEqual(246.3235m, Math.Round((decimal)r6.R2, 4));
            Assert.AreEqual(247.91025m, Math.Round((decimal)r6.R3, 5));
            Assert.AreEqual(252.6705m, Math.Round((decimal)r6.R4, 4));
        }

        [TestMethod]
        public void Demark()
        {
            PeriodSize periodSize = PeriodSize.Month;
            PivotPointType pointType = PivotPointType.Demark;

            List<PivotPointsResult> results = Indicator.GetPivotPoints(history, periodSize, pointType)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.PP != null).Count());

            // sample values
            PivotPointsResult r1 = results[19];
            Assert.AreEqual(null, r1.R4);
            Assert.AreEqual(null, r1.R3);
            Assert.AreEqual(null, r1.PP);
            Assert.AreEqual(null, r1.S1);
            Assert.AreEqual(null, r1.S2);
            Assert.AreEqual(null, r1.R1);
            Assert.AreEqual(null, r1.R2);
            Assert.AreEqual(null, r1.S3);
            Assert.AreEqual(null, r1.S4);

            PivotPointsResult r2 = results[20];
            Assert.AreEqual(null, r2.R4);
            Assert.AreEqual(null, r2.R3);
            Assert.AreEqual(215.1300m, Math.Round((decimal)r2.PP, 4));
            Assert.AreEqual(213.2400m, Math.Round((decimal)r2.S1, 4));
            Assert.AreEqual(null, r2.S2);
            Assert.AreEqual(218.7400m, Math.Round((decimal)r2.R1, 4));
            Assert.AreEqual(null, r2.R2);
            Assert.AreEqual(null, r2.S3);
            Assert.AreEqual(null, r2.S4);

            PivotPointsResult r3 = results[149];
            Assert.AreEqual(null, r3.R4);
            Assert.AreEqual(null, r3.R3);
            Assert.AreEqual(234.3475m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(232.2250m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(null, r3.S2);
            Assert.AreEqual(239.5350m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(null, r3.R2);
            Assert.AreEqual(null, r3.S3);
            Assert.AreEqual(null, r3.S4);

            PivotPointsResult r4 = results[250];
            Assert.AreEqual(null, r4.R4);
            Assert.AreEqual(null, r4.R3);
            Assert.AreEqual(252.1925m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(249.4450m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(null, r4.S2);
            Assert.AreEqual(259.4350m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(null, r4.R2);
            Assert.AreEqual(null, r4.S3);
            Assert.AreEqual(null, r4.S4);

            PivotPointsResult r5 = results[251];
            Assert.AreEqual(null, r5.R4);
            Assert.AreEqual(null, r5.R3);
            Assert.AreEqual(256.0725m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(253.4450m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(null, r5.S2);
            Assert.AreEqual(262.2750m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(null, r5.R2);
            Assert.AreEqual(null, r5.S3);
            Assert.AreEqual(null, r5.S4);

            PivotPointsResult r6 = results[501];
            Assert.AreEqual(null, r6.R4);
            Assert.AreEqual(null, r6.R3);
            Assert.AreEqual(268.6050m, Math.Round((decimal)r6.PP, 4));
            Assert.AreEqual(262.8200m, Math.Round((decimal)r6.S1, 4));
            Assert.AreEqual(null, r6.S2);
            Assert.AreEqual(280.5300m, Math.Round((decimal)r6.R1, 4));
            Assert.AreEqual(null, r6.R2);
            Assert.AreEqual(null, r6.S3);
            Assert.AreEqual(null, r6.S4);


            // special Demark case: test close = open
            PivotPointsResult d1 = Indicator.GetPivotPointDemark(125, 200, 100, 125);
            Assert.AreEqual(550m / 4, d1.PP);
        }

        [TestMethod]
        public void Fibonacci()
        {
            PeriodSize periodSize = PeriodSize.Hour;
            PivotPointType pointType = PivotPointType.Fibonacci;

            IEnumerable<Quote> h = HistoryTestData.GetIntraday(300);
            List<PivotPointsResult> results = Indicator.GetPivotPoints(h, periodSize, pointType)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(300, results.Count);
            Assert.AreEqual(241, results.Where(x => x.PP != null).Count());

            // sample values
            PivotPointsResult r1 = results[58];
            Assert.AreEqual(null, r1.R4);
            Assert.AreEqual(null, r1.R3);
            Assert.AreEqual(null, r1.PP);
            Assert.AreEqual(null, r1.S1);
            Assert.AreEqual(null, r1.S2);
            Assert.AreEqual(null, r1.R1);
            Assert.AreEqual(null, r1.R2);
            Assert.AreEqual(null, r1.S3);
            Assert.AreEqual(null, r1.S4);

            PivotPointsResult r2 = results[59];
            Assert.AreEqual(368.4967m, Math.Round((decimal)r2.PP, 4));
            Assert.AreEqual(367.9237m, Math.Round((decimal)r2.S1, 4));
            Assert.AreEqual(367.5697m, Math.Round((decimal)r2.S2, 4));
            Assert.AreEqual(366.9967m, Math.Round((decimal)r2.S3, 4));
            Assert.AreEqual(369.0697m, Math.Round((decimal)r2.R1, 4));
            Assert.AreEqual(369.4237m, Math.Round((decimal)r2.R2, 4));
            Assert.AreEqual(369.9967m, Math.Round((decimal)r2.R3, 4));

            PivotPointsResult r3 = results[118];
            Assert.AreEqual(368.4967m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(367.9237m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(367.5697m, Math.Round((decimal)r3.S2, 4));
            Assert.AreEqual(366.9967m, Math.Round((decimal)r3.S3, 4));
            Assert.AreEqual(369.0697m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(369.4237m, Math.Round((decimal)r3.R2, 4));
            Assert.AreEqual(369.9967m, Math.Round((decimal)r3.R3, 4));

            PivotPointsResult r4 = results[119];
            Assert.AreEqual(369.0000m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(368.5760m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(368.3140m, Math.Round((decimal)r4.S2, 4));
            Assert.AreEqual(367.8900m, Math.Round((decimal)r4.S3, 4));
            Assert.AreEqual(369.4240m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(369.6860m, Math.Round((decimal)r4.R2, 4));
            Assert.AreEqual(370.1100m, Math.Round((decimal)r4.R3, 4));

            PivotPointsResult r5 = results[149];
            Assert.AreEqual(369.0000m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(368.5760m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(368.3140m, Math.Round((decimal)r5.S2, 4));
            Assert.AreEqual(367.8900m, Math.Round((decimal)r5.S3, 4));
            Assert.AreEqual(369.4240m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(369.6860m, Math.Round((decimal)r5.R2, 4));
            Assert.AreEqual(370.1100m, Math.Round((decimal)r5.R3, 4));

            PivotPointsResult r6 = results[299];
            Assert.AreEqual(368.8200m, Math.Round((decimal)r6.PP, 4));
            Assert.AreEqual(367.5632m, Math.Round((decimal)r6.S1, 4));
            Assert.AreEqual(366.7868m, Math.Round((decimal)r6.S2, 4));
            Assert.AreEqual(365.5300m, Math.Round((decimal)r6.S3, 4));
            Assert.AreEqual(370.0768m, Math.Round((decimal)r6.R1, 4));
            Assert.AreEqual(370.8532m, Math.Round((decimal)r6.R2, 4));
            Assert.AreEqual(372.1100m, Math.Round((decimal)r6.R3, 4));
        }

        [TestMethod]
        public void Woodie()
        {
            PeriodSize periodSize = PeriodSize.Day;
            PivotPointType pointType = PivotPointType.Woodie;

            IEnumerable<Quote> h = HistoryTestData.GetIntraday();
            List<PivotPointsResult> results = Indicator.GetPivotPoints(h, periodSize, pointType)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(1564, results.Count);
            Assert.AreEqual(1173, results.Where(x => x.PP != null).Count());

            // sample values
            PivotPointsResult r2 = results[390];
            Assert.AreEqual(null, r2.R4);
            Assert.AreEqual(null, r2.R3);
            Assert.AreEqual(null, r2.PP);
            Assert.AreEqual(null, r2.S1);
            Assert.AreEqual(null, r2.S2);
            Assert.AreEqual(null, r2.R1);
            Assert.AreEqual(null, r2.R2);
            Assert.AreEqual(null, r2.S3);
            Assert.AreEqual(null, r2.S4);

            PivotPointsResult r3 = results[391];
            Assert.AreEqual(368.7875m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(367.9850m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(365.1175m, Math.Round((decimal)r3.S2, 4));
            Assert.AreEqual(364.3150m, Math.Round((decimal)r3.S3, 4));
            Assert.AreEqual(371.6550m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(372.4575m, Math.Round((decimal)r3.R2, 4));
            Assert.AreEqual(375.3250m, Math.Round((decimal)r3.R3, 4));

            PivotPointsResult r4 = results[1172];
            Assert.AreEqual(370.9769m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(370.7938m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(368.6845m, Math.Round((decimal)r4.S2, 4));
            Assert.AreEqual(368.5014m, Math.Round((decimal)r4.S3, 4));
            Assert.AreEqual(373.0862m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(373.2693m, Math.Round((decimal)r4.R2, 4));
            Assert.AreEqual(375.3786m, Math.Round((decimal)r4.R3, 4));

            PivotPointsResult r5 = results[1173];
            Assert.AreEqual(371.3625m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(370.2650m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(369.9525m, Math.Round((decimal)r5.S2, 4));
            Assert.AreEqual(368.8550m, Math.Round((decimal)r5.S3, 4));
            Assert.AreEqual(371.6750m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(372.7725m, Math.Round((decimal)r5.R2, 4));
            Assert.AreEqual(373.0850m, Math.Round((decimal)r5.R3, 4));

            PivotPointsResult r6 = results[1563];
            Assert.AreEqual(371.3625m, Math.Round((decimal)r6.PP, 4));
            Assert.AreEqual(370.2650m, Math.Round((decimal)r6.S1, 4));
            Assert.AreEqual(369.9525m, Math.Round((decimal)r6.S2, 4));
            Assert.AreEqual(368.8550m, Math.Round((decimal)r6.S3, 4));
            Assert.AreEqual(371.6750m, Math.Round((decimal)r6.R1, 4));
            Assert.AreEqual(372.7725m, Math.Round((decimal)r6.R2, 4));
            Assert.AreEqual(373.0850m, Math.Round((decimal)r6.R3, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<PivotPointsResult> r = Indicator.GetPivotPoints(historyBad, PeriodSize.Week);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // insufficient history - month
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPivotPoints(HistoryTestData.Get(18), PeriodSize.Month));

            // insufficient history - week
            IEnumerable<Quote> w = HistoryTestData.Get(5)
                .OrderBy(x => x.Date).Take(4);

            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPivotPoints(w, PeriodSize.Week));

            // insufficient history - day
            IEnumerable<Quote> d = HistoryTestData.GetIntraday(250);

            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPivotPoints(d, PeriodSize.Day));

            // insufficient history - hour
            IEnumerable<Quote> h = HistoryTestData.GetIntraday(30)
                .OrderBy(x => x.Date).Take(29);

            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetPivotPoints(h, PeriodSize.Hour));
        }
    }
}