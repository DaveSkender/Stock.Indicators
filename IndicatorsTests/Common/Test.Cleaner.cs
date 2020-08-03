﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockIndicators.Tests
{
    [TestClass]
    public class CleanerTests : TestBase
    {

        [TestMethod()]
        public void PrepareHistoryTest()
        {
            IEnumerable<Quote> h = Cleaners.PrepareHistory(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, h.Count());

            // should always have index
            Assert.IsFalse(h.Where(x => x.Index == null || x.Index <= 0).Any());

            // last index should be 502
            Quote r = history
                .Where(x => x.Date == DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", cultureProvider))
                .FirstOrDefault();

            Assert.AreEqual(502, r.Index);
        }


        [TestMethod()]
        public void CleanBasicDataTest()
        {
            // compose basic data
            IEnumerable<BasicData> o = Cleaners.ConvertHistoryToBasic(history, "O");
            IEnumerable<BasicData> h = Cleaners.ConvertHistoryToBasic(history, "H");
            IEnumerable<BasicData> l = Cleaners.ConvertHistoryToBasic(history, "L");
            IEnumerable<BasicData> c = Cleaners.ConvertHistoryToBasic(history, "C");
            IEnumerable<BasicData> v = Cleaners.ConvertHistoryToBasic(history, "V");

            // remove index
            foreach (BasicData x in c) { x.Index = null; }

            // re-clean index
            c = Cleaners.PrepareBasicData(c);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, c.Count());

            // should always have index
            Assert.IsFalse(c.Where(x => x.Index == null || x.Index <= 0).Any());

            // samples
            BasicData ro = o.Where(x => x.Index == 502).FirstOrDefault();
            BasicData rh = h.Where(x => x.Index == 502).FirstOrDefault();
            BasicData rl = l.Where(x => x.Index == 502).FirstOrDefault();
            BasicData rv = v.Where(x => x.Index == 502).FirstOrDefault();

            // use close as special case to evaluate index
            BasicData rc = c
                .Where(x => x.Date == DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", cultureProvider))
                .FirstOrDefault();

            // last index should be 502
            Assert.AreEqual(502, rc.Index);

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
            Cleaners.PrepareHistory(badHistory);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Duplicate date found.")]
        public void DuplicateHistory()
        {
            List<Quote> badHistory = new List<Quote>
            {
            new Quote { Date = DateTime.ParseExact("2017-01-03", "yyyy-MM-dd", cultureProvider), Open=214.86m, High=220.33m, Low=210.96m, Close=216.99m, Volume = 5923254 },
            new Quote { Date = DateTime.ParseExact("2017-01-04", "yyyy-MM-dd", cultureProvider), Open=214.75m, High=228.00m, Low=214.31m, Close=226.99m, Volume = 11213471 },
            new Quote { Date = DateTime.ParseExact("2017-01-05", "yyyy-MM-dd", cultureProvider), Open=226.42m, High=227.48m, Low=221.95m, Close=226.75m, Volume = 5911695 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", cultureProvider), Open=226.93m, High=230.31m, Low=225.45m, Close=229.01m, Volume = 5527893 },
            new Quote { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", cultureProvider), Open=228.97m, High=231.92m, Low=228.00m, Close=231.28m, Volume = 3979484 }
            };

            Cleaners.PrepareHistory(badHistory);
        }


        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "No historical basic data.")]
        public void NoBasicData()
        {
            List<BasicData> bd = new List<BasicData>();
            Cleaners.PrepareBasicData(bd);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Duplicate date found.")]
        public void DuplicateBasicData()
        {
            List<BasicData> bd = new List<BasicData>
            {
            new BasicData { Date = DateTime.ParseExact("2017-01-03", "yyyy-MM-dd", cultureProvider), Value=214.86m},
            new BasicData { Date = DateTime.ParseExact("2017-01-04", "yyyy-MM-dd", cultureProvider), Value=214.75m},
            new BasicData { Date = DateTime.ParseExact("2017-01-05", "yyyy-MM-dd", cultureProvider), Value=226.42m},
            new BasicData { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", cultureProvider), Value=226.93m},
            new BasicData { Date = DateTime.ParseExact("2017-01-06", "yyyy-MM-dd", cultureProvider), Value=228.97m}
            };

            Cleaners.PrepareBasicData(bd);
        }

    }
}