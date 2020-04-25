using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;

namespace StockIndicators.Tests
{
    [TestClass]
    public class CleanerTests : TestBase
    {

        [TestMethod()]
        public void CleanerTest()
        {
            List<Quote> h = Cleaners.PrepareHistory(history);

            // assertions

            // should always be the same number of results as there is history
            Assert.AreEqual(502, h.Count);
        }



        /* BAD HISTORY EXCEPTIONS */

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "No historical quote provided.")]
        public void BadHistoryEmptyTest()
        {
            List<Quote> badHistory = new List<Quote>();
            Cleaners.PrepareHistory(badHistory);
        }

        [TestMethod()]
        [ExpectedException(typeof(BadHistoryException), "Duplicate date found.")]
        public void BadHistoryDuplicateTest()
        {
            List<Quote> badHistory = new List<Quote>
            {
            new Quote { Date = DateTime.Parse("2017-01-03"), Open=(decimal)214.86, High=(decimal)220.33, Low=(decimal)210.96, Close=(decimal)216.99, Volume = 5923254 },
            new Quote { Date = DateTime.Parse("2017-01-04"), Open=(decimal)214.75, High=228, Low=(decimal)214.31, Close=(decimal)226.99, Volume = 11213471 },
            new Quote { Date = DateTime.Parse("2017-01-05"), Open=(decimal)226.42, High=(decimal)227.48, Low=(decimal)221.95, Close=(decimal)226.75, Volume = 5911695 },
            new Quote { Date = DateTime.Parse("2017-01-06"), Open=(decimal)226.93, High=(decimal)230.31, Low=(decimal)225.45, Close=(decimal)229.01, Volume = 5527893 },
            new Quote { Date = DateTime.Parse("2017-01-06"), Open=(decimal)228.97, High=(decimal)231.92, Low=228, Close=(decimal)231.28, Volume = 3979484 },
            };

            Cleaners.PrepareHistory(badHistory);
        }

    }
}