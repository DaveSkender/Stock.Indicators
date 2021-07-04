using System;
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
            IEnumerable<Quote> history = HistoryTestData.Get();
            IEnumerable<EmaResult> emaResults = Indicator.GetEma(history, 20);

            // find specific date
            DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", englishCulture);

            EmaResult r = emaResults.Find(findDate);
            Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
        }

        [TestMethod]
        public void PruneException()
        {
            // bad prune period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                history.GetAdx(14).PruneWarmupPeriods(-1)); ;
        }

    }
}
