﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Results : TestBase
    {

        [TestMethod]
        public void Find()
        {
            IEnumerable<Quote> quotes = HistoryTestData.Get();
            IEnumerable<EmaResult> emaResults = Indicator.GetEma(quotes, 20);

            // find specific date
            DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", englishCulture);

            EmaResult r = emaResults.Find(findDate);
            Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
        }

        [TestMethod]
        public void RemoveException()
        {
            // bad remove period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                quotes.GetAdx(14).RemoveWarmupPeriods(-1)); ;
        }

    }
}
