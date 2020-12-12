using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Tests
{
    [TestClass]
    public class CleanerTests : TestBase
    {

        [TestMethod()]
        public void ValidateHistoryTest()
        {
            IEnumerable<Quote> history = History.GetHistory();

            // clean
            List<Quote> h = Cleaners.ValidateHistory(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, h.Count);

            // check last date
            DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(lastDate, h[501].Date);

            // spot check an out of sequence date
            DateTime spotDate = DateTime.ParseExact("02/01/2017", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(spotDate, h[20].Date);
        }


        [TestMethod()]
        public void ValidateLongHistoryTest()
        {
            IEnumerable<Quote> historyLong = History.GetHistoryLong();

            List<Quote> h = Cleaners.ValidateHistory(historyLong);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(5285, h.Count);

            // check last date
            DateTime lastDate = DateTime.ParseExact("09/04/2020", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(lastDate, h[5284].Date);
        }


        [TestMethod()]
        public void CutHistoryTest()
        {
            // if history post-cleaning, is cut down in size it should not corrupt the results

            IEnumerable<Quote> history = History.GetHistory(200);
            List<Quote> h = Cleaners.ValidateHistory(history);

            // assertions

            // should be 200 periods, initially
            Assert.AreEqual(200, h.Count);

            // should be 20 results and no index corruption
            List<SmaResult> r1 = Indicator.GetSma(h.TakeLast(20), 14).ToList();
            Assert.AreEqual(20, r1.Count);

            for (int i = 1; i < r1.Count; i++)
            {
                Assert.IsTrue(r1[i].Date >= r1[i - 1].Date);
            }

            // should be 50 results and no index corruption
            List<SmaResult> r2 = Indicator.GetSma(h.TakeLast(50), 14).ToList();
            Assert.AreEqual(50, r2.Count);

            for (int i = 1; i < r2.Count; i++)
            {
                Assert.IsTrue(r2[i].Date >= r2[i - 1].Date);
            }

            // should be original 200 periods and no index corruption, after temp mods
            Assert.AreEqual(200, h.Count);

            for (int i = 1; i < h.Count; i++)
            {
                Assert.IsTrue(h[i].Date >= h[i - 1].Date);
            }
        }


        [TestMethod()]
        public void SortHistoryTest()
        {
            IEnumerable<Quote> history = History.GetHistory();

            // clean
            List<Quote> h = history.Sort();

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, h.Count);

            // check last date
            DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(lastDate, h[501].Date);

            // spot check an out of sequence date
            DateTime spotDate = DateTime.ParseExact("02/01/2017", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(spotDate, h[20].Date);
        }


        [TestMethod()]
        public void CleanBasicDataTest()
        {
            // compose basic data
            List<BasicData> o = Cleaners.ConvertHistoryToBasic(history, "O");
            List<BasicData> h = Cleaners.ConvertHistoryToBasic(history, "H");
            List<BasicData> l = Cleaners.ConvertHistoryToBasic(history, "L");
            List<BasicData> c = Cleaners.ConvertHistoryToBasic(history, "C");
            List<BasicData> v = Cleaners.ConvertHistoryToBasic(history, "V");

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, c.Count);

            // samples
            BasicData ro = o[501];
            BasicData rh = h[501];
            BasicData rl = l[501];
            BasicData rc = c[501];
            BasicData rv = v[501];

            // proper last date
            DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(lastDate, rc.Date);

            // last values should be correct
            Assert.AreEqual(244.92m, ro.Value);
            Assert.AreEqual(245.54m, rh.Value);
            Assert.AreEqual(242.87m, rl.Value);
            Assert.AreEqual(245.28m, rc.Value);
            Assert.AreEqual(147031456, rv.Value);
        }


        /* BAD HISTORY EXCEPTIONS */
        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "No historical quotes.")]
        public void NoHistory()
        {
            List<Quote> badHistory = new List<Quote>();
            Cleaners.ValidateHistory(badHistory);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Duplicate date found.")]
        public void DuplicateHistory()
        {
            List<Quote> badHistory = new List<Quote>
            {
            new Quote { Date = DateTime.ParseExact("2017-01-03", "yyyy-MM-dd", englishCulture), Open=214.86m, High=220.33m, Low=210.96m, Close=216.99m, Volume = 5923254 },
            new Quote { Date = DateTime.ParseExact("2017-01-04", "yyyy-MM-dd", englishCulture), Open=214.75m, High=228.00m, Low=214.31m, Close=226.99m, Volume = 11213471 },
            new Quote { Date = DateTime.ParseExact("2017-01-05", "yyyy-MM-dd", englishCulture), Open=226.42m, High=227.48m, Low=221.95m, Close=226.75m, Volume = 5911695 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", englishCulture), Open=226.93m, High=230.31m, Low=225.45m, Close=229.01m, Volume = 5527893 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", englishCulture), Open=228.97m, High=231.92m, Low=228.00m, Close=231.28m, Volume = 3979484 }
            };

            Cleaners.ValidateHistory(badHistory);
        }


        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "No historical basic data.")]
        public void NoBasicData()
        {
            List<Quote> h = new List<Quote>();
            Cleaners.ConvertHistoryToBasic(h);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Bad element.")]
        public void CleanBasicDataBadParamTest()
        {
            // compose basic data
            Cleaners.ConvertHistoryToBasic(history, "E");
        }
    }
}