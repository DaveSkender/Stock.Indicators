using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class StdDevChannels : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            int lookbackPeriod = 20;
            decimal standardDeviations = 2;

            List<StdDevChannelsResult> results =
                Indicator.GetStdDevChannels(history, lookbackPeriod, standardDeviations)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(500, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(500, results.Where(x => x.UpperChannel != null).Count());
            Assert.AreEqual(500, results.Where(x => x.LowerChannel != null).Count());

            // sample value
            StdDevChannelsResult r1 = results[1];
            Assert.IsNull(r1.Centerline);
            Assert.IsNull(r1.UpperChannel);
            Assert.IsNull(r1.LowerChannel);
            Assert.IsFalse(r1.BreakPoint);

            StdDevChannelsResult r2 = results[2];
            Assert.AreEqual(213.7993m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(215.7098m, Math.Round((decimal)r2.UpperChannel, 4));
            Assert.AreEqual(211.8888m, Math.Round((decimal)r2.LowerChannel, 4));
            Assert.IsTrue(r2.BreakPoint);

            StdDevChannelsResult r3 = results[141];
            Assert.AreEqual(236.1744m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(240.4784m, Math.Round((decimal)r3.UpperChannel, 4));
            Assert.AreEqual(231.8704m, Math.Round((decimal)r3.LowerChannel, 4));
            Assert.IsFalse(r3.BreakPoint);

            StdDevChannelsResult r4 = results[142];
            Assert.AreEqual(236.3269m, Math.Round((decimal)r4.Centerline, 4));
            Assert.AreEqual(239.5585m, Math.Round((decimal)r4.UpperChannel, 4));
            Assert.AreEqual(233.0953m, Math.Round((decimal)r4.LowerChannel, 4));
            Assert.IsTrue(r4.BreakPoint);

            StdDevChannelsResult r5 = results[249];
            Assert.AreEqual(259.6044m, Math.Round((decimal)r5.Centerline, 4));
            Assert.AreEqual(267.5754m, Math.Round((decimal)r5.UpperChannel, 4));
            Assert.AreEqual(251.6333m, Math.Round((decimal)r5.LowerChannel, 4));
            Assert.IsFalse(r5.BreakPoint);

            StdDevChannelsResult r6 = results[482];
            Assert.AreEqual(267.9069m, Math.Round((decimal)r6.Centerline, 4));
            Assert.AreEqual(289.7473m, Math.Round((decimal)r6.UpperChannel, 4));
            Assert.AreEqual(246.0664m, Math.Round((decimal)r6.LowerChannel, 4));
            Assert.IsTrue(r6.BreakPoint);

            StdDevChannelsResult r7 = results[501];
            Assert.AreEqual(235.8131m, Math.Round((decimal)r7.Centerline, 4));
            Assert.AreEqual(257.6536m, Math.Round((decimal)r7.UpperChannel, 4));
            Assert.AreEqual(213.9727m, Math.Round((decimal)r7.LowerChannel, 4));
            Assert.IsFalse(r7.BreakPoint);
        }

        [TestMethod]
        public void FullHistory()
        {
            // null provided for lookback period

            List<StdDevChannelsResult> results =
                Indicator.GetStdDevChannels(history, null, 2)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502, results.Where(x => x.Centerline != null).Count());
            Assert.AreEqual(502, results.Where(x => x.UpperChannel != null).Count());
            Assert.AreEqual(502, results.Where(x => x.LowerChannel != null).Count());
            Assert.AreEqual(501, results.Where(x => x.BreakPoint == false).Count());

            // sample value
            StdDevChannelsResult r1 = results[0];
            Assert.AreEqual(219.2605m, Math.Round((decimal)r1.Centerline, 4));
            Assert.AreEqual(258.7104m, Math.Round((decimal)r1.UpperChannel, 4));
            Assert.AreEqual(179.8105m, Math.Round((decimal)r1.LowerChannel, 4));
            Assert.IsTrue(r1.BreakPoint);

            StdDevChannelsResult r2 = results[249];
            Assert.AreEqual(249.3814m, Math.Round((decimal)r2.Centerline, 4));
            Assert.AreEqual(288.8314m, Math.Round((decimal)r2.UpperChannel, 4));
            Assert.AreEqual(209.9315m, Math.Round((decimal)r2.LowerChannel, 4));

            StdDevChannelsResult r3 = results[501];
            Assert.AreEqual(279.8653m, Math.Round((decimal)r3.Centerline, 4));
            Assert.AreEqual(319.3152m, Math.Round((decimal)r3.UpperChannel, 4));
            Assert.AreEqual(240.4153m, Math.Round((decimal)r3.LowerChannel, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<StdDevChannelsResult> r = Indicator.GetStdDevChannels(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStdDevChannels(history, 0));

            // bad standard deviations
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetStdDevChannels(history, 20, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetStdDevChannels(HistoryTestData.Get(19), 20, 2));
        }
    }
}
