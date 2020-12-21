using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class PivotPointsTests : TestBase
    {

        [TestMethod()]
        public void GetPivotPointsStandard()
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
            PivotPointsResult r1 = results[501];
            Assert.AreEqual(null, r1.R4);
            Assert.AreEqual(null, r1.R3);
            Assert.AreEqual(266.6767m, Math.Round((decimal)r1.PP, 4));
            Assert.AreEqual(258.9633m, Math.Round((decimal)r1.S1, 4));
            Assert.AreEqual(248.9667m, Math.Round((decimal)r1.S2, 4));
            Assert.AreEqual(276.6733m, Math.Round((decimal)r1.R1, 4));
            Assert.AreEqual(284.3867m, Math.Round((decimal)r1.R2, 4));
            Assert.AreEqual(null, r1.S3);
            Assert.AreEqual(null, r1.S4);

            PivotPointsResult r2 = results[251];
            Assert.AreEqual(null, r2.R4);
            Assert.AreEqual(null, r2.R3);
            Assert.AreEqual(255.1967m, Math.Round((decimal)r2.PP, 4));
            Assert.AreEqual(251.6933m, Math.Round((decimal)r2.S1, 4));
            Assert.AreEqual(246.3667m, Math.Round((decimal)r2.S2, 4));
            Assert.AreEqual(260.5233m, Math.Round((decimal)r2.R1, 4));
            Assert.AreEqual(264.0267m, Math.Round((decimal)r2.R2, 4));
            Assert.AreEqual(null, r2.S3);
            Assert.AreEqual(null, r2.S4);

            PivotPointsResult r3 = results[250];
            Assert.AreEqual(null, r3.R4);
            Assert.AreEqual(null, r3.R3);
            Assert.AreEqual(251.2767m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(247.6133m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(241.2867m, Math.Round((decimal)r3.S2, 4));
            Assert.AreEqual(257.6033m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(261.2667m, Math.Round((decimal)r3.R2, 4));
            Assert.AreEqual(null, r3.S3);
            Assert.AreEqual(null, r3.S4);

            PivotPointsResult r4 = results[149];
            Assert.AreEqual(null, r4.R4);
            Assert.AreEqual(null, r4.R3);
            Assert.AreEqual(233.6400m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(230.8100m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(226.3300m, Math.Round((decimal)r4.S2, 4));
            Assert.AreEqual(238.1200m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(240.9500m, Math.Round((decimal)r4.R2, 4));
            Assert.AreEqual(null, r4.S3);
            Assert.AreEqual(null, r4.S4);

            PivotPointsResult r5 = results[20];
            Assert.AreEqual(null, r5.R4);
            Assert.AreEqual(null, r5.R3);
            Assert.AreEqual(214.5000m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(211.9800m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(209.0000m, Math.Round((decimal)r5.S2, 4));
            Assert.AreEqual(217.4800m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(220.0000m, Math.Round((decimal)r5.R2, 4));
            Assert.AreEqual(null, r5.S3);
            Assert.AreEqual(null, r5.S4);

            PivotPointsResult r6 = results[19];
            Assert.AreEqual(null, r6.R4);
            Assert.AreEqual(null, r6.R3);
            Assert.AreEqual(null, r6.PP);
            Assert.AreEqual(null, r6.S1);
            Assert.AreEqual(null, r6.S2);
            Assert.AreEqual(null, r6.R1);
            Assert.AreEqual(null, r6.R2);
            Assert.AreEqual(null, r6.S3);
            Assert.AreEqual(null, r6.S4);
        }


        [TestMethod()]
        public void GetPivotPointsCamarilla()
        {
            PeriodSize periodSize = PeriodSize.Week;
            PivotPointType pointType = PivotPointType.Camarilla;

            IEnumerable<Quote> h = History.GetHistory(38);
            List<PivotPointsResult> results = Indicator.GetPivotPoints(h, periodSize, pointType)
                .ToList();

            foreach (PivotPointsResult r in results)
            {
                Console.WriteLine("{0:d},{1:N4},{2:N4},{3:N4},{4:N4},{5:N4},{6:N4},{7:N4},{8:N4},{9:N4}",
                    r.Date, r.PP, r.S1, r.S2, r.S3, r.S4, r.R1, r.R2, r.R3, r.R4);
            }

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(38, results.Count);
            Assert.AreEqual(33, results.Where(x => x.PP != null).Count());

            // sample values
            PivotPointsResult r1 = results[37];
            Assert.AreEqual(239.7667m, Math.Round((decimal)r1.PP, 4));
            Assert.AreEqual(224.3975m, Math.Round((decimal)r1.S1, 4));
            Assert.AreEqual(222.9550m, Math.Round((decimal)r1.S2, 4));
            Assert.AreEqual(221.5125m, Math.Round((decimal)r1.S3, 4));
            Assert.AreEqual(217.1850m, Math.Round((decimal)r1.S4, 4));
            Assert.AreEqual(261.9025m, Math.Round((decimal)r1.R1, 4));
            Assert.AreEqual(263.3450m, Math.Round((decimal)r1.R2, 4));
            Assert.AreEqual(264.7875m, Math.Round((decimal)r1.R3, 4));
            Assert.AreEqual(269.1150m, Math.Round((decimal)r1.R4, 4));

            PivotPointsResult r2 = results[23];
            Assert.AreEqual(262.2767m, Math.Round((decimal)r2.PP, 4));
            Assert.AreEqual(238.1900m, Math.Round((decimal)r2.S1, 4));
            Assert.AreEqual(236.7300m, Math.Round((decimal)r2.S2, 4));
            Assert.AreEqual(235.2700m, Math.Round((decimal)r2.S3, 4));
            Assert.AreEqual(230.8900m, Math.Round((decimal)r2.S4, 4));
            Assert.AreEqual(276.1500m, Math.Round((decimal)r2.R1, 4));
            Assert.AreEqual(277.6100m, Math.Round((decimal)r2.R2, 4));
            Assert.AreEqual(279.0700m, Math.Round((decimal)r2.R3, 4));
            Assert.AreEqual(283.4500m, Math.Round((decimal)r2.R4, 4));

            PivotPointsResult r3 = results[22];
            Assert.AreEqual(265.8100m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(257.4008m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(256.5117m, Math.Round((decimal)r3.S2, 4));
            Assert.AreEqual(255.6225m, Math.Round((decimal)r3.S3, 4));
            Assert.AreEqual(252.9550m, Math.Round((decimal)r3.S4, 4));
            Assert.AreEqual(280.5192m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(281.4083m, Math.Round((decimal)r3.R2, 4));
            Assert.AreEqual(282.2975m, Math.Round((decimal)r3.R3, 4));
            Assert.AreEqual(284.9650m, Math.Round((decimal)r3.R4, 4));

            PivotPointsResult r4 = results[5];
            Assert.AreEqual(270.0567m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(260.5875m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(259.7850m, Math.Round((decimal)r4.S2, 4));
            Assert.AreEqual(258.9825m, Math.Round((decimal)r4.S3, 4));
            Assert.AreEqual(256.5750m, Math.Round((decimal)r4.S4, 4));
            Assert.AreEqual(281.4525m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(282.2550m, Math.Round((decimal)r4.R2, 4));
            Assert.AreEqual(283.0575m, Math.Round((decimal)r4.R3, 4));
            Assert.AreEqual(285.4650m, Math.Round((decimal)r4.R4, 4));

            PivotPointsResult r5 = results[4];
            Assert.AreEqual(null, r5.R4);
            Assert.AreEqual(null, r5.R3);
            Assert.AreEqual(null, r5.PP);
            Assert.AreEqual(null, r5.S1);
            Assert.AreEqual(null, r5.S2);
            Assert.AreEqual(null, r5.R1);
            Assert.AreEqual(null, r5.R2);
            Assert.AreEqual(null, r5.S3);
            Assert.AreEqual(null, r5.S4);
        }


        [TestMethod()]
        public void GetPivotPointsDemark()
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
            PivotPointsResult r1 = results[501];
            Assert.AreEqual(null, r1.R4);
            Assert.AreEqual(null, r1.R3);
            Assert.AreEqual(268.6050m, Math.Round((decimal)r1.PP, 4));
            Assert.AreEqual(262.8200m, Math.Round((decimal)r1.S1, 4));
            Assert.AreEqual(null, r1.S2);
            Assert.AreEqual(280.5300m, Math.Round((decimal)r1.R1, 4));
            Assert.AreEqual(null, r1.R2);
            Assert.AreEqual(null, r1.S3);
            Assert.AreEqual(null, r1.S4);

            PivotPointsResult r2 = results[251];
            Assert.AreEqual(null, r2.R4);
            Assert.AreEqual(null, r2.R3);
            Assert.AreEqual(256.0725m, Math.Round((decimal)r2.PP, 4));
            Assert.AreEqual(253.4450m, Math.Round((decimal)r2.S1, 4));
            Assert.AreEqual(null, r2.S2);
            Assert.AreEqual(262.2750m, Math.Round((decimal)r2.R1, 4));
            Assert.AreEqual(null, r2.R2);
            Assert.AreEqual(null, r2.S3);
            Assert.AreEqual(null, r2.S4);

            PivotPointsResult r3 = results[250];
            Assert.AreEqual(null, r3.R4);
            Assert.AreEqual(null, r3.R3);
            Assert.AreEqual(252.1925m, Math.Round((decimal)r3.PP, 4));
            Assert.AreEqual(249.4450m, Math.Round((decimal)r3.S1, 4));
            Assert.AreEqual(null, r3.S2);
            Assert.AreEqual(259.4350m, Math.Round((decimal)r3.R1, 4));
            Assert.AreEqual(null, r3.R2);
            Assert.AreEqual(null, r3.S3);
            Assert.AreEqual(null, r3.S4);

            PivotPointsResult r4 = results[149];
            Assert.AreEqual(null, r4.R4);
            Assert.AreEqual(null, r4.R3);
            Assert.AreEqual(234.3475m, Math.Round((decimal)r4.PP, 4));
            Assert.AreEqual(232.2250m, Math.Round((decimal)r4.S1, 4));
            Assert.AreEqual(null, r4.S2);
            Assert.AreEqual(239.5350m, Math.Round((decimal)r4.R1, 4));
            Assert.AreEqual(null, r4.R2);
            Assert.AreEqual(null, r4.S3);
            Assert.AreEqual(null, r4.S4);

            PivotPointsResult r5 = results[20];
            Assert.AreEqual(null, r5.R4);
            Assert.AreEqual(null, r5.R3);
            Assert.AreEqual(215.1300m, Math.Round((decimal)r5.PP, 4));
            Assert.AreEqual(213.2400m, Math.Round((decimal)r5.S1, 4));
            Assert.AreEqual(null, r5.S2);
            Assert.AreEqual(218.7400m, Math.Round((decimal)r5.R1, 4));
            Assert.AreEqual(null, r5.R2);
            Assert.AreEqual(null, r5.S3);
            Assert.AreEqual(null, r5.S4);

            PivotPointsResult r6 = results[19];
            Assert.AreEqual(null, r6.R4);
            Assert.AreEqual(null, r6.R3);
            Assert.AreEqual(null, r6.PP);
            Assert.AreEqual(null, r6.S1);
            Assert.AreEqual(null, r6.S2);
            Assert.AreEqual(null, r6.R1);
            Assert.AreEqual(null, r6.R2);
            Assert.AreEqual(null, r6.S3);
            Assert.AreEqual(null, r6.S4);
        }




        [TestMethod()]
        public void GetPivotPointsBadData()
        {
            IEnumerable<PivotPointsResult> r = Indicator.GetPivotPoints(historyBad, PeriodSize.Week);
            Assert.AreEqual(502, r.Count());
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient monthly history.")]
        public void InsufficientHistoryMonth()
        {
            IEnumerable<Quote> h = History.GetHistory(18);
            Indicator.GetPivotPoints(h, PeriodSize.Month);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient weekly history.")]
        public void InsufficientHistoryWeek()
        {
            IEnumerable<Quote> h = History.GetHistory(5).OrderBy(x => x.Date).Take(4);
            Indicator.GetPivotPoints(h, PeriodSize.Week);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient daily history.")]
        public void InsufficientHistoryDay()
        {
            IEnumerable<Quote> h = History.GetHistoryIntraday(250);
            Indicator.GetPivotPoints(h, PeriodSize.Day);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient hourly history.")]
        public void InsufficientHistoryHour()
        {
            IEnumerable<Quote> h = History.GetHistoryIntraday(30).OrderBy(x => x.Date).Take(29);
            Indicator.GetPivotPoints(h, PeriodSize.Hour);
        }
    }
}