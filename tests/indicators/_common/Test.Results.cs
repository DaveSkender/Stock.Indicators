using System;
using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<Quote> quotes = TestData.GetDefault();
            IEnumerable<EmaResult> emaResults = Indicator.GetEma(quotes, 20);

            // find specific date
            DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

            EmaResult r = emaResults.Find(findDate);
            Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
        }


        [TestMethod]
        public void Remove()
        {
            // specific periods
            IEnumerable<HeikinAshiResult> results =
                quotes.GetHeikinAshi()
                  .RemoveWarmupPeriods(102);

            Assert.AreEqual(400, results.Count());
        }

        [TestMethod]
        public void RemoveTooMany()
        {
            // more than available
            IEnumerable<HeikinAshiResult> results =
                quotes.GetHeikinAshi()
                  .RemoveWarmupPeriods(600);

            Assert.AreEqual(0, results.Count());
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
