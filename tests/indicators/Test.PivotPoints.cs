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