using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class FisherTransform : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<FisherTransformResult> results = history.GetFisherTransform(10)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(501, results.Where(x => x.Fisher != 0).Count());

            // sample values
            Assert.AreEqual(0, results[0].Fisher);
            Assert.IsNull(results[0].Trigger);

            Assert.AreEqual(0.3428m, Math.Round(results[1].Fisher.Value, 4));
            Assert.AreEqual(0, results[1].Trigger);

            Assert.AreEqual(0.6873m, Math.Round(results[2].Fisher.Value, 4));
            Assert.AreEqual(0.3428m, Math.Round(results[2].Trigger.Value, 4));

            Assert.AreEqual(1.3324m, Math.Round(results[9].Fisher.Value, 4));
            Assert.AreEqual(1.4704m, Math.Round(results[9].Trigger.Value, 4));

            Assert.AreEqual(0.9790m, Math.Round(results[10].Fisher.Value, 4));
            Assert.AreEqual(1.3324m, Math.Round(results[10].Trigger.Value, 4));

            Assert.AreEqual(6.1509m, Math.Round(results[35].Fisher.Value, 4));
            Assert.AreEqual(4.7014m, Math.Round(results[35].Trigger.Value, 4));

            Assert.AreEqual(5.4455m, Math.Round(results[36].Fisher.Value, 4));
            Assert.AreEqual(6.1509m, Math.Round(results[36].Trigger.Value, 4));

            Assert.AreEqual(1.0349m, Math.Round(results[149].Fisher.Value, 4));
            Assert.AreEqual(0.7351m, Math.Round(results[149].Trigger.Value, 4));

            Assert.AreEqual(1.3496m, Math.Round(results[249].Fisher.Value, 4));
            Assert.AreEqual(1.4408m, Math.Round(results[249].Trigger.Value, 4));

            Assert.AreEqual(-1.2876m, Math.Round(results[501].Fisher.Value, 4));
            Assert.AreEqual(-2.0071m, Math.Round(results[501].Trigger.Value, 4));
        }


        [TestMethod]
        public void BadData()
        {
            IEnumerable<FisherTransformResult> r = Indicator.GetFisherTransform(historyBad, 9);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetFisherTransform(history, 0));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetFisherTransform(HistoryTestData.Get(9), 10));
        }
    }
}
