using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Candles : TestBase
    {
        [TestMethod]
        public void SortCandles()
        {
            IEnumerable<Quote> quotes = TestData.GetMismatch();

            // sort
            List<Candle> candles = quotes.ConvertToCandles();

            // assertions

            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, candles.Count);

            // check first date
            DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(firstDate, candles[0].Date);

            // check last date
            DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(lastDate, candles.LastOrDefault().Date);

            // spot check an out of sequence date
            DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", englishCulture);
            Assert.AreEqual(spotDate, candles[50].Date);
        }

        [TestMethod]
        public void CandleValues()
        {
            // sort
            List<Candle> candles = quotes.ConvertToCandles();

            // assertions

            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, candles.Count);

            // sample values
            Candle r0 = candles[0];
            Assert.AreEqual(212.8m, r0.Close);
            Assert.AreEqual(1.83m, r0.Size);
            Assert.AreEqual(0.19m, r0.Body);
            Assert.AreEqual(0.55m, r0.UpperWick);
            Assert.AreEqual(1.09m, r0.LowerWick);
            Assert.AreEqual(0.10383m, Math.Round(r0.BodyPct, 5));
            Assert.AreEqual(0.30055m, Math.Round(r0.UpperWickPct, 5));
            Assert.AreEqual(0.59563m, Math.Round(r0.LowerWickPct, 5));
            Assert.IsTrue(r0.IsBullish);
            Assert.IsFalse(r0.IsBearish);

            Candle r351 = candles[351];
            Assert.AreEqual(1.24m, r351.Size);
            Assert.AreEqual(0m, r351.Body);
            Assert.AreEqual(0.69m, r351.UpperWick);
            Assert.AreEqual(0.55m, r351.LowerWick);
            Assert.AreEqual(0m, Math.Round(r351.BodyPct, 5));
            Assert.AreEqual(0.55645m, Math.Round(r351.UpperWickPct, 5));
            Assert.AreEqual(0.44355m, Math.Round(r351.LowerWickPct, 5));
            Assert.IsFalse(r351.IsBullish);
            Assert.IsFalse(r351.IsBearish);

            Candle r501 = candles[501];
            Assert.AreEqual(2.67m, r501.Size);
            Assert.AreEqual(0.36m, r501.Body);
            Assert.AreEqual(0.26m, r501.UpperWick);
            Assert.AreEqual(2.05m, r501.LowerWick);
            Assert.AreEqual(0.13483m, Math.Round(r501.BodyPct, 5));
            Assert.AreEqual(0.09738m, Math.Round(r501.UpperWickPct, 5));
            Assert.AreEqual(0.76779m, Math.Round(r501.LowerWickPct, 5));
            Assert.IsTrue(r501.IsBullish);
            Assert.IsFalse(r501.IsBearish);
        }

    }
}
