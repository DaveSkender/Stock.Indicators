﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class ForceIndex : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            List<ForceIndexResult> r = quotes.GetForceIndex(13).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, r.Count);
            Assert.AreEqual(489, r.Where(x => x.ForceIndex != null).Count());

            // sample values
            Assert.IsNull(r[12].ForceIndex);

            Assert.AreEqual(10668240.778m, Math.Round(r[13].ForceIndex.Value, 3));
            Assert.AreEqual(15883211.364m, Math.Round(r[24].ForceIndex.Value, 3));
            Assert.AreEqual(7598218.196m, Math.Round(r[149].ForceIndex.Value, 3));
            Assert.AreEqual(23612118.994m, Math.Round(r[249].ForceIndex.Value, 3));
            Assert.AreEqual(-16824018.428m, Math.Round(r[501].ForceIndex.Value, 3));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<ForceIndexResult> r = Indicator.GetForceIndex(historyBad, 2);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<ForceIndexResult> results = quotes.GetForceIndex(13)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (13 + 100), results.Count);

            ForceIndexResult last = results.LastOrDefault();
            Assert.AreEqual(-16824018.428m, Math.Round(last.ForceIndex.Value, 3));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetForceIndex(quotes, 0));

            // insufficient quotes for N+100
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetForceIndex(HistoryTestData.Get(129), 30));

            // insufficient quotes for 2×N
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetForceIndex(HistoryTestData.Get(499), 250));
        }
    }
}
