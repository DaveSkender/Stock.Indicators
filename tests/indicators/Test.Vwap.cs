using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Vwap : TestBase
    {
        private readonly IEnumerable<Quote> intraday = HistoryTestData.GetIntraday()
            .OrderBy(x => x.Date)
            .Take(391);

        [TestMethod]
        public void Standard()
        {

            List<VwapResult> results = intraday.GetVwap()
                .ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(391, results.Count);
            Assert.AreEqual(391, results.Where(x => x.Vwap != null).Count());

            // sample values
            VwapResult r1 = results[0];
            Assert.AreEqual(367.4800m, Math.Round((decimal)r1.Vwap, 4));

            VwapResult r2 = results[1];
            Assert.AreEqual(367.4223m, Math.Round((decimal)r2.Vwap, 4));

            VwapResult r3 = results[369];
            Assert.AreEqual(367.9494m, Math.Round((decimal)r3.Vwap, 4));

            VwapResult r4 = results[390];
            Assert.AreEqual(368.1804m, Math.Round((decimal)r4.Vwap, 4));
        }

        [TestMethod]
        public void WithStartDate()
        {
            DateTime startDate =
                DateTime.ParseExact("2020-12-15 10:00", "yyyy-MM-dd h:mm", englishCulture);

            List<VwapResult> results = Indicator.GetVwap(intraday, startDate)
                .ToList();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(391, results.Count);
            Assert.AreEqual(361, results.Where(x => x.Vwap != null).Count());

            // sample values
            VwapResult r1 = results[29];
            Assert.AreEqual(null, r1.Vwap);

            VwapResult r2 = results[30];
            Assert.AreEqual(366.8100m, Math.Round((decimal)r2.Vwap, 4));

            VwapResult r3 = results[369];
            Assert.AreEqual(368.0511m, Math.Round((decimal)r3.Vwap, 4));

            VwapResult r4 = results[390];
            Assert.AreEqual(368.2908m, Math.Round((decimal)r4.Vwap, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<VwapResult> r = Indicator.GetVwap(historyBad);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad SMA period
            DateTime startDate =
                DateTime.ParseExact("2000-12-15", "yyyy-MM-dd", englishCulture);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetVwap(history, startDate));

            // insufficient history
            Assert.ThrowsException<BadHistoryException>(() =>
                Indicator.GetVwap(HistoryTestData.Get(0)));
        }
    }
}
