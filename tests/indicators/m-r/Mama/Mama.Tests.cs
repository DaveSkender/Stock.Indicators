using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Mama : TestBase
    {

        [TestMethod]
        public void Standard()
        {
            double fastLimit = 0.5;
            double slowLimit = 0.05;

            List<MamaResult> results = quotes.GetMama(fastLimit, slowLimit)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(497, results.Where(x => x.Mama != null).Count());

            // sample values
            MamaResult r1 = results[4];
            Assert.AreEqual(null, r1.Mama);
            Assert.AreEqual(null, r1.Fama);

            MamaResult r2 = results[5];
            Assert.AreEqual(213.73m, r2.Mama);
            Assert.AreEqual(213.73m, r2.Fama);

            MamaResult r3 = results[6];
            Assert.AreEqual(213.7850m, Math.Round((decimal)r3.Mama, 4));
            Assert.AreEqual(213.7438m, Math.Round((decimal)r3.Fama, 4));

            MamaResult r4 = results[25];
            Assert.AreEqual(215.9524m, Math.Round((decimal)r4.Mama, 4));
            Assert.AreEqual(215.1407m, Math.Round((decimal)r4.Fama, 4));

            MamaResult r5 = results[149];
            Assert.AreEqual(235.6593m, Math.Round((decimal)r5.Mama, 4));
            Assert.AreEqual(234.3660m, Math.Round((decimal)r5.Fama, 4));

            MamaResult r6 = results[249];
            Assert.AreEqual(256.8026m, Math.Round((decimal)r6.Mama, 4));
            Assert.AreEqual(254.0605m, Math.Round((decimal)r6.Fama, 4));

            MamaResult r7 = results[501];
            Assert.AreEqual(244.1092m, Math.Round((decimal)r7.Mama, 4));
            Assert.AreEqual(252.6139m, Math.Round((decimal)r7.Fama, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<MamaResult> r = Indicator.GetMama(badQuotes);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            double fastLimit = 0.5;
            double slowLimit = 0.05;

            List<MamaResult> results = quotes.GetMama(fastLimit, slowLimit)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 50, results.Count);

            MamaResult last = results.LastOrDefault();
            Assert.AreEqual(244.1092m, Math.Round((decimal)last.Mama, 4));
            Assert.AreEqual(252.6139m, Math.Round((decimal)last.Fama, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad fast period (same as slow period)
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMama(quotes, 0.5, 0.5));

            // bad fast period (cannot be 1 or more)
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMama(quotes, 1, 0.5));

            // bad slow period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetMama(quotes, 0.5, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetMama(TestData.GetDefault(49)));
        }
    }
}
