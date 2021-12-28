using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Sma : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<SmaResult> results = quotes.GetSma(20)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());

            // sample values
            Assert.IsNull(results[18].Sma);
            Assert.AreEqual(214.5250m, Math.Round(results[19].Sma.Value, 4));
            Assert.AreEqual(215.0310m, Math.Round(results[24].Sma.Value, 4));
            Assert.AreEqual(234.9350m, Math.Round(results[149].Sma.Value, 4));
            Assert.AreEqual(255.5500m, Math.Round(results[249].Sma.Value, 4));
            Assert.AreEqual(251.8600m, Math.Round(results[501].Sma.Value, 4));
        }

        [TestMethod]
        public void OpenCandlePart()
        {

            List<SmaResult> results = quotes.GetSma(20, CandlePart.Open)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());

            // sample values
            Assert.IsNull(results[18].Sma);
            Assert.AreEqual(214.3795m, Math.Round(results[19].Sma.Value, 4));
            Assert.AreEqual(214.9535m, Math.Round(results[24].Sma.Value, 4));
            Assert.AreEqual(234.8280m, Math.Round(results[149].Sma.Value, 4));
            Assert.AreEqual(255.6915m, Math.Round(results[249].Sma.Value, 4));
            Assert.AreEqual(253.1725m, Math.Round(results[501].Sma.Value, 4));
        }

        [TestMethod]
        public void VolumeCandlePart()
        {
            List<SmaResult> results = quotes.GetSma(20, CandlePart.Volume)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Sma != null).Count());

            // sample values
            SmaResult r24 = results[24];
            Assert.AreEqual(77293768.2m, r24.Sma);

            SmaResult r290 = results[290];
            Assert.AreEqual(157958070.8m, r290.Sma);

            SmaResult r501 = results[501];
            Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture), r501.Date);
            Assert.AreEqual(163695200m, r501.Sma);
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<SmaResult> r = Indicator.GetSma(badQuotes, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<SmaResult> results = quotes.GetSma(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);
            Assert.AreEqual(251.8600m, Math.Round(results.LastOrDefault().Sma.Value, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetSma(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetSma(TestData.GetDefault(9), 10));
        }
    }
}
