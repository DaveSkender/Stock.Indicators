using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Tsi : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<TsiResult> results = quotes.GetTsi(25, 13, 7).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(465, results.Where(x => x.Tsi != null).Count());
            Assert.AreEqual(459, results.Where(x => x.Signal != null).Count());

            // sample values
            TsiResult r2 = results[37];
            Assert.AreEqual(53.1204, Math.Round((double)r2.Tsi, 4));
            Assert.AreEqual(null, r2.Signal);

            TsiResult r3a = results[43];
            Assert.AreEqual(46.0960, Math.Round((double)r3a.Tsi, 4));
            Assert.AreEqual(51.6916, Math.Round((double)r3a.Signal, 4));

            TsiResult r3b = results[44];
            Assert.AreEqual(42.5121, Math.Round((double)r3b.Tsi, 4));
            Assert.AreEqual(49.3967, Math.Round((double)r3b.Signal, 4));

            TsiResult r4 = results[149];
            Assert.AreEqual(29.0936, Math.Round((double)r4.Tsi, 4));
            Assert.AreEqual(28.0134, Math.Round((double)r4.Signal, 4));

            TsiResult r5 = results[249];
            Assert.AreEqual(41.9232, Math.Round((double)r5.Tsi, 4));
            Assert.AreEqual(42.4063, Math.Round((double)r5.Signal, 4));

            TsiResult r6 = results[501];
            Assert.AreEqual(-28.3513, Math.Round((double)r6.Tsi, 4));
            Assert.AreEqual(-29.3597, Math.Round((double)r6.Signal, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<TsiResult> r = Indicator.GetTsi(badQuotes);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void BigData()
        {
            IEnumerable<TsiResult> r = Indicator.GetTsi(bigQuotes);
            Assert.AreEqual(1246, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<TsiResult> results = quotes.GetTsi(25, 13, 7)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - (25 + 13 + 250), results.Count);

            TsiResult last = results.LastOrDefault();
            Assert.AreEqual(-28.3513, Math.Round((double)last.Tsi, 4));
            Assert.AreEqual(-29.3597, Math.Round((double)last.Signal, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTsi(quotes, 0));

            // bad smoothing period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTsi(quotes, 25, 0));

            // bad signal period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetTsi(quotes, 25, 13, -1));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetTsi(TestData.GetDefault(137), 25, 13, 7));
        }
    }
}
