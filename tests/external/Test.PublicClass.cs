using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

[assembly: CLSCompliant(true)]
namespace External.Other
{
    internal class MyQuote : Quote
    {
        public bool MyProperty { get; set; }
        public decimal MyClose { get; set; }
    }

    internal class MyIndicator : EmaResult
    {
        public int Id { get; set; }
        public bool MyProperty { get; set; }
        public float MyEma { get; set; }
    }

    internal class MyGenericQuote : IQuote
    {
        // required base properties
        DateTime IQuote.Date => CloseDate;
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        decimal IQuote.Close => CloseValue;
        public decimal Volume { get; set; }

        // custom properties
        public int MyOtherProperty { get; set; }
        public DateTime CloseDate { get; set; }
        public decimal CloseValue { get; set; }
    }


    [TestClass]
    public class PublicClassTests
    {
        internal static readonly CultureInfo englishCulture = new("en-US", false);

        [TestMethod]
        public void ValidateHistory()
        {
            IEnumerable<Quote> history = HistoryTestData.Get();
            history.Validate();

            history.GetSma(6);
            Indicator.GetSma(history, 5);
        }

        [TestMethod]
        public void ReadQuoteClass()
        {
            IEnumerable<Quote> history = HistoryTestData.Get();
            IEnumerable<Quote> h = history.Validate();

            Quote f = h.FirstOrDefault();
            Console.WriteLine("Date:{0},Close:{1}", f.Date, f.Close);
        }

        [TestMethod]
        public void DerivedQuoteClass()
        {
            // can use a derive Quote class
            MyQuote myQuote = new()
            {
                Date = DateTime.Now,
                MyProperty = true
            };

            Assert.AreEqual(true, myQuote.MyProperty);
        }

        [TestMethod]
        public void DerivedQuoteClassLinq()
        {
            IEnumerable<Quote> history = HistoryTestData.Get();
            history = history.Validate();

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

        [TestMethod]
        public void CustomQuoteClass()
        {
            List<MyGenericQuote> myGenericHistory = HistoryTestData.Get()
                .Select(x => new MyGenericQuote
                {
                    CloseDate = x.Date,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    CloseValue = x.Close,
                    Volume = x.Volume,
                    MyOtherProperty = 123456
                })
                .ToList();

            List<EmaResult> results = myGenericHistory.GetEma(20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Ema != null).Count());

            // sample values
            EmaResult r1 = results[501];
            Assert.AreEqual(249.3519m, Math.Round((decimal)r1.Ema, 4));

            EmaResult r2 = results[249];
            Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

            EmaResult r3 = results[29];
            Assert.AreEqual(216.6228m, Math.Round((decimal)r3.Ema, 4));
        }

        [TestMethod]
        public void CustomQuoteAggregate()
        {
            List<MyGenericQuote> myGenericHistory = HistoryTestData.GetIntraday()
                .Select(x => new MyGenericQuote
                {
                    CloseDate = x.Date,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    CloseValue = x.Close,
                    Volume = x.Volume,
                    MyOtherProperty = 123456
                })
                .ToList();

            List<Quote> historyList = myGenericHistory
                .Aggregate(PeriodSize.TwoHours)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is history
            Assert.AreEqual(20, historyList.Count);

            // sample values
            Quote r19 = historyList[19];
            Assert.AreEqual(369.04m, r19.Low);
        }


        [TestMethod]
        public void DerivedIndicatorClass()
        {
            // can use a derive Indicator class
            MyIndicator myIndicator = new()
            {
                Date = DateTime.Now,
                MyEma = 123.456f,
                MyProperty = false
            };

            Assert.AreEqual(false, myIndicator.MyProperty);
        }

        [TestMethod]
        public void DerivedIndicatorClassLinq()
        {
            IEnumerable<Quote> history = HistoryTestData.Get();
            IEnumerable<EmaResult> emaResults = history.GetEma(14);

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

        [TestMethod]
        public void DerivedIndicatorFind()
        {
            IEnumerable<Quote> history = HistoryTestData.Get();
            IEnumerable<EmaResult> emaResults = Indicator.GetEma(history, 20);

            // can use a derive Indicator class using Linq

            IEnumerable<MyIndicator> myIndicatorResults = emaResults
                .Where(x => x.Ema != null)
                .Select(x => new MyIndicator
                {
                    Id = 12345,
                    Date = x.Date,
                    MyEma = (float)x.Ema,
                    MyProperty = false
                });

            Assert.IsTrue(myIndicatorResults.Any());

            // find specific date
            DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", englishCulture);

            MyIndicator i = myIndicatorResults.Find(findDate);
            Assert.AreEqual(12345, i.Id);

            EmaResult r = emaResults.Find(findDate);
            Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
        }



    }
}
