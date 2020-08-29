using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class ZigZagTests : TestBase
    {

        [TestMethod()]
        public void GetZigZagClose()
        {
            decimal percentChange = 3;
            IEnumerable<ZigZagResult> results = Indicator.GetZigZag(history, ZigZagType.Close, percentChange);

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(234, results.Where(x => x.ZigZag != null).Count());
            //Assert.AreEqual(234, results.Where(x => x.RetraceHigh != null).Count());
            //Assert.AreEqual(221, results.Where(x => x.RetraceLow != null).Count());
            Assert.AreEqual(14, results.Where(x => x.PointType != null).Count());

            // sample values
            ZigZagResult r0 = results.Where(x => x.Index == 250).FirstOrDefault();
            Assert.AreEqual(null, r0.ZigZag);
            Assert.AreEqual(null, r0.RetraceHigh);
            Assert.AreEqual(null, r0.RetraceLow);
            Assert.AreEqual(null, r0.PointType);

            ZigZagResult r1 = results.Where(x => x.Index == 278).FirstOrDefault();
            Assert.AreEqual(248.13m, r1.ZigZag);
            //Assert.AreEqual(272.248m, r1.RetraceHigh);
            //Assert.AreEqual(248.13m, r1.RetraceLow);
            Assert.AreEqual("L", r1.PointType);

            ZigZagResult r2 = results.Where(x => x.Index == 484).FirstOrDefault();
            Assert.AreEqual(272.52m, r2.ZigZag);
            //Assert.AreEqual(272.52m, r2.RetraceHigh);
            //Assert.AreEqual(248.799m, r2.RetraceLow);
            Assert.AreEqual("H", r2.PointType);

            ZigZagResult r3 = results.Where(x => x.Index == 440).FirstOrDefault();
            Assert.AreEqual(276.0133m, Math.Round((decimal)r3.ZigZag, 4));
            //Assert.AreEqual(280.9158m, Math.Round((decimal)r3.RetraceHigh, 4));
            //Assert.AreEqual(264.5769m, Math.Round((decimal)r3.RetraceLow, 4));
            Assert.AreEqual(null, r3.PointType);

            ZigZagResult r4 = results.Where(x => x.Index == 501).FirstOrDefault();
            Assert.AreEqual(241.4575m, Math.Round((decimal)r4.ZigZag, 4));
            //Assert.AreEqual(246.7933m, Math.Round((decimal)r4.RetraceHigh, 4));
            //Assert.AreEqual(null, r4.RetraceLow);
            Assert.AreEqual(null, r4.PointType);

            ZigZagResult r5 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(245.28m, r5.ZigZag);
            //Assert.AreEqual(245.28m, r5.RetraceHigh);
            //Assert.AreEqual(null, r5.RetraceLow);
            Assert.AreEqual(null, r5.PointType);

        }

        [TestMethod()]
        public void GetZigZagHighLow()
        {
            decimal percentChange = 3;
            IEnumerable<ZigZagResult> results = Indicator.GetZigZag(history, ZigZagType.HighLow, percentChange);

            foreach (ZigZagResult r in results
                //.Where(x => x.Index >= 490)
                )
            {
                Console.WriteLine("{0},{1:N4},{2:N4},{3:N4},{4}",
                    r.Index, r.ZigZag, r.RetraceHigh, r.RetraceLow, r.PointType);
            }

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count());
            Assert.AreEqual(463, results.Where(x => x.ZigZag != null).Count());
            //Assert.AreEqual(462, results.Where(x => x.RetraceHigh != null).Count());
            //Assert.AreEqual(445, results.Where(x => x.RetraceLow != null).Count());
            Assert.AreEqual(31, results.Where(x => x.PointType != null).Count());

            // sample values
            ZigZagResult r0 = results.Where(x => x.Index == 39).FirstOrDefault();
            Assert.AreEqual(null, r0.ZigZag);
            Assert.AreEqual(null, r0.RetraceHigh);
            Assert.AreEqual(null, r0.RetraceLow);
            Assert.AreEqual(null, r0.PointType);

            ZigZagResult r1 = results.Where(x => x.Index == 278).FirstOrDefault();
            Assert.AreEqual(252.9550m, r1.ZigZag);
            //Assert.AreEqual(262.8054m, Math.Round(r1.RetraceHigh,4));
            //Assert.AreEqual(245.4467m, Math.Round(r1.RetraceLow,4));
            Assert.AreEqual(null, r1.PointType);

            ZigZagResult r2 = results.Where(x => x.Index == 317).FirstOrDefault();
            Assert.AreEqual(249.48m, r2.ZigZag);
            //Assert.AreEqual(258.34m, r2.RetraceHigh);
            //Assert.AreEqual(249.48m, r2.RetraceLow);
            Assert.AreEqual("L", r2.PointType);

            ZigZagResult r3 = results.Where(x => x.Index == 457).FirstOrDefault();
            Assert.AreEqual(261.3325m, Math.Round((decimal)r3.ZigZag, 4));
            //Assert.AreEqual(274.3419m, Math.Round((decimal)r3.RetraceHigh, 4));
            //Assert.AreEqual(256.1050m, Math.Round((decimal)r3.RetraceLow, 4));
            Assert.AreEqual(null, r3.PointType);

            ZigZagResult r4 = results.Where(x => x.Index == 501).FirstOrDefault();
            Assert.AreEqual(246.73m, Math.Round((decimal)r4.ZigZag, 4));
            //Assert.AreEqual(246.73m, r4.RetraceHigh);
            //Assert.AreEqual(238.3867m, Math.Round((decimal)r4.RetraceLow, 4));
            Assert.AreEqual("H", r4.PointType);

            ZigZagResult r5 = results.Where(x => x.Index == 502).FirstOrDefault();
            Assert.AreEqual(242.87m, r5.ZigZag);
            //Assert.AreEqual(null, r5.RetraceHigh);
            //Assert.AreEqual(242.87m, r5.RetraceLow);
            Assert.AreEqual(null, r5.PointType);

        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadParameterException), "Bad lookback.")]
        public void BadLookback()
        {
            Indicator.GetZigZag(history, ZigZagType.Close, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history.")]
        public void InsufficientHistory()
        {
            Indicator.GetZigZag(history.Where(x => x.Index < 2));
        }
    }
}