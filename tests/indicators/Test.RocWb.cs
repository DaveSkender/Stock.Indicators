using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class RocWb : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<RocWbResult> results = history.GetRocWb(20, 3, 20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(482, results.Where(x => x.Roc != null).Count());
            Assert.AreEqual(480, results.Where(x => x.RocEma != null).Count());
            Assert.AreEqual(463, results.Where(x => x.UpperBand != null).Count());
            Assert.AreEqual(463, results.Where(x => x.LowerBand != null).Count());

            // sample values
            RocWbResult r19 = results[19];
            Assert.IsNull(r19.Roc);
            Assert.IsNull(r19.RocEma);
            Assert.IsNull(r19.UpperBand);
            Assert.IsNull(r19.LowerBand);

            RocWbResult r20 = results[20];
            Assert.AreEqual(1.0573m, Math.Round(r20.Roc.Value, 4));
            Assert.IsNull(r20.RocEma);
            Assert.IsNull(r20.UpperBand);
            Assert.IsNull(r20.LowerBand);

            RocWbResult r22 = results[22];
            Assert.AreEqual(0.9617m, Math.Round(r22.RocEma.Value, 4));
            Assert.IsNull(r22.UpperBand);
            Assert.IsNull(r22.LowerBand);

            RocWbResult r23 = results[23];
            Assert.AreEqual(0.8582m, Math.Round(r23.RocEma.Value, 4));
            Assert.IsNull(r23.UpperBand);
            Assert.IsNull(r23.LowerBand);

            RocWbResult r38 = results[38];
            Assert.AreEqual(3.6872m, Math.Round(r38.RocEma.Value, 4));
            Assert.IsNull(r38.UpperBand);
            Assert.IsNull(r38.LowerBand);

            RocWbResult r39 = results[39];
            Assert.AreEqual(4.5348m, Math.Round(r39.RocEma.Value, 4));
            Assert.AreEqual(3.0359m, Math.Round(r39.UpperBand.Value, 4));
            Assert.AreEqual(-3.0359m, Math.Round(r39.LowerBand.Value, 4));

            RocWbResult r49 = results[49];
            Assert.AreEqual(2.3147m, Math.Round(r49.RocEma.Value, 4));
            Assert.AreEqual(3.6761m, Math.Round(r49.UpperBand.Value, 4));

            RocWbResult r149 = results[149];
            Assert.AreEqual(1.7377m, Math.Round(r149.UpperBand.Value, 4));

            RocWbResult r249 = results[249];
            Assert.AreEqual(3.0683m, Math.Round(r249.UpperBand.Value, 4));

            RocWbResult r501 = results[501];
            Assert.AreEqual(-8.2482m, Math.Round(r501.Roc.Value, 4));
            Assert.AreEqual(-8.3390m, Math.Round(r501.RocEma.Value, 4));
            Assert.AreEqual(6.1294m, Math.Round(r501.UpperBand.Value, 4));
            Assert.AreEqual(-6.1294m, Math.Round(r501.LowerBand.Value, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<RocWbResult> r = Indicator.GetRocWb(historyBad, 35, 3, 35);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRocWb(history, 0, 3, 12));

            // bad EMA period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRocWb(history, 14, 0, 14));

            // bad STDDEV period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetRocWb(history, 15, 3, 16));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetRocWb(HistoryTestData.Get(10), 10, 2, 10));
        }
    }
}
