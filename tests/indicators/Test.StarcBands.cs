using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class StarcBandsTests : TestBase
    {

        [TestMethod()]
        public void Standard()
        {
            int smaPeriod = 20;
            int multiplier = 2;
            int atrPeriod = 14;
            int lookbackPeriod = Math.Max(smaPeriod, atrPeriod);

            List<StarcBandsResult> results =
                Indicator.GetStarcBands(history, smaPeriod, multiplier, atrPeriod)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(483, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(483, results.Where(x => x.LowerBand != null).Count());

            // sample value
            StarcBandsResult r1 = results[501];
            Assert.AreEqual(251.8600m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(264.1595m, Math.Round((decimal)r1.UpperBand, 4));
            Assert.AreEqual(239.5605m, Math.Round((decimal)r1.LowerBand, 4));

            StarcBandsResult r2 = results[485];
            Assert.AreEqual(265.4855m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(275.1161m, Math.Round((decimal)r2.UpperBand, 4));
            Assert.AreEqual(255.8549m, Math.Round((decimal)r2.LowerBand, 4));

            StarcBandsResult r3 = results[249];
            Assert.AreEqual(255.5500m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(258.2261m, Math.Round((decimal)r3.UpperBand, 4));
            Assert.AreEqual(252.8739m, Math.Round((decimal)r3.LowerBand, 4));

            StarcBandsResult r4 = results[19];
            Assert.AreEqual(214.5250m, Math.Round((decimal)r4.Centerline, 4));
            Assert.AreEqual(217.2831m, Math.Round((decimal)r4.UpperBand, 4));
            Assert.AreEqual(211.7669m, Math.Round((decimal)r4.LowerBand, 4));

            StarcBandsResult r5 = results[18];
            Assert.AreEqual(null, r5.Centerline);
            Assert.AreEqual(null, r5.UpperBand);
            Assert.AreEqual(null, r5.LowerBand);
        }

        [TestMethod()]
        public void BadData()
        {
            IEnumerable<StarcBandsResult> r = Indicator.GetStarcBands(historyBad, 10, 3, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod()]
        public void Convergence()
        {
            foreach (int qty in convergeQuantities)
            {
                IEnumerable<Quote> h = History.GetHistoryLong(200 + qty);
                IEnumerable<StarcBandsResult> r = Indicator.GetStarcBands(h, 100);

                StarcBandsResult l = r.LastOrDefault();
                Console.WriteLine("STARC UPPER on {0:d} with {1,4} periods: {2:N8}",
                    l.Date, h.Count(), l.UpperBand);
            }
        }


        /* EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad EMA period.")]
        public void BadEmaPeriod()
        {
            Indicator.GetStarcBands(history, 1, 2, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad ATR period.")]
        public void BadAtrPeriod()
        {
            Indicator.GetStarcBands(history, 20, 2, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Bad multiplier.")]
        public void BadMultiplier()
        {
            Indicator.GetStarcBands(history, 20, 0, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history 120.")]
        public void InsufficientHistory120()
        {
            IEnumerable<Quote> h = History.GetHistory(119);
            Indicator.GetStarcBands(h, 120, 2, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Insufficient history 250.")]
        public void InsufficientHistory250()
        {
            IEnumerable<Quote> h = History.GetHistory(249);
            Indicator.GetStarcBands(h, 20, 2, 150);
        }

    }
}