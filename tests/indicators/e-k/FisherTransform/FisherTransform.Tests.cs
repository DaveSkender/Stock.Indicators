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

            List<FisherTransformResult> results = quotes.GetFisherTransform(10)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(501, results.Where(x => x.Fisher != 0).Count());

            // sample values
            Assert.AreEqual(0, results[0].Fisher);
            Assert.IsNull(results[0].Trigger);

            Assert.AreEqual(0.3428, Math.Round(results[1].Fisher.Value, 4));
            Assert.AreEqual(0, results[1].Trigger);

            Assert.AreEqual(0.6873, Math.Round(results[2].Fisher.Value, 4));
            Assert.AreEqual(0.3428, Math.Round(results[2].Trigger.Value, 4));

            Assert.AreEqual(1.3324, Math.Round(results[9].Fisher.Value, 4));
            Assert.AreEqual(1.4704, Math.Round(results[9].Trigger.Value, 4));

            Assert.AreEqual(0.9790, Math.Round(results[10].Fisher.Value, 4));
            Assert.AreEqual(1.3324, Math.Round(results[10].Trigger.Value, 4));

            Assert.AreEqual(6.1509, Math.Round(results[35].Fisher.Value, 4));
            Assert.AreEqual(4.7014, Math.Round(results[35].Trigger.Value, 4));

            Assert.AreEqual(5.4455, Math.Round(results[36].Fisher.Value, 4));
            Assert.AreEqual(6.1509, Math.Round(results[36].Trigger.Value, 4));

            Assert.AreEqual(1.0349, Math.Round(results[149].Fisher.Value, 4));
            Assert.AreEqual(0.7351, Math.Round(results[149].Trigger.Value, 4));

            Assert.AreEqual(1.3496, Math.Round(results[249].Fisher.Value, 4));
            Assert.AreEqual(1.4408, Math.Round(results[249].Trigger.Value, 4));

            Assert.AreEqual(-1.2876, Math.Round(results[501].Fisher.Value, 4));
            Assert.AreEqual(-2.0071, Math.Round(results[501].Trigger.Value, 4));
        }


        [TestMethod]
        public void BadData()
        {
            IEnumerable<FisherTransformResult> r = Indicator.GetFisherTransform(badQuotes, 9);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetFisherTransform(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetFisherTransform(TestData.GetDefault(9), 10));
        }
    }
}
