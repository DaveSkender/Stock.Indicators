using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class HtTrendline : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<HtlResult> results = history.GetHtTrendline().ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(502, results.Where(x => x.Trendline != null).Count());
            Assert.AreEqual(496, results.Where(x => x.SmoothPrice != null).Count());

            // sample values
            HtlResult r1 = results[5];
            Assert.AreEqual(214.205m, r1.Trendline);
            Assert.AreEqual(null, r1.SmoothPrice);

            HtlResult r2 = results[6];
            Assert.AreEqual(213.84m, r2.Trendline);
            Assert.AreEqual(214.071m, r2.SmoothPrice);

            HtlResult r3 = results[11];
            Assert.AreEqual(213.9502m, Math.Round((decimal)r3.Trendline, 4));
            Assert.AreEqual(213.8460m, Math.Round((decimal)r3.SmoothPrice, 4));

            HtlResult r4 = results[25];
            Assert.AreEqual(215.3948m, Math.Round((decimal)r4.Trendline, 4));
            Assert.AreEqual(216.3365m, Math.Round((decimal)r4.SmoothPrice, 4));

            HtlResult r5 = results[149];
            Assert.AreEqual(233.9410m, Math.Round((decimal)r5.Trendline, 4));
            Assert.AreEqual(235.8570m, Math.Round((decimal)r5.SmoothPrice, 4));

            HtlResult r6 = results[249];
            Assert.AreEqual(253.8788m, Math.Round((decimal)r6.Trendline, 4));
            Assert.AreEqual(257.5825m, Math.Round((decimal)r6.SmoothPrice, 4));

            HtlResult r7 = results[501];
            Assert.AreEqual(252.2172m, Math.Round((decimal)r7.Trendline, 4));
            Assert.AreEqual(242.3435m, Math.Round((decimal)r7.SmoothPrice, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<HtlResult> r = Indicator.GetHtTrendline(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void PennyData()
        {
            IEnumerable<Quote> penny = HistoryTestData.GetPenny();
            IEnumerable<HtlResult> r = Indicator.GetHtTrendline(penny);
            Assert.AreEqual(533, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetHtTrendline(HistoryTestData.Get(99)));
        }
    }
}
