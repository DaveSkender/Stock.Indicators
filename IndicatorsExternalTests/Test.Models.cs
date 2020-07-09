using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using StockIndicators.Tests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsExternalTests
{
    public class MyQuote : Quote
    {
        public new int Index { get; set; }
        public bool MyProperty { get; set; }
        public decimal MyClose { get; set; }
    }

    public class MyIndicator : EmaResult
    {
        public new int Index { get; set; }
        public bool MyProperty { get; set; }
        public float MyEma { get; set; }
    }


    [TestClass]
    public class ExternalModelTests
    {

        [TestMethod()]
        public void ReadQuoteClass()
        {
            IEnumerable<Quote> history = History.GetHistory();
            List<Quote> h = Cleaners.PrepareHistory(history);

            Quote f = h.FirstOrDefault();
            Console.WriteLine("Index:{0},Date:{1},Close:{2}", f.Index, f.Date, f.Close);
        }

        [TestMethod()]
        public void DerivedQuoteClass()
        {
            // can use a derive Quote class
            MyQuote myQuote = new MyQuote
            {
                Index = 0,
                Date = DateTime.Now,
                MyProperty = true
            };

            Assert.AreEqual(0, myQuote.Index);
        }

        [TestMethod()]
        public void DerivedQuoteClassLinq()
        {
            IEnumerable<Quote> history = History.GetHistory();
            history = Cleaners.PrepareHistory(history);

            // can use a derive Quote class using Linq

            IEnumerable<MyQuote> myHistory = history
                .Select(x => new MyQuote
                {
                    Index = (int)x.Index,
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
                Index = 0,
                Date = DateTime.Now,
                MyEma = 123.456f,
                MyProperty = false
            };

            Assert.AreEqual(0, myIndicator.Index);
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
                    Index = x.Index,
                    Date = x.Date,
                    MyEma = (float)x.Ema,
                    MyProperty = false
                });

            Assert.IsTrue(myIndicatorResults.Any());
        }

    }
}
