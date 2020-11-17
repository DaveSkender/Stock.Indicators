using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace External.Tests
{
    public class MyQuote : Quote
    {
        public bool MyProperty { get; set; }
        public decimal MyClose { get; set; }
    }

    public class MyIndicator : EmaResult
    {
        public bool MyProperty { get; set; }
        public float MyEma { get; set; }
    }


    [TestClass]
    public class PublicClassTests
    {

        [TestMethod()]
        public void CleanHistory()
        {
            IEnumerable<Quote> history = History.GetHistory();
            history = Cleaners.ValidateHistory(history);

            Indicator.GetSma(history, 5);
        }

        [TestMethod()]
        public void ReadQuoteClass()
        {
            IEnumerable<Quote> history = History.GetHistory();
            List<Quote> h = Cleaners.ValidateHistory(history);

            Quote f = h.FirstOrDefault();
            Console.WriteLine("Date:{0},Close:{1}", f.Date, f.Close);
        }

        [TestMethod()]
        public void DerivedQuoteClass()
        {
            // can use a derive Quote class
            MyQuote myQuote = new MyQuote
            {
                Date = DateTime.Now,
                MyProperty = true
            };

            Assert.AreEqual(true, myQuote.MyProperty);
        }

        [TestMethod()]
        public void DerivedQuoteClassLinq()
        {
            IEnumerable<Quote> history = History.GetHistory();
            history = Cleaners.ValidateHistory(history);

            // can use a derive Quote class using Linq

            IEnumerable<MyQuote> myHistory = history
                .Select(x => new MyQuote
                {
                    Date = x.Date,
                    MyClose = x.Close,
                    MyProperty = false
                });

            Assert.IsTrue(myHistory.Any());
        }

        [TestMethod()]
        public void DerivedIndicatorClass()
        {
            // can use a derive Indicator class
            MyIndicator myIndicator = new MyIndicator
            {
                Date = DateTime.Now,
                MyEma = 123.456f,
                MyProperty = false
            };

            Assert.AreEqual(false, myIndicator.MyProperty);
        }

        [TestMethod()]
        public void DerivedIndicatorClassLinq()
        {
            IEnumerable<Quote> history = History.GetHistory();
            IEnumerable<EmaResult> emaResults = Indicator.GetEma(history, 14);

            // can use a derive Indicator class using Linq

            IEnumerable<MyIndicator> myIndicatorResults = emaResults
                .Where(x => x.Ema != null)
                .Select(x => new MyIndicator
                {
                    Date = x.Date,
                    MyEma = (float)x.Ema,
                    MyProperty = false
                });

            Assert.IsTrue(myIndicatorResults.Any());
        }

    }
}
